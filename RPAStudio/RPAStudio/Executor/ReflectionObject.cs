// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionObject.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RPAStudio.Executor
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///   A dynamic object that uses reflection to invoke members
    /// </summary>
    /// <remarks>
    ///   This class makes use of reflection to access non public members.
    ///   This practice is dangerous and may result in code that is easily broken 
    ///   in the future.  The class is marked internal for this reason.
    /// </remarks>
    internal class ReflectionObject : DynamicObject
    {
        #region Constants

        /// <summary>
        ///   The flags.
        /// </summary>
        private const BindingFlags Flags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        #endregion

        #region Fields

        /// <summary>
        ///   The inner.
        /// </summary>
        private readonly object inner;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionObject"/> class. 
        ///   Creates an instance of ReflectionObject
        /// </summary>
        /// <param name="obj">
        /// The inner object 
        /// </param>
        internal ReflectionObject(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            this.inner = obj;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the inner.
        /// </summary>
        public object Inner
        {
            get
            {
                return this.inner;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.) 
        /// </returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive. 
        /// </param>
        /// <param name="result">
        /// The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/> . 
        /// </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.TryGetProperty(binder.Name, out result) || this.TryGetField(binder.Name, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.) 
        /// </returns>
        /// <param name="binder">
        /// Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive. 
        /// </param>
        /// <param name="args">
        /// The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, args[0] is equal to 100. 
        /// </param>
        /// <param name="result">
        /// The result of the member invocation. 
        /// </param>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var method = GetMethod(this.Inner.GetType(), binder.Name, GetTypes(args));

            if (method != null)
            {
                result = method.Invoke(this.Inner, args);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Tries to set a property or field with the given name
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <returns>
        /// The try set member. 
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return this.TrySetProperty(binder.Name, value) || this.TrySetField(binder.Name, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads a type by name and creates a wrapped instance of it
        /// </summary>
        /// <param name="assembly">
        /// The assembly to load 
        /// </param>
        /// <param name="typeName">
        /// The type to load 
        /// </param>
        /// <param name="args">
        /// The arguments to the ctor 
        /// </param>
        /// <returns>
        /// The load. 
        /// </returns>
        internal static dynamic Load(Assembly assembly, string typeName, params object[] args)
        {
            // Find the type by name
            var type = assembly.GetTypes().First(item => item.Name == typeName);

            // Get the constructor
            var ctor = type.GetConstructor(Flags, null, GetTypes(args), null);

            return ctor != null ? new ReflectionObject(ctor.Invoke(args)) : null;
        }

        /// <summary>
        /// The get field.
        /// </summary>
        /// <param name="type">
        /// The type. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <returns>
        /// The field info 
        /// </returns>
        private static FieldInfo GetField(Type type, string name)
        {
            var fld = type.GetField(name, Flags);

            return fld ?? (type.BaseType != null ? GetField(type.BaseType, name) : null);
        }

        /// <summary>
        /// The get method.
        /// </summary>
        /// <param name="type">
        /// The type. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <returns>
        /// The method info 
        /// </returns>
        private static MethodInfo GetMethod(Type type, string name, Type[] args)
        {
            var method = type.GetMethod(name, Flags, null, args, null);

            return method ?? (type.BaseType != null ? GetMethod(type.BaseType, name, args) : null);
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="type">
        /// The type. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <returns>
        /// The property info 
        /// </returns>
        private static PropertyInfo GetProperty(Type type, string name)
        {
            var prop = type.GetProperty(name, Flags);

            return prop ?? (type.BaseType != null ? GetProperty(type.BaseType, name) : null);
        }

        /// <summary>
        /// The get types.
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <returns>
        /// The types 
        /// </returns>
        private static Type[] GetTypes(IEnumerable<object> args)
        {
            return args.Select((o, i) => o.GetType()).ToArray();
        }

        /// <summary>
        /// The try get field.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// true if the field was returned 
        /// </returns>
        private bool TryGetField(string name, out object result)
        {
            var fld = GetField(this.Inner.GetType(), name);

            if (fld != null)
            {
                result = fld.GetValue(this.Inner);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// The try get property.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// true if the property was returned 
        /// </returns>
        private bool TryGetProperty(string name, out object result)
        {
            var prop = GetProperty(this.Inner.GetType(), name);
            if (prop != null)
            {
                result = prop.GetValue(this.Inner, null);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// The try set field.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <returns>
        /// true if the field was set 
        /// </returns>
        private bool TrySetField(string name, object value)
        {
            var fld = GetField(this.Inner.GetType(), name);

            if (fld != null)
            {
                fld.SetValue(this.Inner, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The try set property.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <returns>
        /// true if the property was set 
        /// </returns>
        private bool TrySetProperty(string name, object value)
        {
            var prop = GetProperty(this.Inner.GetType(), name);

            if (prop != null)
            {
                prop.SetValue(this.Inner, value, null);
                return true;
            }

            return false;
        }

        #endregion
    }
}
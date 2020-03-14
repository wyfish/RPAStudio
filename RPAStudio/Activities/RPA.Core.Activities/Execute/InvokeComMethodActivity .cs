using Plugins.Shared.Library.Editors;
using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace RPA.Core.Activities.Execute
{
    [Designer(typeof(InvokeComMethod))]
    public sealed class InvokeComMethodActivity : CodeActivity<object>
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Invoke Com Method"; } }

        [Localize.LocalizedCategory("Category11")] //公共
        [Localize.LocalizedDisplayName("key252")] //错误执行
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        //[Category("输出")]
        //[Browsable(true)]
        //[Description("表示被调用方法的结果。")]
        //public OutArgument<object> Result { get; set; }

        [Category("Target")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description116")] //指定用于控制绑定的标志以及通过反射进行成员和类型搜索的方式。
        public BindingFlags BindingFlags { get; set; }

        [Category("Target")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description117")] //COM对象的类ID。
        public InArgument<string> CLSID { get; set; }

        [Category("Target")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description118")] //要调用的方法的名称。
        public InArgument<string> MethodName { get; set; }

        [Category("Target")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description119")] //要调用的方法的对象类型的ProgID。
        public InArgument<string> ProgID { get; set; }

        [Category("Target")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description120")] //要调用的方法的参数集合。
        public Dictionary<string, Argument> Arguments { get; private set; } = new Dictionary<string, Argument>();

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Execute/InvokeComMethod.png";
            }
        }

        public InvokeComMethodActivity()
        {
            this.BindingFlags = BindingFlags.InvokeMethod;
        }
        static InvokeComMethodActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(InvokeComMethodActivity), "Arguments", new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
        protected override object Execute(CodeActivityContext context)
        {
            object result;
            try
            {
                Type type;
                if (!string.IsNullOrEmpty(this.CLSID.Get(context)))
                {
                    type = Type.GetTypeFromCLSID(new Guid(this.CLSID.Get(context)));
                }
                else
                {
                    type = Type.GetTypeFromProgID(this.ProgID.Get(context));
                }
                if (type == null)
                {
                   
                }
               
                ArrayList arrayList = new ArrayList();
                foreach (KeyValuePair<string, Argument> current in this.Arguments)
                {
                    arrayList.Add(current.Value.Get(context));
                }
                object obj = Activator.CreateInstance(type);
                result = obj.GetType().InvokeMember(this.MethodName.Get(context), this.BindingFlags, null, obj, arrayList.ToArray());
            }
            catch
            {
                if (!this.ContinueOnError.Get(context))
                {
                    throw;
                }
                result = null;
            }
            return result;
        }

    }
}

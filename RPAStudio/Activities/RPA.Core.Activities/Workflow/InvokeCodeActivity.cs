using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Activities.Expressions;
using System.Activities.Presentation.Metadata;
using Plugins.Shared.Library.Editors;
using System.Activities.Presentation.PropertyEditing;
using Plugins.Shared.Library.Extensions;

namespace RPA.Core.Activities.Workflow
{
    [Designer(typeof(InvokeCodeDesigner))]
    public sealed class InvokeCodeActivity : CodeActivity
    {
        private static Dictionary<string, CompilerRunner> codeRunnerCache = new Dictionary<string, CompilerRunner>(25);

        private static object codeRunnerCacheLock = new object();

        [Browsable(false)]
        public string CompilationError
        {
            get;
            set;
        }

        [RequiredArgument]
        [Category("Input")]
        public string Code
        {
            get;
            set;
        }

        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError
        {
            get;
            set;
        }

        [Category("Input")]
        [Browsable(true)]
        public Dictionary<string, Argument> Arguments
        {
            get;
            set;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (!string.IsNullOrWhiteSpace(CompilationError))
            {
                metadata.AddValidationError(CompilationError);
            }
        }

        public InvokeCodeActivity()
        {
            Arguments = new Dictionary<string, Argument>();

            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(InvokeCodeActivity), "Arguments", new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            builder.AddCustomAttributes(typeof(InvokeCodeActivity), "Code", new EditorAttribute(typeof(VBNetCodeEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public static string GetImports(IEnumerable<string> imports)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string import in imports)
            {
                if (!string.IsNullOrWhiteSpace(import))
                {
                    stringBuilder.AppendLine($"Imports {import}");
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetVbNetArguments(List<Tuple<string, Type, ArgumentDirection>> inArgs)
        {
            string text = "";
            foreach (Tuple<string, Type, ArgumentDirection> inArg in inArgs)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text += ", ";
                }
                string arg = "";
                switch (inArg.Item3)
                {
                    case ArgumentDirection.In:
                        arg = "ByVal";
                        break;
                    case ArgumentDirection.Out:
                    case ArgumentDirection.InOut:
                        arg = "ByRef";
                        break;
                }
                text += $"{arg} {inArg.Item1} As {GetVbNetTypeName(inArg.Item2)}";
            }
            return text;
        }

        private static CompilerRunner GetCompilerRunner(string userCode, List<Tuple<string, Type, ArgumentDirection>> args, string imps)
        {
            CompilerRunner value = null;
            string vbFunctionCode = GetVbFunctionCode(userCode, args);
            lock (codeRunnerCacheLock)
            {
                if (codeRunnerCache.TryGetValue(vbFunctionCode, out value))
                {
                    return value;
                }
                Tuple<string, string, int> vbModuleCode = GetVbModuleCode(vbFunctionCode, imps);
                value = new CompilerRunner(vbModuleCode.Item1, vbModuleCode.Item2, "Run", vbModuleCode.Item3);
                codeRunnerCache.Add(vbFunctionCode, value);
                return value;
            }
        }

        public static CompilerRunner CreateCompilerRunner(string userCode, string imps, List<Tuple<string, Type, ArgumentDirection>> args)
        {
            Tuple<string, string, int> vbModuleCode = GetVbModuleCode(GetVbFunctionCode(userCode, args), imps);
            return new CompilerRunner(vbModuleCode.Item1, vbModuleCode.Item2, "Run", vbModuleCode.Item3, generateInMemory: false);
        }

        private IList<string> GetImports(Activity workflow)
        {
            return TextExpression.GetNamespacesForImplementation(workflow) ?? new string[0];
        }


        private Activity GetRootActivity(CodeActivityContext context)
        {
            var ext = context.GetExtension<WorkflowRuntime>();
            return ext?.GetRootActivity();
        }

        protected override void Execute(CodeActivityContext context)
        {
            IList<string> imports = GetImports(GetRootActivity(context));

            string code = Code;
            bool flag = ContinueOnError.Get(context);
            List<Tuple<string, Type, ArgumentDirection>> list = new List<Tuple<string, Type, ArgumentDirection>>(Arguments.Count);
            object[] array = new object[Arguments.Count];
            int num = 0;
            foreach (KeyValuePair<string, Argument> argument2 in Arguments)
            {
                list.Add(new Tuple<string, Type, ArgumentDirection>(argument2.Key, argument2.Value.ArgumentType, argument2.Value.Direction));
                array[num++] = argument2.Value.Get(context);
            }
            try
            {
                string imports2 = GetImports(imports);
                GetCompilerRunner(code, list, imports2).Run(array);
                int num3 = 0;
                foreach (Tuple<string, Type, ArgumentDirection> item in list)
                {
                    Argument argument = Arguments[item.Item1];
                    if (argument.Direction != 0)
                    {
                        argument.Set(context, array[num3]);
                    }
                    num3++;
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception )
            {
                if (!flag)
                {
                    throw;
                }
            }
        }


        private static Tuple<string, string, int> GetVbModuleCode(string funcCode, string imps)
        {
            int num = imps.Count((char c) => c == '\n');
            string text = GenerateRandomSufix();
            return new Tuple<string, string, int>($"Option Explicit\r\nOption Strict\r\n{imps}Module RPACodeRunner_{text}\r\n{funcCode}\r\nEnd Module", "RPACodeRunner_" + text, 4 + num);
        }

        private static string GetVbFunctionCode(string userCode, List<Tuple<string, Type, ArgumentDirection>> inArgs)
        {
            string vbNetArguments = GetVbNetArguments(inArgs);
            return $"Sub Run({vbNetArguments})\r\n{userCode}\r\nEnd Sub";
        }

        private static string GenerateRandomSufix()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private static string GetVbNetTypeName(Type t)
        {
            if (!t.IsGenericType)
            {
                return t.FullName.Replace("[]", "()");
            }
            if (t.IsNested && t.DeclaringType.IsGenericType)
            {
                throw new NotImplementedException();
            }
            string str = t.FullName.Substring(0, t.FullName.IndexOf('`')) + "(Of ";
            int num = 0;
            Type[] genericArguments = t.GetGenericArguments();
            foreach (Type t2 in genericArguments)
            {
                if (num > 0)
                {
                    str += ", ";
                }
                str += GetVbNetTypeName(t2);
                num++;
            }
            return str + ")";
        }

        public void SetSuccessfulCompilation()
        {
            CompilationError = "";
        }

        public void SetCompilationError(string errorMessage)
        {
            CompilationError = errorMessage;
        }
    }

}

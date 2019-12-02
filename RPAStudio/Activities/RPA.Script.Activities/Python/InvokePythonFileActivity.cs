using System.Collections.Generic;
using System.Activities;
using System.ComponentModel;
using System.Activities.Presentation.Metadata;
using Plugins.Shared.Library.Editors;
using System.Activities.Presentation.PropertyEditing;
using Plugins.Shared.Library.Librarys;
using System.Linq;
using Python.Runtime;
using Plugins.Shared.Library;
using System;

namespace RPA.Script.Activities.Python
{
    [Designer(typeof(InvokePythonFileDesigner))]
    public sealed class InvokePythonFileActivity : CodeActivity
    {
        private static string PythonHome;

        [Category("Input")]
        [RequiredArgument]
        [Description("Python脚本文件路径，必须用双引号括起来")]
        public InArgument<string> PythonFilePath { get; set; }

        [Category("Input")]
        [Description("Python脚本文件执行时的工作目录，默认为当前项目目录")]
        public InArgument<string> PythonWorkingDirectory { get; set; }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
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
        } = new Dictionary<string, Argument>();

        static InvokePythonFileActivity()
        {
            if (!PythonEngine.IsInitialized)
            {
                PythonHome = AppDomain.CurrentDomain.BaseDirectory + @"Python";
                Environment.SetEnvironmentVariable("PATH", PythonHome + ";" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine), EnvironmentVariableTarget.Process);
                PythonEngine.PythonHome = PythonHome;

                PythonEngine.Initialize();
            }
        }

        public InvokePythonFileActivity()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(InvokePythonFileActivity), "Arguments", new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void Execute(CodeActivityContext context)
        {
            bool flag = ContinueOnError.Get(context);

            IntPtr ts = IntPtr.Zero;

            try
            {
                ts = PythonEngine.BeginAllowThreads();
                using (Py.GIL())
                {
                    using (var ps = Py.CreateScope())
                    {
                        //入参设置
                        Dictionary<string, object> inArguments = (from argument in Arguments
                                                                  where argument.Value.Direction != ArgumentDirection.Out
                                                                  select argument).ToDictionary((KeyValuePair<string, Argument> argument) => argument.Key, (KeyValuePair<string, Argument> argument) => argument.Value.Get(context));
                        foreach (var arg in inArguments)
                        {
                            ps.Set(arg.Key, arg.Value);
                        }

                        using (var scope = ps.NewScope())
                        {
                            PyObject pyObj = PythonPrintRedirectObject.Instance.ToPython();
                            dynamic sys = Py.Import("sys");
                            sys.stdout = pyObj;

                            dynamic os = Py.Import("os");
                            string workDir = PythonWorkingDirectory.Get(context);
                            if (string.IsNullOrEmpty(workDir))
                            {
                                os.chdir(SharedObject.Instance.ProjectPath);//设置python运行时的默认当前目录为项目目录
                            }
                            else
                            {
                                sys.path.append(workDir);
                               // os.chdir(workDir);
                            }

                            //由于是32 bit的python，耗内存操作可能会报错(如aircv.find_sift(imsrc, imsch)会内存分配报错)
                            string pythonFilePath = PythonFilePath.Get(context);
                            scope.Exec(System.IO.File.ReadAllText(pythonFilePath));

                            //出参设置
                            Dictionary<string, object> outArguments = (from argument in Arguments
                                                                       where argument.Value.Direction != ArgumentDirection.In
                                                                       select argument).ToDictionary((KeyValuePair<string, Argument> argument) => argument.Key, (KeyValuePair<string, Argument> argument) => argument.Value.Get(context));

                            foreach (var arg in outArguments)
                            {
                                Type argumentType = Arguments[arg.Key].ArgumentType;

                                var argVal = scope.Get(arg.Key).AsManagedObject(argumentType);

                                if (arg.Value != null && !argumentType.IsAssignableFrom(argVal.GetType()))
                                {
                                    Arguments[arg.Key].Set(context, JsonParser.DeserializeArgument(argVal, argumentType));
                                }
                                else
                                {
                                    Arguments[arg.Key].Set(context, argVal);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (!flag)
                {
                    throw;
                }
            }
            finally
            {
                if (ts != IntPtr.Zero)
                {
                    PythonEngine.EndAllowThreads(ts);
                }
            }
        }
    }



}

using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using Plugins.Shared.Library.Editors;
using System.Activities.Presentation;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Collections;

namespace RPA.Core.Activities.PowerShellActivity
{
    [Designer(typeof(PowerShellDesigner))]
    public sealed class ShellActivity<T> : CodeActivity
    {
        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Localize.LocalizedDescription("Description112")] //要执行的PowerShell命令。 //The PowerShell command to execute. //実行するPowerShellコマンド。
        public InArgument<string> CommandText { get; set; }

        [Category("Input")]
        [Localize.LocalizedDescription("Description113")] //传递给用于执行命令的管道的编写器的PSObjects的集合。可以是另一个InvokePowerShell活动的输出。 //A collection of PSObjects passed to the writer of the pipeline used to execute the command.  Can be the output of another InvokePowerShell activity. //コマンドの実行に使用されるパイプラインのライターに渡されるPSObjectのコレクション。 別のInvokePowerShellアクティビティの出力にすることができます。
        public InArgument<Collection<PSObject>> Input { get; set; }

        [Category("Input")]
        [Browsable(true)]
        [DisplayNameAttribute("Parameters")]
        [Description("")]
        public Dictionary<string, InArgument> parameters { get; private set; } = new Dictionary<string, InArgument>();

        bool _isScript = false;
        [Category("Misc")]
        public bool IsScript
        {
            get
            {
                return _isScript;
            }
            set
            {
                _isScript = value;
            }
        }

        [Category("Misc")]
        [Browsable(true)]
        public Dictionary<string, Argument> PowerShellVariables { get; private set; } = new Dictionary<string, Argument>();

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDescription("Description114")] //命令执行时返回的一组类型参数objets。可以用于管道几个InvokePowerShell活动。 //A set of type parameters objets returned when the command is executed.  Can be used to pipe several InvokePowerShell activities. //コマンドの実行時に返される一連の型パラメーターオブジェクト。 いくつかのInvokePowerShellアクティビティのパイプに使用できます。
        public OutArgument<Collection<T>> Output { get; set; }


        static ShellActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            Type attrType = Type.GetType("System.Activities.Presentation.FeatureAttribute, System.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Type argType = Type.GetType("System.Activities.Presentation.UpdatableGenericArgumentsFeature, System.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            Type psType = Type.GetType("");
            builder.AddCustomAttributes(typeof(ShellActivity<>), new Attribute[] { Activator.CreateInstance(attrType, new object[] { argType ,}) as Attribute });
            builder.AddCustomAttributes(typeof(ShellActivity<>), new DefaultTypeArgumentAttribute(typeof(object)));
            builder.AddCustomAttributes(typeof(ShellActivity<>), "parameters", new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            builder.AddCustomAttributes(typeof(ShellActivity<>), "PowerShellVariables", new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/PowerShell/powershell.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            Runspace runspace = null;
            Pipeline pipeline = null;
            try
            {
                runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();
                pipeline = runspace.CreatePipeline();
                string _commandText = CommandText.Get(context);
                Command cmd = new Command(_commandText, this.IsScript);
                if (this.parameters != null)
                {
                    foreach (KeyValuePair<string, InArgument> parameter in this.parameters)
                    {
                        if (parameter.Value.Expression != null)
                        {
                            cmd.Parameters.Add(parameter.Key, parameter.Value.Get(context));
                        }
                        else
                        {
                            cmd.Parameters.Add(parameter.Key, true);
                        }
                    }
                }
                if (this.PowerShellVariables != null)
                {
                    foreach(KeyValuePair<string, Argument> powerShellVariable in this.PowerShellVariables)
                    {
                        if ((powerShellVariable.Value.Direction == ArgumentDirection.In) || (powerShellVariable.Value.Direction == ArgumentDirection.InOut))
                        {
                            runspace.SessionStateProxy.SetVariable(powerShellVariable.Key, powerShellVariable.Value.Get(context));
                        }
                    }
                }   

                pipeline.Commands.Add(cmd);
                IEnumerable pipelineInput = this.Input.Get(context);
                if (pipelineInput != null)
                {
                    foreach (object inputItem in pipelineInput)
                    {
                        pipeline.Input.Write(inputItem);
                    }
                }
                pipeline.Input.Close();
                Collection<T> _result= new Collection<T>();
                T result_;
                foreach (PSObject result in pipeline.Invoke())
                {
                    result_ = (T)result.BaseObject;
                    _result.Add(result_);
                }
                Output.Set(context, _result);
                pipeline.Dispose();
                runspace.Close();
            }
            catch (Exception e)
            {
                if (runspace != null)
                {
                    runspace.Dispose();
                }

                if (pipeline != null)
                {
                    pipeline.Dispose();
                }
                if (!ContinueOnError.Get(context))
                {
                    throw new InvalidOperationException(e.Message, e);
                }
            }
        }
    }
}

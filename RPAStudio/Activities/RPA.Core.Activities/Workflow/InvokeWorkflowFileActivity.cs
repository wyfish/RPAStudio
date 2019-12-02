using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using Plugins.Shared.Library.Editors;
using System.Collections.Generic;
using Plugins.Shared.Library;
using System.Activities.XamlIntegration;
using System.Linq;
using Plugins.Shared.Library.Librarys;
using Plugins.Shared.Library.Extensions;
using System;

namespace RPA.Core.Activities.Workflow
{
    [Designer(typeof(InvokeWorkflowFileDesigner))]
    public sealed class InvokeWorkflowFileActivity : CodeActivity
    {
        private string ProjectPath { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("工作流文件路径，必须用双引号括起来")]
        public InArgument<string> WorkflowFilePath { get; set; }

        [Category("Input")]
        [Browsable(true)]
        public Dictionary<string, Argument> Arguments { get; private set; } = new Dictionary<string, Argument>();

        public InvokeWorkflowFileActivity()
        {
            ProjectPath = SharedObject.Instance.ProjectPath;

            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(InvokeWorkflowFileActivity), "Arguments", new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }


        public void SetWorkflowFilePath(string filePath)
        {
            if (filePath.StartsWith(SharedObject.Instance.ProjectPath, System.StringComparison.CurrentCultureIgnoreCase))
            {
                //如果在项目目录下，则使用相对路径保存
                filePath = Common.MakeRelativePath(SharedObject.Instance.ProjectPath, filePath);
            }

            WorkflowFilePath = filePath;
        }

        /// <summary>
        /// 创建并验证活动的参数、变量、子活动和活动委托的说明。
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            foreach (KeyValuePair<string, Argument> argument2 in Arguments)
            {
                Argument value = argument2.Value;
                RuntimeArgument argument = new RuntimeArgument(argument2.Key, value.ArgumentType, value.Direction);
                metadata.Bind(value, argument);
                metadata.AddArgument(argument);
            }

            base.CacheMetadata(metadata);
        }

        // 如果活动返回值，则从 CodeActivity<TResult>
        // 并从 Execute 方法返回该值。
        protected override void Execute(CodeActivityContext context)
        {
            // 获取 Text 输入参数的运行时值
            string workflowFilePath = context.GetValue(this.WorkflowFilePath);
            //如果workflowFilePath不是绝对路径，则转成绝对路径
            if(!System.IO.Path.IsPathRooted(workflowFilePath))
            {
                workflowFilePath = System.IO.Path.Combine(ProjectPath, workflowFilePath);
            }

            try
            {
                Dictionary<string, object> inArguments = (from argument in Arguments
                                                          where argument.Value.Direction != ArgumentDirection.Out
                                                          select argument).ToDictionary((KeyValuePair<string, Argument> argument) => argument.Key, (KeyValuePair<string, Argument> argument) => argument.Value.Get(context));

                Activity workflow = ActivityXamlServices.Load(workflowFilePath);

                var invoker = new WorkflowInvoker(workflow);

                invoker.Extensions.Add(new LogToOutputWindowTextWriter());

                if (workflow is DynamicActivity)
                {
                    var wr = new WorkflowRuntime();
                    wr.RootActivity = workflow;
                    invoker.Extensions.Add(wr);
                }

                var outputArguments = invoker.Invoke(inArguments);

                foreach (KeyValuePair<string, object> item in from argument in outputArguments
                                                              where Arguments.ContainsKey(argument.Key)
                                                              select argument)
                {
                    Type argumentType = Arguments[item.Key].ArgumentType;
                    if (item.Value != null && !argumentType.IsAssignableFrom(item.Value.GetType()))
                    {
                        Arguments[item.Key].Set(context, JsonParser.DeserializeArgument(item.Value, argumentType));
                    }
                    else
                    {
                        Arguments[item.Key].Set(context, item.Value);
                    }
                }

            }
            catch (System.Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, e.ToString());

                //继续抛出异常（考虑到执行工作流文件功能比较重要，需要强制提醒用户）
                throw;
            }
            
        }
    }
}

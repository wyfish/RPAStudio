using GongSolutions.Wpf.DragDrop;
using Plugins.Shared.Library;
using RPAStudio.Librarys;
using RPAStudio.ViewModel;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Windows;
//using WorkflowUtils;

namespace RPAStudio.DragDropHandler
{
    public class ProjectDragHandler: DefaultDragHandler
    {
        public override void StartDrag(IDragInfo dragInfo)
        {
            var item = dragInfo.SourceItem as ProjectTreeItem;

            //让xaml文件支持拖拽生成执行工作流文件的组件
            if(item.IsXaml && ViewModelLocator.Instance.Dock.ActiveDocument != null)
            {
                var designer = ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance;

                var type = Type.GetType("RPA.Core.Activities.Workflow.InvokeWorkflowFileActivity,RPA.Core.Activities");

                dynamic dragActivity = Activator.CreateInstance(type);
                dragActivity.SetWorkflowFilePath(item.Path);


                //通过类型获取对应的显示名并进行设置

                var activity_item = ViewModelLocator.Instance.Activities.GetActivityTreeItemByType(type);
                if (activity_item != null)
                {
                    dragActivity.DisplayName = activity_item.Name;
                }

                Activity resultActivity = dragActivity;
                if(Common.IsWorkflowDesignerEmpty(designer))
                {
                    resultActivity = Common.ProcessAutoSurroundWithSequence(dragActivity);
                }
               
                if (resultActivity != null)
                {
                    //动态生成组件，以便解决拖动到flowchart报错的问题
                    var dag = new DynamicActivityGenerator("_DynamicActivityGenerator_" + System.Guid.NewGuid().ToString());
                    var t = dag.AppendSubWorkflowTemplate(System.Guid.NewGuid().ToString(), Common.ToXaml(resultActivity));
                    try
                    {
                        dag.Save();
                    }
                    catch (Exception )
                    {
                    }

                    DataObject data = new DataObject(System.Activities.Presentation.DragDropHelper.WorkflowItemTypeNameFormat, t.AssemblyQualifiedName);

                    dragInfo.DataObject = data;
                }
            }

            //让Python文件支持拖拽生成执行Python脚本文件组件的功能
            if (item.IsPython && ViewModelLocator.Instance.Dock.ActiveDocument != null)
            {
                var designer = ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance;

                var type = Type.GetType("RPA.Script.Activities.Python.InvokePythonFileActivity,RPA.Script.Activities");

                dynamic dragActivity = Activator.CreateInstance(type);

                if (item.Path.StartsWith(SharedObject.Instance.ProjectPath, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    //如果在项目目录下，则使用相对路径保存
                    dragActivity.PythonFilePath = Common.MakeRelativePath(SharedObject.Instance.ProjectPath, item.Path);
                }

                //通过类型获取对应的显示名并进行设置

                var activity_item = ViewModelLocator.Instance.Activities.GetActivityTreeItemByType(type);
                if (activity_item != null)
                {
                    dragActivity.DisplayName = activity_item.Name;
                }

                Activity resultActivity = dragActivity;
                if (Common.IsWorkflowDesignerEmpty(designer))
                {
                    resultActivity = Common.ProcessAutoSurroundWithSequence(dragActivity);
                }

                if (resultActivity != null)
                {
                    //动态生成组件，以便解决拖动到flowchart报错的问题
                    var dag = new DynamicActivityGenerator("_DynamicActivityGenerator_" + System.Guid.NewGuid().ToString());
                    var t = dag.AppendSubWorkflowTemplate(System.Guid.NewGuid().ToString(), Common.ToXaml(resultActivity));
                    try
                    {
                        dag.Save();
                    }
                    catch (Exception )
                    {
                    }

                    DataObject data = new DataObject(System.Activities.Presentation.DragDropHelper.WorkflowItemTypeNameFormat, t.AssemblyQualifiedName);

                    dragInfo.DataObject = data;
                }
            }


            //只允许文件夹或文件拖动
            if (!item.IsDirectory && !item.IsFile)
            {
                return;
            }

            base.StartDrag(dragInfo);
        }


  


    }

   


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPAStudio.ViewModel;
using System.Activities;
using System.Activities.Presentation.View;
using System.Activities.Debugger;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using System.Reflection;
using RPAStudio.DataManager;
using RPAStudio.Librarys;
using log4net;

namespace RPAStudio.Executor
{
    class BreakpointsManager
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService(WorkflowDesigner workflowDesigner)
        {
            var debugView = workflowDesigner.DebugManagerView;//不能保存m_workflowDesigner.DebugManagerView在以后使用，会是旧数据
            var modelService = workflowDesigner.Context.Services.GetService<ModelService>();

            var nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            var debuggerServiceType = typeof(DebuggerService);
            var ensureMappingMethodName = "EnsureSourceLocationUpdated";
            var mappingFieldName = "instanceToSourceLocationMapping";
            var ensureMappingMethod = debuggerServiceType.GetMethod(ensureMappingMethodName, nonPublicInstance);
            var mappingField = debuggerServiceType.GetField(mappingFieldName, nonPublicInstance);

            if (ensureMappingMethod == null)
                return new Dictionary<object, SourceLocation>();
            if (mappingField == null)
                return new Dictionary<object, SourceLocation>();

            var rootActivity = (modelService.Root.GetCurrentValue() as ActivityBuilder).Implementation as Activity;
            if (rootActivity != null)
                WorkflowInspectionServices.CacheMetadata(rootActivity);

            ensureMappingMethod.Invoke(debugView, new object[0]);
            var mapping = (Dictionary<object, SourceLocation>)mappingField.GetValue(debugView);

            //TODO WJF 在空白项目中拖动一个组件时，断点设置失败,mapping为空
            return mapping;
        }

        private static Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    if (map.ContainsKey(wfElement.Id))
                    {
                        map.Remove(wfElement.Id);
                    }
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;
        }

        public static void SetBreakpoint(DocumentViewModel activeDocument, string activityId, bool IsEnabled)
        {
            try
            {
                var workflowDesigner = activeDocument.WorkflowDesignerInstance;

                var wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService(workflowDesigner);
                var activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);
                if (activityIdToWfElementMap.ContainsKey(activityId))
                {
                    SourceLocation srcLoc = wfElementToSourceLocationMap[activityIdToWfElementMap[activityId]];

                    if (IsEnabled)
                    {
                        workflowDesigner.DebugManagerView.InsertBreakpoint(srcLoc, BreakpointTypes.Enabled | BreakpointTypes.Bounded);
                    }
                    else
                    {
                        workflowDesigner.DebugManagerView.DeleteBreakpoint(srcLoc);
                    }
                }
                else
                {
                    //找不到断点位置，说明文件有修改，则该断点信息删除
                    ProjectSettingsDataManager.Instance.m_projectBreakpointsDataManager.RemoveBreakpointLocation(activeDocument.RelativeXamlPath, activityId);
                }
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
            }
        }

        public static void ToggleBreakpoint(DocumentViewModel activeDocument)
        {
            try
            {
                var workflowDesigner = activeDocument.WorkflowDesignerInstance;
                Activity activity = workflowDesigner.Context.Items.GetValue<Selection>().
                        PrimarySelection.GetCurrentValue() as Activity;

                if (activity != null)
                {
                    //切换该活动的断点
                    var wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService(workflowDesigner);
                    var activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);
                    if (activityIdToWfElementMap.Count == 0)
                    {
                        return;
                    }

                    SourceLocation srcLoc = wfElementToSourceLocationMap[activityIdToWfElementMap[activity.Id]];
                    //TODO WJF srcLoc在只有一个组件时为null，需要特殊处理下

                    bool bInsertBreakpoint = false;
                    var breakpointLocations = workflowDesigner.DebugManagerView.GetBreakpointLocations();
                    if (breakpointLocations.ContainsKey(srcLoc))
                    {
                        var types = breakpointLocations[srcLoc];
                        if (types != (BreakpointTypes.Enabled | BreakpointTypes.Bounded))
                        {
                            bInsertBreakpoint = true;
                        }
                    }
                    else
                    {
                        bInsertBreakpoint = true;
                    }

                    if (bInsertBreakpoint)
                    {
                        workflowDesigner.DebugManagerView.InsertBreakpoint(srcLoc, BreakpointTypes.Enabled | BreakpointTypes.Bounded);
                        ProjectSettingsDataManager.Instance.m_projectBreakpointsDataManager.AddBreakpointLocation(activeDocument.RelativeXamlPath, activity.Id, true);
                    }
                    else
                    {
                        workflowDesigner.DebugManagerView.DeleteBreakpoint(srcLoc);
                        ProjectSettingsDataManager.Instance.m_projectBreakpointsDataManager.RemoveBreakpointLocation(activeDocument.RelativeXamlPath, activity.Id);
                    }

                }
            }
            catch (Exception err)
            {
                //特殊情况时触发，目前在flowchart中如果不连接start node,也会报错
                Logger.Debug(err, logger);
            }
            
        }

        public static void RemoveAllBreakpoints(DocumentViewModel activeDocument)
        {
            var workflowDesigner = activeDocument.WorkflowDesignerInstance;
            workflowDesigner.DebugManagerView.ResetBreakpoints();

            ProjectSettingsDataManager.Instance.m_projectBreakpointsDataManager.RemoveAllBreakpointsLocation(activeDocument.RelativeXamlPath);
        }
    }
}

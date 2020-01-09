using System;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Presentation;
using System.Activities.Tracking;
using System.Activities.Validation;
using System.Activities.XamlIntegration;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Plugins.Shared.Library.Extensions;
using Plugins.Shared.Library;
using RPAStudio.Librarys;
using RPAStudio.Localization;
using RPAStudio.ViewModel;

namespace RPAStudio.Executor
{
    public class DebuggerManager
    {
        private WorkflowApplication m_app;
        public WorkflowDesigner m_workflowDesigner { get; set; }
        public string m_xamlPath { get; set; }

        public Dictionary<object, SourceLocation> m_wfElementToSourceLocationMap;
        private Dictionary<string, Activity> m_activityIdToWfElementMap;
       
        public Window m_mainView;
        private VisualTrackingParticipant m_simTracker;
        public MainViewModel m_mainViewModel;

        public DebuggerManager(MainViewModel mainViewModel, DocumentViewModel documentViewModel)
        {
            m_mainViewModel = mainViewModel;
            m_mainView = mainViewModel.m_view;
            m_workflowDesigner = documentViewModel.WorkflowDesignerInstance;
            m_xamlPath = documentViewModel.XamlPath;
        }

        public int m_speed_ms { get; set; }

        public enOperate m_nextOperate { get; set; }

        public enSpeed m_speedType { get; set; }

        public enum enSpeed
        {
            Off,//关闭
            One,//1x
            Two,//2x
            Three,//3x
            Four//4x
        }

        public enum enOperate
        {
            Null,//无操作
            StepInto,//步入
            StepOver,//步过
            Continue,//继续
            Break,//中断
            Stop,//停止
        }

        public void SetSpeed(enSpeed speedType)
        {
            m_speedType = speedType;
            switch (speedType)
            {
                case enSpeed.Off:
                    m_speed_ms = 0;
                    break;
                case enSpeed.One:
                    m_speed_ms = 2000;
                    break;
                case enSpeed.Two:
                    m_speed_ms = 1000;
                    break;
                case enSpeed.Three:
                    m_speed_ms = 500;
                    break;
                case enSpeed.Four:
                    m_speed_ms = 250;
                    break;
                default:
                    m_speed_ms = 0;
                    break;
            }
        }


        public void SetNextOperate(enOperate operate)
        {
            m_nextOperate = operate;
        }


        private VisualTrackingParticipant generateTracker()
        {
            const String all = "*";

            VisualTrackingParticipant simTracker = new VisualTrackingParticipant(this)
            {
                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries =
                        {
                         new CustomTrackingQuery()
                            {
                                Name = all,
                                ActivityName = all
                            },
                            new WorkflowInstanceQuery()
                            {
                                // Limit workflow instance tracking records for started and completed workflow states
                                States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
                            },

                             new ActivityStateQuery()
                            {
                                // Subscribe for track records from all activities for all states
                                ActivityName = all,
                                States = { all },

                                // Extract workflow variables and arguments as a part of the activity tracking record
                                // VariableName = "*" allows for extraction of all variables in the scope
                                // of the activity
                                Variables =
                                {
                                     all
                                },

                                Arguments =
                                {
                                    all
                                },
                            },

                             new ActivityScheduledQuery()
                            {
                                ActivityName = all,
                                ChildActivityName = all
                            },
                        }
                }
            };

            trackerVarsAdd(simTracker);

            simTracker.m_activityIdToWorkflowElementMap = m_activityIdToWfElementMap;

            return simTracker;
        }


        /// <summary>
        ///提前填充工作流用到的变量，以便在全局作用域里监视
        /// </summary>
        /// <param name="simTracker"></param>
        private void trackerVarsAdd(VisualTrackingParticipant simTracker)
        {
            List<string> varNameLsit = new List<string>();

            ModelService modelService = m_workflowDesigner.Context.Services.GetService<ModelService>();

            IEnumerable<ModelItem> flowcharts = modelService.Find(modelService.Root, typeof(Flowchart));
            IEnumerable<ModelItem> sequences = modelService.Find(modelService.Root, typeof(Sequence));

            foreach (var modelItem in flowcharts)
            {
                foreach (var varItem in modelItem.Properties["Variables"].Collection)
                {
                    var varName = varItem.Properties["Name"].ComputedValue as string;
                    varNameLsit.Add(varName);
                }
            }

            foreach (var modelItem in sequences)
            {
                foreach (var varItem in modelItem.Properties["Variables"].Collection)
                {
                    var varName = varItem.Properties["Name"].ComputedValue as string;
                    varNameLsit.Add(varName);
                }
            }

            if (varNameLsit.Count > 0)
            {
                foreach (var item in simTracker.TrackingProfile.Queries)
                {
                    if (item is ActivityStateQuery)
                    {
                        var query = item as ActivityStateQuery;

                        foreach (var name in varNameLsit)
                        {
                            query.Variables.Add(name);
                        }
                        break;
                    }
                }
            }

        }

        Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService()
        {
            var debugView = m_workflowDesigner.DebugManagerView;//不能保存m_workflowDesigner.DebugManagerView在以后使用，会是旧数据
            var modelService = m_workflowDesigner.Context.Services.GetService<ModelService>();

            var nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            var debuggerServiceType = typeof(DebuggerService);
            var ensureMappingMethodName = "EnsureSourceLocationUpdated";
            var mappingFieldName = "instanceToSourceLocationMapping";
            var ensureMappingMethod = debuggerServiceType.GetMethod(ensureMappingMethodName, nonPublicInstance);
            var mappingField = debuggerServiceType.GetField(mappingFieldName, nonPublicInstance);

            if (ensureMappingMethod == null)
                throw new MissingMethodException(debuggerServiceType.FullName, ensureMappingMethodName);
            if (mappingField == null)
                throw new MissingFieldException(debuggerServiceType.FullName, mappingFieldName);

            var rootActivity = modelService.Root.GetCurrentValue() as Activity;
            if (rootActivity != null)
                WorkflowInspectionServices.CacheMetadata(rootActivity);

            ensureMappingMethod.Invoke(debugView, new object[0]);
            var mapping = (Dictionary<object, SourceLocation>)mappingField.GetValue(debugView);

            return mapping;
        }

        private Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    if(map.ContainsKey(wfElement.Id))
                    {
                        map.Remove(wfElement.Id);
                    }
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;
        }

        /// <summary>
        /// 开始执行调试流程
        /// </summary>
        public void Debug()
        {
            //授权检测
            ViewModelLocator.Instance.SplashScreen.DoAuthorizationCheck();

            Activity workflow = ActivityXamlServices.Load(m_xamlPath);

            var result = ActivityValidationServices.Validate(workflow);
            if (result.Errors.Count == 0)
            {
                Messenger.Default.Send(this, "BeginRun");

                m_wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService();
                m_activityIdToWfElementMap = BuildActivityIdToWfElementMap(m_wfElementToSourceLocationMap);

                if (m_app != null)
                {
                    m_app.Terminate("");
                }

                m_app = new WorkflowApplication(workflow);
                m_app.OnUnhandledException = WorkflowApplicationOnUnhandledException;
                m_app.Completed = WorkflowApplicationExecutionCompleted;

                m_simTracker = generateTracker();
                m_app.Extensions.Add(m_simTracker);
                m_app.Extensions.Add(new LogToOutputWindowTextWriter());

                if (workflow is DynamicActivity)
                {
                    var wr = new WorkflowRuntime();
                    wr.RootActivity = workflow;
                    m_app.Extensions.Add(wr);
                }

                m_app.Run();
            }
            else
            {
                MessageBox.Show(App.Current.MainWindow, "工作流校验错误，请检查参数配置", ResxIF.GetString("ErrorText"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private UnhandledExceptionAction WorkflowApplicationOnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            var name = e.ExceptionSource.DisplayName;
            SharedObject.Instance.Output(SharedObject.enOutputType.Error, string.Format("{0} 执行时出现异常", name), e.UnhandledException.ToString());

            return UnhandledExceptionAction.Terminate;
        }

        private void WorkflowApplicationExecutionCompleted(WorkflowApplicationCompletedEventArgs obj)
        {
            if (obj.TerminationException != null)
            {
                if (!string.IsNullOrEmpty(obj.TerminationException.Message))
                {
                    Common.RunInUI(() =>
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "调试时执行错误", obj.TerminationException.ToString());
                        MessageBox.Show(App.Current.MainWindow, obj.TerminationException.Message, "调试时执行错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }

            Messenger.Default.Send(this, "EndRun");
            m_mainView.Dispatcher.Invoke(DispatcherPriority.Render
                        , (Action)(() =>
            {
                m_workflowDesigner.DebugManagerView.CurrentLocation = new SourceLocation(m_xamlPath, 1, 1, 1, 10);
            }));

        }

        public void Continue(enOperate operate = enOperate.Continue)
        {
            SetNextOperate(operate);
            m_simTracker.m_slowStepEvent.Set();
            m_simTracker.m_slowStepEvent.Reset();
        }

        public void Stop()
        {
            if (m_app != null)
            {
                try
                {
                    Continue(enOperate.Stop);//防止有事件卡住未往下走导致无法正常停止
                    m_app.Terminate("执行已取消", new TimeSpan(0, 0, 0, 30));
                }
                catch (Exception )
                {
                    MessageBox.Show(App.Current.MainWindow, "停止调试发生异常！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void Break()
        {
            //中断调试
            SetNextOperate(enOperate.Break);
        }
    }
}

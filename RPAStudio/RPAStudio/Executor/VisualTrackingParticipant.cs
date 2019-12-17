using GalaSoft.MvvmLight.Messaging;
using Plugins.Shared.Library;
using RPAStudio.Librarys;
using System;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Presentation.Debug;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

namespace RPAStudio.Executor
{
    public class VisualTrackingParticipant : TrackingParticipant
    {
        public ManualResetEvent m_slowStepEvent = new ManualResetEvent(false);
        private DebuggerManager m_debuggerManager;

        /// <summary>
        /// 记录上一次的中断时候的位置（可能是断点中断，手动中断，单步调试等位置，主要供单步步过时记录步过前的位置）
        /// </summary>
        public ActivityScheduledRecord m_lastDebugActivityScheduledRecord { get; set; }

        /// <summary>
        /// 主要用来记录中断时当前监视的变量信息
        /// </summary>
        public ActivityStateRecord m_lastActivityStateRecord { get; set; }

        public Dictionary<string, Activity> m_activityIdToWorkflowElementMap { get; set; }
        public Dictionary<string, string> m_activityIdParentMap = new Dictionary<string, string>();//child id => parent id

        public VisualTrackingParticipant(DebuggerManager debuggerManager)
        {
            m_debuggerManager = debuggerManager;
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            OnTrackingRecordReceived(record, timeout);
        }

        private bool MeetingBreakpoint(ActivityInfo child)
        {
            var breakpointLocations = m_debuggerManager.m_workflowDesigner.DebugManagerView.GetBreakpointLocations();
            SourceLocation srcLoc = m_debuggerManager.m_wfElementToSourceLocationMap[m_activityIdToWorkflowElementMap[child.Id]];
            if (breakpointLocations.ContainsKey(srcLoc))
            {
                var types = breakpointLocations[srcLoc];
                if (types == (BreakpointTypes.Enabled | BreakpointTypes.Bounded))
                {
                    return true;
                }
            }

            return false;
        }

        private bool activityCurrentOrParentIdExists(string id1, string id2)
        {
            if (id1 == id2)
            {
                return true;
            }

            if (m_activityIdParentMap.ContainsKey(id1))
            {
                return activityCurrentOrParentIdExists(m_activityIdParentMap[id1], id2);
            }

            return false;
        }

        private void doWaitThings(string id)
        {
            showCurrentLocation(id);
            showLocals();
        }

        private void processSlowStep(string id)
        {
            m_debuggerManager.SetSpeed(m_debuggerManager.m_mainViewModel.SlowStepSpeed);
            if (m_debuggerManager.m_speed_ms > 0)
            {
                doWaitThings(id);
                m_slowStepEvent.WaitOne(m_debuggerManager.m_speed_ms);
            }
        }

        private void processWait(string id)
        {
            bool isPaused = !m_slowStepEvent.WaitOne(0);

            Common.RunInUI(() =>
            {
                m_debuggerManager.m_mainViewModel.IsWorkflowDebuggingPaused = isPaused;
            });

            if (isPaused)
            {
                doWaitThings(id);
            }

            m_slowStepEvent.WaitOne();

            Common.RunInUI(() =>
            {
                m_debuggerManager.m_mainViewModel.IsWorkflowDebuggingPaused = false;
            });

            hideCurrentLocation();
        }

        private void showLocals()
        {
            //Locals监视窗口显示vars和args
            Messenger.Default.Send(m_lastActivityStateRecord, "ShowLocals");
        }

        private void hideCurrentLocation()
        {
            if (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Stop)
            {
                //停止时调用Dispatcher.Invoke会卡死，所以此处直接返回不往下走
                return;
            }

            m_debuggerManager.m_mainView.Dispatcher.Invoke(DispatcherPriority.Render
               , (Action)(() =>
               {
                   m_debuggerManager.m_workflowDesigner.DebugManagerView.CurrentLocation = null;

               }));
        }

        private void showCurrentLocation(string id)
        {
            if (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Stop)
            {
                //停止时调用Dispatcher.Invoke会卡死，所以此处直接返回不往下走
                return;
            }

            if (!m_activityIdToWorkflowElementMap.ContainsKey(id))
            {
                return;
            }

            SourceLocation srcLoc = m_debuggerManager.m_wfElementToSourceLocationMap[m_activityIdToWorkflowElementMap[id]];
            m_debuggerManager.m_mainView.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
                {
                    m_debuggerManager.m_workflowDesigner.DebugManagerView.CurrentLocation = srcLoc;
                }));
        }

        //On Tracing Record Received call the TrackingRecordReceived with the record received information from the TrackingParticipant. 
        //We also do not worry about Expressions' tracking data
        protected void OnTrackingRecordReceived(TrackingRecord record, TimeSpan timeout)
        {
            if (record is WorkflowInstanceRecord)
            {

            }
            else if (record is ActivityScheduledRecord)
            {
                var activityScheduledRecord = record as ActivityScheduledRecord;

                if (activityScheduledRecord.Child != null && m_activityIdToWorkflowElementMap.ContainsKey(activityScheduledRecord.Child.Id))
                {
                    m_activityIdParentMap[activityScheduledRecord.Child.Id] = activityScheduledRecord.Activity.Id;

                    if (MeetingBreakpoint(activityScheduledRecord.Child))
                    {
                        m_slowStepEvent.Reset();
                        processWait(activityScheduledRecord.Child.Id);
                        m_lastDebugActivityScheduledRecord = activityScheduledRecord;
                    }
                    else
                    {
                        if (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Null
                        || m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Continue
                        )
                        {
                            processSlowStep(activityScheduledRecord.Child.Id);
                        }
                        else if (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Break)
                        {
                            processWait(activityScheduledRecord.Child.Id);
                            m_lastDebugActivityScheduledRecord = activityScheduledRecord;
                        }
                        else if (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.StepInto)
                        {
                            processWait(activityScheduledRecord.Child.Id);
                            m_lastDebugActivityScheduledRecord = activityScheduledRecord;
                        }
                        else if (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.StepOver)
                        {
                            if (m_lastDebugActivityScheduledRecord != null)
                            {
                                if (activityCurrentOrParentIdExists(activityScheduledRecord.Activity.Id, m_lastDebugActivityScheduledRecord.Child.Id))
                                {
                                    Common.RunInUI(() =>
                                    {
                                        m_debuggerManager.m_mainViewModel.IsWorkflowDebuggingPaused = false;
                                    });
                                }
                                else
                                {
                                    processWait(activityScheduledRecord.Child.Id);
                                    m_lastDebugActivityScheduledRecord = activityScheduledRecord;
                                }
                            }
                            else
                            {
                                processWait(activityScheduledRecord.Child.Id);
                                m_lastDebugActivityScheduledRecord = activityScheduledRecord;
                            }

                        }
                    }
                }

                
            }
            else if (record is ActivityStateRecord)
            {
                var activityStateRecord = record as ActivityStateRecord;

                if (activityStateRecord.State == ActivityStates.Closed
                     && (activityStateRecord.Activity.TypeName == "System.Activities.Statements.Sequence"
                         || activityStateRecord.Activity.TypeName == "System.Activities.Statements.Flowchart"
                        )
                    && (m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Null
                        || m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.Continue
                        )
                    )
                {
                    processSlowStep(activityStateRecord.Activity.Id);
                }

                if (activityStateRecord.State == ActivityStates.Closed
                    && (
                    activityStateRecord.Activity.TypeName == "System.Activities.Statements.Sequence"
                    || activityStateRecord.Activity.TypeName == "System.Activities.Statements.Flowchart"
                    )
                    && (
                    m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.StepInto
                    || m_debuggerManager.m_nextOperate == DebuggerManager.enOperate.StepOver
                         )
                    )
                {
                    if (!activityCurrentOrParentIdExists(activityStateRecord.Activity.Id, m_lastDebugActivityScheduledRecord.Child.Id))
                    {
                        //此处需要判断下
                        processWait(activityStateRecord.Activity.Id);
                    }
                }

                if (m_debuggerManager.m_mainViewModel.IsLogActivities)
                {
                    Common.RunInUI(()=> {
                        var name = activityStateRecord.Activity.Name;

                        dynamic activityObj = new ReflectionObject(activityStateRecord.Activity);
                        var activity = activityObj.Activity;

                        if (activityStateRecord.Activity.TypeName == "System.Activities.DynamicActivity")
                        {
                            name = (activity as DynamicActivity).Name;
                        }
                        else
                        {
                            name = activity.DisplayName;
                        }

                        SharedObject.Instance.Output(SharedObject.enOutputType.Trace, string.Format("{0} {1}", name, activityStateRecord.State));
                    });
                }

                m_lastActivityStateRecord = activityStateRecord;
            }


        }

       
    }



}
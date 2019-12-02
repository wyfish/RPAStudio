using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace RPA.Core.Activities.EventActivity
{
    [Designer(typeof(MonitorActivityDesigner))]
    public class MonitorActivity : NativeActivity
    {
        public InArgument<bool> ContinueOnError
        {
            get;
            set;
        }
        private Bookmark _monitorBookmark;
        private BookmarkResumptionHelper _bookmarkResumptionHelper;
        protected Queue<KeyValuePair<ActivityAction<object>, object>> EventQueue = new Queue<KeyValuePair<ActivityAction<object>, object>>();

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        [Browsable(false)]
        public ActivityAction<object> Handler
        {
            get;
            set;
        }
        [Browsable(false)]
        public List<Activity> Triggers
        {
            get;
            private set;
        }

        public Activity<bool> RepeatForever
        {
            get;
            set;
        }

        public MonitorActivity()
        {
            this.Triggers = new List<Activity>();
           
            this.Handler = new ActivityAction<object>
            {
                Handler = new Sequence
                {
                    DisplayName = "EventHandler"
                },
                Argument = new DelegateInArgument<object>("args")
            };
            this.RepeatForever = true;
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Event/monitor.png";
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddChild(this.RepeatForever);
            foreach (Activity current in this.Triggers)
            {
                metadata.AddChild(current);
            }
            metadata.RequireExtension<BookmarkResumptionHelper>();
            base.CacheMetadata(metadata);
            metadata.AddDefaultExtensionProvider<BookmarkResumptionHelper>(() => new BookmarkResumptionHelper());
        }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                this.EventQueue.Clear();
                this._monitorBookmark = context.CreateBookmark(new BookmarkCallback(this.OnMonitorTrigger), BookmarkOptions.MultipleResume);
                this._bookmarkResumptionHelper = context.GetExtension<BookmarkResumptionHelper>();
                context.Properties.Add("MonitorBookmark", this._monitorBookmark, true);
                this.StartMonitor(context);
            }
            catch (Exception ex)
            {
                this.DisposeMonitor(context);
                this.HandleException(ex, this.ContinueOnError.Get(context));
            }
        }

        protected void StartMonitor(NativeActivityContext context)
        {
            foreach (Activity current in this.Triggers)
            {
                context.ScheduleActivity(current);
            }
        }

        protected override void Cancel(NativeActivityContext context)
        {
            this.DisposeMonitor(context);
            base.Cancel(context);
        }
        private void OnMonitorTrigger(NativeActivityContext context, Bookmark bookmark, object value)
        {
            this.EventQueue.Enqueue(new KeyValuePair<ActivityAction<object>, object>(this.Handler, value));
            if (this.EventQueue.Count == 1)
            {
                this.ExecuteEventHandler(context);
            }
        }
        protected void ExecuteEventHandler(NativeActivityContext context)
        {
            KeyValuePair<ActivityAction<object>, object> keyValuePair = this.EventQueue.Peek();
            if (keyValuePair.Key != null)
            {
                context.ScheduleAction<object>(keyValuePair.Key, keyValuePair.Value, new CompletionCallback(this.BodyCompleted), new FaultCallback(this.BodyFaulted));
            }
        }
        protected virtual void BodyCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
           // EventInfo eventInfo = this.EventQueue.Peek().Value as EventInfo;
            //if (eventInfo != null && eventInfo.ReplayEvent)
            //{
            //    eventInfo.Replay();
            //}
            if (this.RepeatForever == null)
            {
                this.RepeatForever = false;
            }
            context.ScheduleActivity<bool>(this.RepeatForever, new CompletionCallback<bool>(this.RepeatForeverCompleted), null);
        }
        protected void RepeatForeverCompleted(NativeActivityContext context, ActivityInstance completedInstance, bool result)
        {
            if (!result)
            {
                this.DisposeMonitor(context);
                return;
            }
            this.EventQueue.Dequeue();
            if (this.EventQueue.Count > 0)
            {
                this.ExecuteEventHandler(context);
            }
        }
        protected void BodyFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            this.DisposeMonitor(faultContext);
            this.HandleException(propagatedException, this.ContinueOnError.Get(faultContext));
            faultContext.CancelChildren();
            faultContext.HandleFault();
        }
        protected void DisposeMonitor(NativeActivityContext context)
        {
            context.RemoveBookmark(this._monitorBookmark);
            context.CancelChildren();
        }
        protected void HandleException(Exception ex, bool continueOnError)
        {
            if (continueOnError)
            {
                return;
            }
            throw ex;
        }
    }
}

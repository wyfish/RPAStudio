using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(CAAnchorDesigner))]
    public sealed class CAAnchor : NativeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Context Aware Anchor";
            }
        }

        [Category("选项")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Browsable(false)]
        public Activity AnchorBody { get; set; }

        [Browsable(false)]
        public Activity ActivityBody { get; set; }

        InArgument<Int32> _AnchorLeft = 0 , _AnchorRight = 0, _AnchorTop = 0, _AnchorBottom = 0;
        [Category("锚点边界框")]
        [DisplayName("Left")]
        [Browsable(true)]
        public InArgument<Int32> AnchorLeft { get { return _AnchorLeft; } set { _AnchorLeft = value; } }
        [Category("锚点边界框")]
        [DisplayName("Right")]
        [Browsable(true)]
        public InArgument<Int32> AnchorRight { get { return _AnchorRight; } set { _AnchorRight = value; } }
        [Category("锚点边界框")]
        [DisplayName("Top")]
        [Browsable(true)]
        public InArgument<Int32> AnchorTop { get { return _AnchorTop; } set { _AnchorTop = value; } }
        [Category("锚点边界框")]
        [DisplayName("Bottom")]
        [Browsable(true)]
        public InArgument<Int32> AnchorBottom { get { return _AnchorBottom; } set { _AnchorBottom = value; } }


        InArgument<Int32> _TargetLeft = 0, _TargetRight = 0, _TargetTop = 0, _TargetBottom = 0;
        [Category("目标边界框")]
        [DisplayName("Left")]
        [Browsable(true)]
        public InArgument<Int32> TargetLeft { get { return _TargetLeft; } set { _TargetLeft = value; } }
        [Category("目标边界框")]
        [DisplayName("Right")]
        [Browsable(true)]
        public InArgument<Int32> TargetRight { get { return _TargetRight; } set { _TargetRight = value; } }
        [Category("目标边界框")]
        [DisplayName("Top")]
        [Browsable(true)]
        public InArgument<Int32> TargetTop { get { return _TargetTop; } set { _TargetTop = value; } }
        [Category("目标边界框")]
        [DisplayName("Bottom")]
        [Browsable(true)]
        public InArgument<Int32> TargetBottom { get { return _TargetBottom; } set { _TargetBottom = value; } }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/AnchorBase.png";
            }
        }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            //metadata.AddChild(this.Body);
            //metadata.AddImplementationVariable(MessageVariable);
        }

        protected override void Execute(NativeActivityContext context)
        {
        }

        //void InternalExecute(NativeActivityContext context, ActivityInstance instance)
        //{
        //    //grab the index of the current Activity
        //    int currentActivityIndex = this.currentIndex.Get(context);
        //    if (currentActivityIndex == children.Count)
        //    {
        //        //if the currentActivityIndex is equal to the count of MySequence's Activities
        //        //MySequence is complete
        //        return;
        //    }

        //    if (this.onChildComplete == null)
        //    {
        //        //on completion of the current child, have the runtime call back on this method
        //        this.onChildComplete = new CompletionCallback(InternalExecute);
        //    }

        //    //grab the next Activity in MySequence.Activities and schedule it
        //    Activity nextChild = children[currentActivityIndex];
        //    context.ScheduleActivity(nextChild, this.onChildComplete);

        //    //increment the currentIndex
        //    this.currentIndex.Set(context, ++currentActivityIndex);
        //}

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //TODO
        }
    }
}

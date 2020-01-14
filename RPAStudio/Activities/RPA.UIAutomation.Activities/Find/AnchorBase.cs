using System;
using System.Activities;
using System.ComponentModel;


namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(AnchorBaseDesigner))]
    public sealed class AnchorBase : NativeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Anchor Base";
            }
        }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Browsable(false)]
        public Activity AnchorBody { get; set; }

        [Browsable(false)]
        public Activity ActivityBody { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName35")] //锚点位置 //Anchor position //アンカー位置
        public AnchorPositionEnums AnchorPosition { get; set; }

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

            //限制一组允许的活动类型
            //if(AnchorBody!=null)
            //{
            //    if (AnchorBody.GetType() != typeof(ClickActivity))
            //    {
            //        metadata.AddValidationError("Child activity is not of type WriteLine or Assign");
            //    }
            //}
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

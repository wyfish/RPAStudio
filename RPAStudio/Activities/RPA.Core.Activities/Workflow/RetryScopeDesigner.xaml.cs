using System;
using System.Activities;
using System.Activities.Presentation;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows;

namespace RPA.Core.Activities.Workflow
{
    // RetryScopeDesigner.xaml 的交互逻辑
    public partial class RetryScopeDesigner : ActivityDesigner, IComponentConnector
    {
        public RetryScopeDesigner()
        {
            this.InitializeComponent();
           // this.ConditionPresenter.AllowedItemType = typeof(Activity<bool>);
        }


        ////Token: 0x06000483 RID: 1155 RVA: 0x000117E8 File Offset: 0x0000F9E8
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    if (this.contentLoaded)
        //    {
        //        return;
        //    }
        //    this.contentLoaded = true;
        //    Uri resourceLocator = new Uri("/RPA.System.Activities.Workflow;component/designers/workflow/retrydesigner.xaml", UriKind.Relative);
        //    Application.LoadComponent(this, resourceLocator);
        //}

        ////Token: 0x06000484 RID: 1156 RVA: 0x00011818 File Offset: 0x0000FA18
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void IComponentConnector.Connect(int connectionId, object target)
        //{
        //    if (connectionId == 1)
        //    {
        //        //this.ConditionPresenter = (WorkflowItemPresenter)target;
        //        return;
        //    }
        //    this.contentLoaded = true;
        //}

        ////Token: 0x040000CB RID: 203
        //internal WorkflowItemPresenter ConditionPresenter;

        ////Token: 0x040000CC RID: 204
        //private bool contentLoaded;


    }
}

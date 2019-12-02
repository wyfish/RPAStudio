using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Activities.Statements;

namespace RPA.Core.Activities.DebugActivity
{
    [Designer(typeof(CommentOutDesigner))]
    public sealed class CommentOutActivity : CodeActivity
    {
        [Browsable(false)]
        public Activity Body
        {
            get;
            set;
        }

        public CommentOutActivity()
        {
            Body = new Sequence
            {
                DisplayName = "忽略的活动"
            };
        }

        protected override void Execute(CodeActivityContext context)
        {
        }
    }
}

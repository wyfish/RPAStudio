using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(ArchiveOutlookMailsDesigner))]
    public sealed class ArchiveOutlookMailsActivity : CodeActivity
    {
        [Category("Input")]
        [DisplayName("存档路径(.pst)")]
        [Description("Outlook数据文件的保存路径")]
        [RequiredArgument]
        public InArgument<string> PstFilePath
        {
            get;
            set;
        }

        [Category("Input")]
        [DisplayName("待存档账户")]
        [Description("Outlook账户名")]
        public InArgument<string> Account
        {
            get;
            set;
        }


        protected override void Execute(CodeActivityContext context)
        {
            var account = Account.Get(context);
            var pstPath = PstFilePath.Get(context);
            OutlookAPI.ArchiveMails(account, pstPath);
        }
    }
}

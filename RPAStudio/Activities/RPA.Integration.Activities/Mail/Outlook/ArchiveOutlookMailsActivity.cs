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
        [Localize.LocalizedDisplayName("DisplayName93")] //存档路径(.pst) //Archive path (.pst) //アーカイブパス（.pst）
        [Localize.LocalizedDescription("Description59")] //Outlook数据文件的保存路径 //Outlook data file save path //Outlookデータファイルの保存パス
        [RequiredArgument]
        public InArgument<string> PstFilePath
        {
            get;
            set;
        }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName94")] //待存档账户 //To be archived account //アカウントをアーカイブする
        [Localize.LocalizedDescription("Description60")] //Outlook账户名 //Outlook account name //Outlookアカウント名
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

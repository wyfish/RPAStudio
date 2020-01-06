using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Diagnostics;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.IO;
using MailKit.Net.Pop3;
using System.Net.Mail;
using System.Linq;
using System.Reflection;
using System.Globalization;
using MimeKit;
using MimeKit.Text;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(SaveMailDesigner))]
    public sealed class SaveMail : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "SaveMail";
            }
        }


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName45")] //路径 //path //パス
        [Localize.LocalizedDescription("Description43")] //要保存MailMessage对象的完整路径 //To save the full path of the MailMessage object //MailMessageオブジェクトのフルパスを保存するには
        public InArgument<string> PathName { get; set; }
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName81")] //邮件消息 //Mail message //メールメッセージ
        [Localize.LocalizedDescription("Description44")] //要保存的MailMessage对象 //The MailMessage object to save //保存するMailMessageオブジェクト
        public InArgument<MimeMessage> MimeMessageSave { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/Mail/SaveMail.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string pathName = PathName.Get(context);
            MimeMessage mailMessage = MimeMessageSave.Get(context);

            try
            {
                if (!Directory.Exists(pathName))
                    Directory.CreateDirectory(pathName);
                mailMessage.WriteTo(pathName);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "保存邮件失败", e.Message);
            }
        }

        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }
    }
}

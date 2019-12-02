using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Diagnostics;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.IO;
using MimeKit.Text;
using MailKit.Net.Pop3;
using System.Net.Mail;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(SaveMailAttachmentsDesigner))]
    public sealed class SaveMailAttachments : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "SaveMailAttachments";
            }
        }


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("路径")]
        [Description("要保存MimeMessage附件的完整路径")]
        public InArgument<string> PathName { get; set; }
        [Category("输入")]
        [RequiredArgument]
        [DisplayName("邮件消息")]
        [Description("要保存的MimeMessage附件")]
        public InArgument<MimeMessage> MimeMessageAttachs { get; set; }

        [Category("输出")]
        [DisplayName("附件列表")]
        [Description("检索到的附件")]
        public InArgument<string[]> AttachFiles { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/Mail/SaveMailAttachments.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string pathName = PathName.Get(context);
            MimeMessage mailMessage = MimeMessageAttachs.Get(context);
            try
            {
                if (!Directory.Exists(pathName))
                    Directory.CreateDirectory(pathName);
                List<string> list = new List<string>();
                foreach (MimePart attachment in mailMessage.Attachments)
                {
                    using (var cancel = new System.Threading.CancellationTokenSource())
                    {
                        string fileName = attachment.FileName;
                        string filePath = pathName + "\\" + fileName;
                        using (var stream = File.Create(filePath))
                        {
                            attachment.Content.DecodeTo(stream, cancel.Token);
                            list.Add(filePath);
                        }
                    }
                }
                AttachFiles.Set(context, list.ToArray());
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "保存附件失败", e.Message);
            }
        }
    }
}

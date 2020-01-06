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


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName45")] //路径 //path //パス
        [Localize.LocalizedDescription("Description45")] //要保存MimeMessage附件的完整路径 //The full path to save the MimeMessage attachment //MimeMessage添付ファイルを保存するフルパス
        public InArgument<string> PathName { get; set; }
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName81")] //邮件消息 //Mail message //メールメッセージ
        [Localize.LocalizedDescription("Description46")] //要保存的MimeMessage附件 //MimeMessage attachment to save //保存するMimeMessage添付ファイル
        public InArgument<MimeMessage> MimeMessageAttachs { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName82")] //附件列表 //Attachment list //添付リスト
        [Localize.LocalizedDescription("Description47")] //检索到的附件 //Retrieved attachment //取得した添付ファイル
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

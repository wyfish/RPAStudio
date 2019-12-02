using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Plugins.Shared.Library.Extensions;
using System.IO;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.Mail
{
    public class OutlookAPI
    {
        private static List<MailMessage> GetMessages(MAPIFolder inboxFolder, int count, string filter, bool onlyUnread, bool markAsRead, bool getAttachments, CancellationToken cancellationToken)
        {
            Items items = null;
            List<MailMessage> list = new List<MailMessage>();
            try
            {
                if (inboxFolder == null)
                {
                    throw new ArgumentException("找不到指定的邮件目录！");
                }
                bool flag = !string.IsNullOrEmpty(filter);
                bool flag2 = true;
                dynamic obj = null;
                items = inboxFolder.Items;
                items.Sort("[ReceivedTime]", true);
                if (flag && onlyUnread)
                {
                    obj = ((!filter.StartsWith("@SQL=", StringComparison.OrdinalIgnoreCase)) ? items.Find($"({filter}) and [UnRead] = True") : items.Find($"@SQL=(({filter.Substring(5)}) AND (\"urn:schemas:httpmail:read\" = 0))"));
                }
                else if (onlyUnread)
                {
                    obj = items.Find($"[UnRead] = {onlyUnread}");
                }
                else if (flag)
                {
                    obj = items.Find($"{filter}");
                }
                else
                {
                    obj = items.GetFirst();
                    flag2 = false;
                }
                int num = 0;
                while (true)
                {
                    if (!((obj != null) ? true : false))
                    {
                        return list;
                    }
                    if (num == count)
                    {
                        return list;
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (obj is MailItem)
                    {
                        if (markAsRead)
                        {
                            obj.UnRead = false;
                        }
                        list.Add(OutlookAPI.CreateMailMessageFromOutlookMailItem(obj, getAttachments, null));
                        num++;
                    }
                    obj = (flag2 ? items.FindNext() : items.GetNext());
                }
                return list;
            }
            finally
            {
                if (inboxFolder != null)
                {
                    Marshal.ReleaseComObject(inboxFolder);
                }
            }
        }


        public static List<MailMessage> GetMessages(string account, string folderMail, int count, string filter, bool onlyUnread, bool markAsRead, bool getAttachments, CancellationToken cancellationToken)
        {
            return GetMessages(GetFolder(folderMail, account), count, filter, onlyUnread, markAsRead, getAttachments, cancellationToken);
        }

        public static MAPIFolder GetFolder(string folderPath, string account)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return null;
            }
            Application application = null;
            try
            {
                application = InitOutlook();
                MAPIFolder mAPIFolder = null;
                if (string.IsNullOrWhiteSpace(account))
                {
                    account = application.Session.GetDefaultFolder(OlDefaultFolders.olFolderInbox).FolderPath;
                    account = account.Substring(2);
                    if (account.IndexOf('\\') < account.Length)
                    {
                        account = account.Substring(0, account.IndexOf('\\'));
                    }
                }

                for (int i = 1; i <= application.Session.Folders.Count; i++)
                {
                    if (application.Session.Folders[i].Name == account)
                    {
                        mAPIFolder = application.Session.Folders[i];
                        break;
                    }
                }

                if (mAPIFolder == null)
                {
                    throw new ArgumentException("请检查账户是否存在！");
                }

                string[] array = folderPath.Split('\\');
                Folders folders = mAPIFolder.Folders;
                MAPIFolder mAPIFolder2 = null;
                string[] array2 = array;
                foreach (string folder in array2)
                {
                    mAPIFolder2 = folders.OfType<MAPIFolder>().FirstOrDefault((MAPIFolder f) => string.Equals(folder, f.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (mAPIFolder2 == null)
                    {
                        throw new ArgumentException("找不到对应的目录，请检查账户及邮件目录设置是否正确！");
                    }
                    folders = mAPIFolder2.Folders;
                }
                return mAPIFolder2;
            }
            finally
            {
                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
            }

        }


        private static MailMessage CreateMailMessageFromOutlookMailItem(MailItem mailItem, bool saveattachments = false, string folderpath = null)
        {
            MailMessage mailMessage = new MailMessage();
            try
            {
                try
                {
                    mailMessage.Headers.Add("Uid", mailItem.EntryID);
                    mailMessage.Headers["Date"] = mailItem.SentOn.ToString();
                    mailMessage.Headers["DateCreated"] = mailItem.CreationTime.ToString();
                    mailMessage.Headers["DateRecieved"] = mailItem.ReceivedTime.ToString();
                    mailMessage.Headers["HtmlBody"] = mailItem.HTMLBody;
                    mailMessage.Headers["PlainText"] = mailItem.Body;
                    mailMessage.Headers["Size"] = mailItem.Size.ToString();
                }
                catch (System.Exception ex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                }
                mailMessage.Subject = mailItem.Subject;
                mailMessage.Body = mailItem.Body;
                try
                {
                    string fromAddress = GetFromAddress(mailItem);
                    string senderName = mailItem.SenderName;
                    if (!string.IsNullOrEmpty(senderName))
                    {
                        mailMessage.From = new MailAddress(fromAddress, senderName);
                        mailMessage.Sender = new MailAddress(fromAddress, senderName);
                    }
                    else
                    {
                        mailMessage.From = new MailAddress(fromAddress);
                        mailMessage.Sender = new MailAddress(fromAddress);
                    }
                }
                catch (System.Exception ex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                }
                mailMessage.Priority = MailPriority.Normal;
                if (mailItem.Importance == OlImportance.olImportanceHigh)
                {
                    mailMessage.Priority = MailPriority.High;
                }
                if (mailItem.Importance == OlImportance.olImportanceLow)
                {
                    mailMessage.Priority = MailPriority.Low;
                }
                try
                {
                    if (!string.IsNullOrEmpty(mailItem.To))
                    {
                        foreach (MailAddress item in GetMailAddressCollection(mailItem, OlMailRecipientType.olTo))
                        {
                            mailMessage.To.Add(item);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                }
                try
                {
                    if (!string.IsNullOrEmpty(mailItem.BCC))
                    {
                        foreach (MailAddress item2 in GetMailAddressCollection(mailItem, OlMailRecipientType.olBCC))
                        {
                            mailMessage.Bcc.Add(item2);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                }
                try
                {
                    if (!string.IsNullOrEmpty(mailItem.CC))
                    {
                        foreach (MailAddress item3 in GetMailAddressCollection(mailItem, OlMailRecipientType.olCC))
                        {
                            mailMessage.CC.Add(item3);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                }
                try
                {
                    if (saveattachments && mailItem.Attachments != null)
                    {
                        foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in mailItem.Attachments)
                        {
                            string text = null;
                            try
                            {
                                text = attachment.FileName;
                            }
                            catch
                            {
                                continue;
                            }
                            string path = System.IO.Path.Combine(folderpath ?? System.IO.Path.GetTempPath(), text);
                            attachment.SaveAsFile(path);
                            try
                            {
                                mailMessage.Attachments.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(path)), attachment.FileName));
                            }
                            finally
                            {
                                if (folderpath == null)
                                {
                                    System.IO.File.Delete(path);
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                }
                if (!saveattachments)
                {
                    return mailMessage;
                }
                if (string.IsNullOrEmpty(mailMessage.Body))
                {
                    return mailMessage;
                }
                mailMessage.AlternateViews.Add(new AlternateView(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(mailItem.HTMLBody)), "text/html"));
                return mailMessage;
            }
            catch (System.Exception ex)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);

                return mailMessage;
            }
        }

        private static string GetFromAddress(MailItem mailItem)
        {
            Application application = null;
            try
            {
                if (!(mailItem.SenderEmailType == "EX"))
                {
                    return mailItem.SenderEmailAddress;
                }
                application = InitOutlook();
                Recipient recipient = application.GetNamespace("MAPI").CreateRecipient(mailItem.SenderEmailAddress);
                AddressEntry addressEntry = recipient.AddressEntry;
                if (addressEntry != null)
                {
                    if (addressEntry.AddressEntryUserType != 0 && addressEntry.AddressEntryUserType != OlAddressEntryUserType.olExchangeRemoteUserAddressEntry)
                    {
                        return recipient.Address;
                    }
                    ExchangeUser exchangeUser = addressEntry.GetExchangeUser();
                    if (exchangeUser != null)
                    {
                        return exchangeUser.PrimarySmtpAddress;
                    }
                }
            }
            catch (System.Exception ex)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
            }
            finally
            {
                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
            }
            return string.Empty;
        }

        internal static Application InitOutlook()
        {
            Application application = null;
            try
            {
                application = (Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("0006F03A-0000-0000-C000-000000000046")));
                string version = application.Version;
                if (version != null && version.StartsWith("14.0"))
                {
                    MailItem mailItem = (MailItem)(dynamic)application.CreateItem(OlItemType.olMailItem);

                    Marshal.ReleaseComObject(mailItem.GetInspector);
                    Marshal.ReleaseComObject(mailItem);
                    return application;
                }
                return application;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == -2147221164)
                {
                    throw new SystemException("Outlook初始化失败，请检查Outlook是否已经安装", ex);
                }

                throw new SystemException(ex.Message, ex);
            }
            catch
            {
                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
                throw;
            }
        }

        public static MailAddressCollection GetMailAddressCollection(MailItem mailItem, OlMailRecipientType recipentType)
        {
            MailAddressCollection mailAddressCollection = new MailAddressCollection();
            Recipients recipients = mailItem.Recipients;
            try
            {
                foreach (Recipient item in recipients)
                {
                    if (item.Type == (int)recipentType)
                    {
                        try
                        {
                            string text = item.Address;
                            if (!text.Contains("@"))
                            {
                                try
                                {
                                    text = ((dynamic)item.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x39FE001E")).ToString();
                                }
                                catch (System.Exception ex)
                                {
                                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                                }
                            }
                            mailAddressCollection.Add(new MailAddress(text, item.Name));
                        }
                        catch (System.Exception ex)
                        {
                            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                        }
                    }
                }
                return mailAddressCollection;
            }
            finally
            {
                if (recipients != null)
                {
                    Marshal.ReleaseComObject(recipients);
                }
            }
        }



        public static void MoveMessage(MailMessage message, string folderPath, string account)
        {
            MAPIFolder folder = GetFolder(folderPath, account);
            if (folder == null)
            {
                throw new ArgumentException("找不到目录");
            }
            string text = message.Headers["Uid"];
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("非法邮件消息");
            }
            MailItem mailItem = null;
            MailItem mailItem2 = null;
            Application application = null;
            try
            {
                application = InitOutlook();
                mailItem = (dynamic)application.Session.GetItemFromID(text, Type.Missing);
                if (mailItem != null)
                {
                    mailItem2 = (mailItem.Move(folder) as MailItem);
                    mailItem2.Save();
                }
            }
            finally
            {
                if (folder != null)
                {
                    Marshal.ReleaseComObject(folder);
                }
                if (mailItem != null)
                {
                    Marshal.ReleaseComObject(mailItem);
                }
                if (mailItem2 != null)
                {
                    Marshal.ReleaseComObject(mailItem2);
                }
                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
            }
        }


        public static Task ReplyToAsync(MailMessage message, string body, List<string> attachments, bool replyAll, CancellationToken token)
        {
            return Task.Run(delegate
            {
                ReplyTo(message, body, attachments, replyAll, token);
            }, token);
        }

        public static void ReplyTo(MailMessage message, string body, List<string> attachments, bool replyAll, CancellationToken ct)
        {
            Application application = null;
            MailItem mailItem = null;
            MailItem mailItem2 = null;
            ct.ThrowIfCancellationRequested();
            application = InitOutlook();
            using (application.DisposeWithReleaseComObject())
            {
                NameSpace @namespace = application.GetNamespace("MAPI");
                string text = message.Headers["Uid"];
                if (string.IsNullOrEmpty(text))
                {
                    throw new ArgumentException("非法的邮件对象！");
                }
                mailItem = (dynamic)@namespace.GetItemFromID(text, Type.Missing);
                using (mailItem.DisposeWithReleaseComObject())
                {
                    mailItem2 = (replyAll ? mailItem.ReplyAll() : mailItem.Reply());
                }
                using (mailItem2.DisposeWithReleaseComObject())
                {
                    foreach (string attachment in attachments)
                    {
                        ct.ThrowIfCancellationRequested();
                        mailItem2.Attachments.Add(attachment, Type.Missing, Type.Missing, Type.Missing);
                    }
                    mailItem2.HTMLBody = body + mailItem2.HTMLBody;
                    mailItem2.BodyFormat = OlBodyFormat.olFormatHTML;
                    ct.ThrowIfCancellationRequested();
                    mailItem2.Send();
                }
            }
        }


        private static MAPIFolder GetDraftsFolder(Application outlookApp, string account)
        {
            try
            {
                NameSpace @namespace = outlookApp.GetNamespace("MAPI");
                Recipient recipient = @namespace.CreateRecipient(account);
                recipient.Resolve();
                if (!recipient.Resolved)
                {
                    throw new System.Exception("无法登陆");
                }
                return @namespace.GetSharedDefaultFolder(recipient, OlDefaultFolders.olFolderDrafts);
            }
            catch
            {
                return null;
            }
        }

        public static Account GetAccountForEmailAddress(Application application, string smtpAddress)
        {

            // Loop over the Accounts collection of the current Outlook session.
            Accounts accounts = application.Session.Accounts;
            foreach (Account account in accounts)
            {
                // When the email address matches, return the account.
                if (account.SmtpAddress == smtpAddress)
                {
                    return account;
                }
            }
            return null;
        }
        public static void SendMail(MailMessage mailMessage, string account, string sentOnBehalfOfName, string to, string cc, string bcc, List<string> attachments, bool isDraft, bool isBodyHtml)
        {
            Application application = null;
            MailItem mailItem = null;
            try
            {
                application = InitOutlook();
                if (isDraft && !string.IsNullOrWhiteSpace(account))
                {
                    MAPIFolder draftsFolder = GetDraftsFolder(application, account);
                    if (draftsFolder != null)
                    {
                        mailItem = (MailItem)(dynamic)draftsFolder.Items.Add(Type.Missing);
                    }
                }
                if (mailItem == null)
                {
                    mailItem = (application.CreateItem(OlItemType.olMailItem) as MailItem);
                }
                mailItem.SendUsingAccount = GetAccountForEmailAddress(application, account);
                mailItem.Subject = mailMessage.Subject;
                mailItem.Body = mailMessage.Body;
                if (isBodyHtml)
                {
                    mailItem.HTMLBody = mailMessage.Body;
                    mailItem.BodyFormat = OlBodyFormat.olFormatHTML;
                }
                mailItem.To = to;
                mailItem.CC = cc;
                mailItem.BCC = bcc;
                mailItem.Recipients.ResolveAll();
                foreach (string attachment in attachments)
                {
                    mailItem.Attachments.Add(attachment, Type.Missing, Type.Missing, Type.Missing);
                }
                foreach (System.Net.Mail.Attachment attachment2 in mailMessage.Attachments)
                {
                    string text = null;
                    try
                    {
                        text = Path.GetFileName(attachment2.Name);
                        text = Path.Combine(Path.GetTempPath(), text);
                        using (FileStream destination = File.Open(text, FileMode.Create, FileAccess.Write))
                        {
                            attachment2.ContentStream.Position = 0L;
                            attachment2.ContentStream.CopyTo(destination);
                        }
                        mailItem.Attachments.Add(text, Type.Missing, Type.Missing, Type.Missing);
                    }
                    catch (SystemException ex)
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                        throw;
                    }
                    finally
                    {
                        attachment2.ContentStream.Position = 0L;
                        if (!string.IsNullOrWhiteSpace(text) && File.Exists(text))
                        {
                            File.Delete(text);
                        }
                    }
                }
                if (isDraft)
                {
                    mailItem.Save();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(account))
                    {
                        try
                        {
                            mailItem.SendUsingAccount = application.Session.Accounts[account];
                        }
                        catch
                        {
                            throw new ArgumentException(string.Format("账户 {0} 找不到", account));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sentOnBehalfOfName))
                    {
                        mailItem.SentOnBehalfOfName = sentOnBehalfOfName;
                    }
                    mailItem.Send();
                }
            }
            finally
            {
                if (mailItem != null)
                {
                    Marshal.ReleaseComObject(mailItem);
                }
                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
            }
        }

        public static void DeleteMessage(MailMessage message)
        {
            string text = message.Headers["Uid"];
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("非法邮件消息");
            }
            MailItem mailItem = null;
            Application application = null;
            try
            {
                application = InitOutlook();
                mailItem = (dynamic)application.Session.GetItemFromID(text, Type.Missing);
                if (mailItem != null)
                {
                    mailItem.Delete();
                }
            }
            finally
            {
                if (mailItem != null)
                {
                    Marshal.ReleaseComObject(mailItem);
                }

                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
            }
        }

        

        public static void ArchiveMails(string account, string pstPath)
        {
            Application application = null;
            NameSpace ns = null;
            MAPIFolder oFolder = null;
            MAPIFolder oMailbox = null;
            try
            {
                application = InitOutlook();
                ns = (dynamic)application.Session;

                ns.AddStore(pstPath);
                oFolder = ns.Session.Folders.GetLast();
                
                for (int i = 1; i <= application.Session.Folders.Count; i++)
                {
                    if(string.IsNullOrEmpty(account))
                    {
                        oMailbox = application.Session.Folders[i];
                        break;
                    }

                    if (application.Session.Folders[i].Name == account)
                    {
                        oMailbox = application.Session.Folders[i];
                        break;
                    }
                }

                CopyRecursive(oMailbox, oFolder,ns);
            }
            catch (SystemException ex)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
                throw;
            }
            finally
            {
                ns.RemoveStore(oFolder);

                if (application != null)
                {
                    Marshal.ReleaseComObject(application);
                }
            }
        }

        /// <summary>
        /// 源目录所有文件复制到目的目录，重复的条目不复制，不重复的合并
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        private static void CopyRecursive(MAPIFolder src, MAPIFolder dst,NameSpace ns)
        {           
            foreach (Folder f in src.Folders)
            {
                bool bFind = false;
                Folder findFolder = null;
                foreach (Folder f2 in dst.Folders)
                {
                    if(f.Name == f2.Name)
                    {
                        bFind = true;
                        findFolder = f2;
                        break;
                    }
                }

                if(!bFind)
                {
                    f.CopyTo(dst);
                }
                else
                {
                    //for (int i = f.Items.Count; i > 0; i--)
                    //{
                    //    f.Items[i].Move(findFolder);
                    //}
                }
                
            }
        }
    }
}
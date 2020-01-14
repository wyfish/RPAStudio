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
using System.Activities.Statements;
using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(GetMailImgsDesigner))]
    public sealed class GetMailImgs : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "GetMailImgs";
            }
        }


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName76")] //邮件 //Mail //郵便
        [Localize.LocalizedDescription("Description35")] //用于提取图片的邮件，该字段仅支持MimeMessage变量 //The message used to extract the image, this field only supports the MimeMessage variable. //画像の抽出に使用されるメッセージ。このフィールドはMimeMessage変数のみをサポートします。
        public InArgument<MimeMessage> MimeMessage { get; set; }


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName65")] //文件夹 //Folder //フォルダー
        [Localize.LocalizedDescription("Description36")] //将邮件中的邮件存储到本地目录,为空则不保存 //Store the mail in the mail to a local directory. If it is empty, it will not be saved. //メール内のメールをローカルディレクトリに保存します。空の場合、保存されません。
        public InArgument<string> ImgFolder { get; set; }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName77")] //图片路径列表 //Picture path list //ピクチャーパスリスト
        [Localize.LocalizedDescription("Description37")] //图片本地保存路径列表 //Picture local save path list //画像のローカル保存パスリスト
        public OutArgument<string[]> ImgList { get; set; }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName78")] //图片资源列表 //Image resource list //イメージリソースリスト
        [Localize.LocalizedDescription("Description38")] //保存图片信息，分为地址和base64码两种方式 //Save image information, divided into address and base64 code //アドレスとBase64コードに分割された画像情報を保存する
        public OutArgument<string[]> ImgSourceList { get; set; }
        

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/Mail/GetImgs.png";
            }
        }

        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }

        protected override void Execute(CodeActivityContext context)
        {
            MimeMessage mimeMessage = MimeMessage.Get(context);
            List<string> imgList = new List<string>();
            List<string> imgSourceList = new List<string>();
            string imgFolder = ImgFolder.Get(context);

            string htmlBody = null;
            IEnumerable<MimeEntity> bodys;
            try
            {
                if (!Directory.Exists(imgFolder) && imgFolder != null && imgFolder != "")
                    Directory.CreateDirectory(imgFolder);

                htmlBody = mimeMessage.HtmlBody;
                ParseHtmlbody(imgFolder, htmlBody, ref imgList, ref imgSourceList);

                bodys = mimeMessage.BodyParts;
                ParseBodyParts(bodys, imgFolder, ref imgList, ref imgSourceList);
                ImgList.Set(context, imgList.ToArray());
                if(imgSourceList != null)
                    ImgSourceList.Set(context, imgSourceList.ToArray());
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取邮件图片失败", e.Message);
            }
        }

        //Html Agility Pack解析HTML nuget组件 真TM强大
        private void ParseHtmlbody(string imgFolder, string content, ref List<string> imgList, ref List<string> imgSourceList)
        {
            WebClient wc = new WebClient();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            HtmlNodeCollection titleNodes = doc.DocumentNode.SelectNodes("//img");
            if (titleNodes != null)
            {
                int i = 0;
                foreach (var item in titleNodes)
                {
                    i++;
                    string imgurl = item.Attributes["src"].Value.ToString();

                    Debug.WriteLine(imgurl);
                    try
                    {
                        int index = imgurl.LastIndexOf('.');
                        string postfix = imgurl.Substring(index);
                        DateTime time = DateTime.Now;
                        string filename = time.ToString("yyyyMMddHHmmssms");
                        string filepath = imgFolder + "\\" + filename + i + postfix;
                        if(imgFolder != null && imgFolder != "")
                        {
                            wc.DownloadFile(imgurl, filepath);
                            imgList.Add(filepath);
                        }
                        if(imgurl.Contains("http://") || imgurl.Contains("https://"))
                            imgSourceList.Add(imgurl);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void ParseBodyParts(IEnumerable<MimeEntity> bodys, string imgFolder, ref List<string> imgList, ref List<string> imgSourceList)
        {
            int flag = 0;
            try
            {
                foreach (MimeEntity entity in bodys)
                {
                    flag++;
                    ContentType type = entity.ContentType;
                    string mediaType = type.MediaType;
                    string mimeType = type.MimeType;
                    if (mediaType != "image")
                        continue;

                    MimePart part = entity as MimePart;
                    string fileName = part.FileName;
                    IMimeContent content = part.Content;
                    string postfix = "";
                    MemoryStream stream = new MemoryStream();
                    content.WriteTo(stream);
                    stream.Position = 0;
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string Base64Text = reader.ReadToEnd();
                    imgSourceList.Add(Base64Text);

                    if (imgFolder != null && imgFolder != "")
                    {

                        byte[] arr2 = Convert.FromBase64String(Base64Text);
                        using (MemoryStream ms2 = new MemoryStream(arr2))
                        {
                            ImageFormat format = ImageFormat.Jpeg;
                            Bitmap bmp2 = new Bitmap(ms2);
                            switch (mimeType)
                            {
                                case "image/gif":
                                    {
                                        format = ImageFormat.Gif;
                                        postfix = ".gif";
                                        break;
                                    }
                                case "image/x-ms-bmp":
                                    {
                                        format = ImageFormat.Bmp;
                                        postfix = ".bmp";
                                        break;
                                    }
                                case "image/x-png":
                                    {
                                        format = ImageFormat.Png;
                                        postfix = ".png";
                                        break;
                                    }
                                case "image/jpeg":
                                    {
                                        format = ImageFormat.Jpeg;
                                        postfix = ".jpeg";
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                            DateTime time = DateTime.Now;
                            string filename = time.ToString("yyyyMMddHHmmssms");
                            string filepath = imgFolder + "\\" + filename + flag + postfix;
                            bmp2.Save(filepath, format);
                            imgList.Add(filepath);
                            bmp2.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

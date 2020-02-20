using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RPA.UIAutomation.Activities.OCR
{
    /// <summary>
    /// 阿里云印刷文字识别
    /// </summary>
    [Designer(typeof(AliOCRDesigner))]
    public sealed class AliOCRActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "AliOCR"; } }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description88")]//要载入的图像的完整路径。//The full path of the image to load. //読み込む画像のフルパス。
        public InArgument<string> FileName { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description119")]//您的AppCode。//Your AppCode.//あなたのAppCode。
        public InArgument<string> AppCode { get; set; }

        bool Probability = false;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description120")]//是否需要识别结果中每一行的置信度，默认为false。//Whether the confidence level of each row in the result needs to be recognized is false by default.//結果の信頼度を確認する必要がありますか？デフォルトはfalseです。
        public bool probability
        {
            get { return Probability; }
            set { Probability = value; }
        }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description121")]//图片识别结果。//Image recognition results. //画像認識結果。
        public OutArgument<string> Result { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/OCR/ALiOCR.png";
            }
        }
        protected override void Execute(CodeActivityContext context)
        {
            string fileName = FileName.Get(context);
            string appcode = AppCode.Get(context);         
            string base64 = ImageToBase64String(fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            //限制图片大小
            if (fileInfo.Length > 1204 * 1204 * 4)
            {
                throw new Exception("文件不能大于4M");
            }
            //调用阿里云地址：https://ocrapi-document.taobao.com/ocrservice/document
            string host = "https://ocrapi-document.taobao.com";
            string path = "/ocrservice/document";
            string method = "POST";
            string querys = "";
            string bodys = "{\"img\":\""+base64+"\",\"url\":\"\",\"prob\":"+ probability.ToString().ToLower()+ "\"}";
            string url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }
            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            //根据API的要求，定义相对应的Content-Type
            httpRequest.ContentType = "application/json; charset=UTF-8";
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));          
            string responseStr = reader.ReadToEnd();
            Result.Set(context, responseStr);
            //OcrResult ocrResult = JsonConvert.DeserializeObject<OcrResult>(responseStr);
            //string text = OcrResultToString(ocrResult);
            //Result.Set(context, text);
        }
        //识别出的仅有结果
        private static string OcrResultToString(OcrResult ocrResult)
        {
            StringBuilder sb = new StringBuilder(500);
            var wordList = ocrResult.prism_wordsInfo;
           // sb.AppendLine("sid:" + ocrResult.sid + "\n" + "prism_version:" + ocrResult.prism_version + "\n" + "prism_wnum:" + ocrResult.prism_wnum + "\n");
            foreach (var item in wordList)
            {
                int leftX = item.pos[0].x;
                int blankSpaceCount = (int)Math.Floor((double)leftX / 60);
                if (blankSpaceCount > 0)
                {
                    sb.Append(' ', blankSpaceCount * 2);
                }
                sb.AppendLine(item.word);              
            }
            return sb.ToString();
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        //将图片转换为Base64字符串
        private static string ImageToBase64String(string fileName)
        {
            Bitmap bitmap = new Bitmap(fileName);
            string ex = System.IO.Path.GetExtension(fileName).ToLower();
            using (MemoryStream ms = new MemoryStream())
            {
                ImageFormat format;
                switch (ex)
                {
                    case ".png":
                        format = ImageFormat.Png;
                        break;
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    default:
                        format = ImageFormat.Bmp;
                        break;
                }
                bitmap.Save(ms, format);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
        }   
    }
}

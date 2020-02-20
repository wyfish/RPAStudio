using Baidu.Aip.Ocr;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace RPA.UIAutomation.Activities.OCR
{
    /// <summary>
    /// 百度云OCR网络图片文字识别
    /// </summary>
    [Designer(typeof(BaiDuOCRWebImageDesigner))]
    public sealed class BaiDuOCRWebImageActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "BaiDuOCR"; } }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description93")]//您的APIKey。//Your APIKey. //あなたのAPIKey。
        public InArgument<string> APIKey { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description94")]//您的SecretKey。//Your SecretKey.//あなたのSecretKey。
        public InArgument<string> SecretKey { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("FileName")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description83")]//图像的完整路径以及名称。如果设置了此属性，则忽略输入项中Image属性。//The full path and name of the image.If this property is set, the image property in the input item is ignored.//画像のフルパスと名前。この属性を設定すると、入力項目のImage属性は無視されます。
        public InArgument<string> FileName { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("image")]
        [DisplayName("Image")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description84")]//要进行处理的图像，仅支持Image变量。如果设置了此属性，则忽略输入项中FileName属性。//Only image variables are supported for images to be processed. If this property is set, the filename property in the entry is ignored.//処理する画像は、Image変数のみに対応します。この属性を設定すると、入力項目のFileName属性が無視されます。
        public InArgument<System.Drawing.Image> image { get; set; }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description121")]//图片识别结果。//Image recognition results. //画像認識結果。
        public OutArgument<string> Result { get; set; }

        bool Detect_direction = false;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName75")]//检测图像朝向 //Detect image orientation //画像の向きを検出
        [Browsable(true)]
        [Localize.LocalizedDescription("Description122")]//是否检测图像朝向，默认不检测。朝向是指输入图像是正常方向还是逆时针旋转90/180/270度。//Whether to detect the image orientation is not detected by default. Orientation refers to whether the input image rotates 90 / 180 / 270 degrees in normal direction or anticlockwise.//画像の向きを検出するかどうかは、デフォルトでは検出されません。向きとは、入力画像が正常方向か反時計回りに90/180/270度回転することです。
        public bool detect_direction
        {
            get { return Detect_direction; }
            set { Detect_direction = value; }
        }

        bool Detect_language = false;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName78")]//检测语言 //Detection language //検出言語
        [Browsable(true)]
        [Localize.LocalizedDescription("Description126")]//是否检测语言，默认不检测。当前支持（中文、英语、日语、韩语）//Whether to detect the language. It is not detected by default. Currently supported (Chinese, English, Japanese, Korean)//言語を検出するかどうかは、デフォルトでは検出されません。現在のサポート（中国語、英語、日本語、韓国語）
        public bool detect_language
        {
            get { return Detect_language; }
            set { Detect_language = value; }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/OCR/OCR.png";
            }
        }

        //将图片转换成字节数组
        public byte[] SaveImage(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return imgBytesIn;
        }

        //将Image变量转换成字节数组
        public byte[] ConvertImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        System.Drawing.Image img;
        byte[] by;
        protected override void Execute(CodeActivityContext context)
        {
            string path = FileName.Get(context);
            string API_KEY = APIKey.Get(context);
            string SECRET_KEY = SecretKey.Get(context);
            img = image.Get(context);
            try
            {
                if (path != null)
                {
                    by = SaveImage(path);
                }
                else
                {
                    by = ConvertImageToByte(img);
                }
                var client = new Ocr(API_KEY, SECRET_KEY);
                //修改超时时间   
                client.Timeout = 60000; 
                //参数设置               
                var options = new Dictionary<string, object>
                {
                    {"detect_direction",detect_direction.ToString().ToLower()},
                    {"detect_language", detect_language.ToString().ToLower()}
                };
                //带参数调用网络图片文字识别
                string result = client.WebImage(by, options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

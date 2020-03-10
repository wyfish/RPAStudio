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
    /// 百度云OCR通用文字识别（含位置信息高精度版）
    /// </summary>
    [Designer(typeof(BaiDuOCRLocationHighDesigner))]
    public sealed class BaiDuOCRLocationHighActivity : CodeActivity
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

        public enum Recognizegranularity
        {
            big,//不定位单字符位置
            small//定位单字符位置
        }
        Recognizegranularity Recognize_granularity = Recognizegranularity.big;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName77")]//定位单字符位置 //Locate single character position //単文字の位置を指定
        [Localize.LocalizedDescription("Description125")]//是否定位单字符位置，默认为big（不定位单字符位置）。//Whether to locate the single character position. The default is big (not to locate the single character position).//単文字の位置を特定するかどうかは、デフォルトではbigです。
        [Browsable(true)]
        public Recognizegranularity recognize_granularity
        {
            get { return Recognize_granularity; }
            set { Recognize_granularity = value; }
        }

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

        bool Vertexes_location = false;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName79")]//返回顶点位置 //Return to vertex position //頂点の位置に戻ります
        [Browsable(true)]
        [Localize.LocalizedDescription("Description127")]//是否返回文字外接多边形顶点位置，不支持单字位置，默认为false。//Whether to return to the vertex position of the text external polygon. Single word position is not supported. The default is false.//テキストの外側の多角形の頂点位置を返すかどうかは、単語の位置はサポートされていません。デフォルトはfalseです。
        public bool vertexes_location
        {
            get { return Vertexes_location; }
            set { Vertexes_location = value; }
        }

        bool Probability = false;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [DisplayName("Probability")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description123")]//是否返回识别结果中每一行的置信度。//Whether to return the confidence level of each line in the recognition result. //識別結果の各行の信頼度を返しますか？
        public bool probability
        {
            get { return Probability; }
            set { Probability = value; }
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
                    {"recognize_granularity", recognize_granularity},
                    {"detect_direction",detect_direction.ToString().ToLower()},
                    {"vertexes_location", vertexes_location.ToString().ToLower()},
                    {"probability", probability.ToString().ToLower()}
                };
                //带参数调用通用文字识别（含位置信息高精度版）
                string result = client.Accurate(by, options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("msgErrorOccurred"), e.Message);
            }
        }
    }
}

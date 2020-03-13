using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace RPA.UIAutomation.Activities.OCR
{
    /// <summary>
    /// Tesseract识别
    /// </summary>
    [Designer(typeof(TesseractDesigner))]
    public sealed class TesseractActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "TesseractOCR"; } }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("FileName")]
        [Browsable(true)]
        [Description("图片的完整路径以及名称。如果设置了此属性，则忽略输入项中Image属性。")]
        public InArgument<string> FileName { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("image")]
        [DisplayName("Image")]
        [Browsable(true)]
        [Description("要进行文本识别的图像，仅支持Image变量。如果设置了此属性，则忽略输入项中FileName属性。")]
        public InArgument<System.Drawing.Image> image { get; set; }

        public enum LanguageType
        {
            num,//验证码
            eng,//英文
            chi_sim,//中文（简体）
            chi_tra,//中文（繁体）
            jpn,//日语
            kor,//韩语
            fra//法语
        }
        LanguageType _Languagetype = LanguageType.eng;
        [Category("选项")]
        [DisplayName("语言类型")]
        [Description("识别语言类型，默认为eng(英文)。num:字母和数字结合;eng:英文;chi_sim:中文(简体);chi_tra:中文(繁体);jpn:日语;kor:韩语;fra:法语")]
        [Browsable(true)]
        public LanguageType _languagetype {
            get { return _Languagetype; }
            set { _Languagetype = value; }
        }

        public enum engineMode
        {
            TesseractOnly,
            CubeOnly,
            TesseractAndCube,
            Default
        }
        engineMode _EngineMode = engineMode.Default;
        [Category("选项")]
        [DisplayName("EngineMode")]
        [Description("默认为Default。")]
        [Browsable(true)]
        public engineMode _engineMode {
            get { return _EngineMode; }
            set { _EngineMode = value; }
        }

        [Category("输出")]
        [Browsable(true)]
        [Description("图片识别结果。")]
        public OutArgument<string> Result { get; set; }

        [Browsable(false)]
        public string icoPath {
            get {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/OCR/OCR.png";
            }
        }

        Bitmap img;
        protected override void Execute(CodeActivityContext context)
        {
            string fileName = FileName.Get(context);
            System.Drawing.Image _image = image.Get(context);
            if (fileName != null)
            {
                img = new Bitmap(fileName);
            }
            else
            {
                img = (Bitmap)_image;
            }
            var ocr = new TesseractEngine("tessdata", _languagetype.ToString(), (EngineMode)_engineMode);
            var page = ocr.Process(img);
            string text = page.GetText();
            Result.Set(context, text);
        }
    }
}
using Plugins.Shared.Library;
using RPA.UIAutomation.Activities;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace ImageOperaActivity
{
    [Designer(typeof(ImageConvertBase64Designer))]
    public sealed class ImageConvertBase64Activity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Image Convert";
            }
        }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDescription("Description86")]//要读取的文件的路径。 //The path to the file to read. //読み込むファイルのパス。
        public InArgument<string> FileName { get; set; }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Localize.LocalizedDescription("Description87")]//输出Base64。 //Output Base64. //Base 64を出力します。
        public OutArgument<string> Content { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/ImageConvertBase64.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string Imagefilename = FileName.Get(context);
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                string strbaser64 = Convert.ToBase64String(arr);
                Content.Set(context, strbaser64);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("msgErrorOccurred"), e.Message);
            }
        }
    }
}

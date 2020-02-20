using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(BinaryzationDesigner))]
    public sealed class BinaryzationActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Binaryzation"; } }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("FileName")]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description83")]//图像的完整路径以及名称。如果设置了此属性，则忽略输入项中Image属性。//The full path and name of the image.If this property is set, the image property in the input item is ignored.//画像のフルパスと名前。この属性を設定すると、入力項目のImage属性は無視されます。
        public InArgument<string> FileName { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("image")]
        [Localize.LocalizedDisplayName("DisplayName68")]//Image//Image//Image
        [Browsable(true)]
        [Localize.LocalizedDescription("Description84")]//要进行处理的图像，仅支持Image变量。如果设置了此属性，则忽略输入项中FileName属性。//Only image variables are supported for images to be processed. If this property is set, the filename property in the entry is ignored.//処理する画像は、Image変数のみに対応します。この属性を設定すると、入力項目のFileName属性が無視されます。
        public InArgument<System.Drawing.Image> image { get; set; }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Localize.LocalizedDisplayName("DisplayName68")]//Image//Image//Image
        [Browsable(true)]
        [Localize.LocalizedDescription("Description85")]//处理完成的图像。仅支持Image变量。//Process the finished image. Only image variables are supported.//処理が完了した画像。Image変数のみをサポートします。
        public OutArgument<System.Drawing.Image> _Image { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/image.png";
            }
        }
        
        //二值化处理
        public static Bitmap Binary(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* start = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (start[0] != 255)
                        {
                            start[0] = start[1] = start[2] = 0;
                        }
                        start += 4;
                    }
                    start += bdata.Stride - width * 4;
                }
            }
            img.UnlockBits(bdata);
            return img;
        }   

        System.Drawing.Image img;
        System.Drawing.Image imgBinary;
        private string imgBinaried;
        protected override void Execute(CodeActivityContext context)
        {
            img = image.Get(context);
            string fileName = this.FileName.Get(context);
            try
            {
                if (img != null)
                {

                }
                else
                {
                    Bitmap bit = new Bitmap(fileName);
                    img = bit as System.Drawing.Image;
                }
                imgBinary = (System.Drawing.Image)img.Clone();
                imgBinary = Binary((Bitmap)imgBinary);
                Common com = new Common();
                imgBinaried = com.Image2Num((Bitmap)imgBinary);
                _Image.Set(context, imgBinary);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(RemoveNoiseDesigner))]
    public sealed class RemoveNoiseActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "RemoveBackground"; } }

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

        InArgument<Int32> _Noise = 1;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName72")]//噪点阈值 //Noise threshold //ノイズのしきい値
        [Localize.LocalizedDescription("Description90")]//合适的噪点阈值可达到更好的去噪点效果。//A suitable noise threshold can achieve better denoising effect.//適切なノイズのしきい値は、より良いノイズ効果を達成することができます。
        [Browsable(true)]
        public InArgument<Int32> Noise
        {
            get
            {
                return _Noise;
            }
            set
            {
                _Noise = value;
            }
        }

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

        public static System.Drawing.Image RemoveNoise(Bitmap img, int maxAroundPoints = 1)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
            #region 指针法
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i == 0 || i == height - 1 || j == 0 || j == width - 1) //边界点，直接当作噪点去除掉
                        {
                            ptr[0] = ptr[1] = ptr[2] = 255;
                        }
                        else
                        {
                            int aroundPoint = 0;
                            if (ptr[0] != 255) //看标记，不是背景点
                            {
                                //判断其周围8个方向与自己相连接的有几个点
                                if ((ptr - 4)[0] != 255) aroundPoint++; //左边
                                if ((ptr + 4)[0] != 255) aroundPoint++; //右边
                                if ((ptr - width * 4)[0] != 255) aroundPoint++; //正上方
                                if ((ptr - width * 4 + 4)[0] != 255) aroundPoint++; //右上角
                                if ((ptr - width * 4 - 4)[0] != 255) aroundPoint++; //左上角
                                if ((ptr + width * 4)[0] != 255) aroundPoint++; //正下方
                                if ((ptr + width * 4 + 4)[0] != 255) aroundPoint++; //右下方
                                if ((ptr + width * 4 - 4)[0] != 255) aroundPoint++; //左下方
                            }
                            if (aroundPoint < maxAroundPoints)//目标点是噪点
                            {
                                ptr[0] = ptr[1] = ptr[2] = 255; //去噪点
                            }
                        }
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            img.UnlockBits(bdata);
            #endregion
            return img;
        }

        System.Drawing.Image img;
        private string imgNoised;
        //private static string imgurl;
        protected override void Execute(CodeActivityContext context)
        {
            img = image.Get(context);
            string fileName = this.FileName.Get(context);
            Int32 noise = Noise.Get(context);
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
                System.Drawing.Image img_noise = RemoveNoise((Bitmap)img.Clone(), noise);
                Common com = new Common();
                imgNoised = com.Image2Num((Bitmap)img_noise);
              //  WriteToFile(imgNoised, "experiment\\" + Path.GetFileNameWithoutExtension(imgurl) + "_noised.txt");
                _Image.Set(context, img_noise);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("msgErrorOccurred"), e.Message);
            }
        }
    }
}

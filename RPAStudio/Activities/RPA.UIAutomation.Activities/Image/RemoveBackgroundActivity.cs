using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(RemoveBackgroundActivityDesigner))]
    public sealed class RemoveBackgroundActivity : CodeActivity
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

        public static int ComputeThresholdValue(Bitmap img)
        {
            int i;
            int k;
            double csum;
            int thresholdValue = 1;
            int[] ihist = new int[0x100];
            for (i = 0; i < 0x100; i++)
            {
                ihist[i] = 0;
            }
            int gmin = 0xff;
            int gmax = 0;
            for (i = 1; i < (img.Width - 1); i++)
            {
                for (int j = 1; j < (img.Height - 1); j++)
                {
                    int cn = img.GetPixel(i, j).R; //生成直方图
                    ihist[cn]++;
                    if (cn > gmax)
                    {
                        gmax = cn; //找到最大像素点R
                    }
                    if (cn < gmin)
                    {
                        gmin = cn; //找到最小像素点R
                    }
                }
            }
            double sum = csum = 0.0;
            int n = 0;
            for (k = 0; k <= 0xff; k++)
            {
                sum += k * ihist[k];
                n += ihist[k];
            }
            if (n == 0)
            {
                return 60;
            }
            double fmax = -1.0;
            int n1 = 0;
            for (k = 0; k < 0xff; k++)
            {
                n1 += ihist[k];
                if (n1 != 0)
                {
                    int n2 = n - n1;
                    if (n2 == 0)
                    {
                        return thresholdValue;
                    }
                    csum += k * ihist[k];
                    double m1 = csum / ((double)n1);
                    double m2 = (sum - csum) / ((double)n2);
                    double sb = ((n1 * n2) * (m1 - m2)) * (m1 - m2);
                    if (sb > fmax)
                    {
                        fmax = sb;
                        thresholdValue = k;
                    }
                }
            }
            return thresholdValue;
        }
        /// <summary>
        /// 去除背景
        /// </summary>
        /// <param name="img">原图片</param>
        /// <param name="dgGrayValue">前景背景分界灰度值</param>
        /// <returns></returns>
        public static System.Drawing.Image RemoveBg(Bitmap img, int dgGrayValue)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb); //红绿蓝个八位，其余8位没使用
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (ptr[1] > dgGrayValue)//背景点
                        {
                            ptr[0] = ptr[1] = ptr[2] = 255;
                        }
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            //从内存中解锁
            img.UnlockBits(bdata);
            return img;
        }

        private string imgBged;
        System.Drawing.Image img;
        System.Drawing.Image img_noise;
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
                img_noise = (System.Drawing.Image)img.Clone();
                int v = ComputeThresholdValue((Bitmap)img_noise);
                System.Drawing.Image img_bg = RemoveBg((Bitmap)img_noise, v);//-----------去背景，内存法
                Common com = new Common();
                imgBged = com.Image2Num((Bitmap)img_bg);
                _Image.Set(context, img_bg);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("msgErrorOccurred"), e.Message);
            }
        }
    }
}

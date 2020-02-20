using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(GrayDesigner))]
    public sealed class GrayActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "CAPTCHA"; } }

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

        bool _pingjun = true;
        [Localize.LocalizedCategory("Category12")]//灰度方式 //Grayscale mode //グレースケール
        [Localize.LocalizedDisplayName("DisplayName69")]//平均值法 //Average method //平均値法
        [Browsable(true)]
        public bool Pingjun
        {
            get
            {
                return _pingjun;
            }
            set
            {
                _pingjun = value;
            }
        }
        bool _max;
        [Localize.LocalizedCategory("Category12")]//灰度方式 //Grayscale mode //グレースケール
        [Localize.LocalizedDisplayName("DisplayName70")]//最大值法 //Maximum value method //最大値法
        [Browsable(true)]
        public bool Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
        bool _quanzhong;
        [Localize.LocalizedCategory("Category12")]//灰度方式 //Grayscale mode //グレースケール
        [Localize.LocalizedDisplayName("DisplayName71")]//加权平均 //weighted mean //加重平均
        [Browsable(true)]
        public bool Quanzhong
        {
            get
            {
                return _quanzhong;
            }
            set
            {
                _quanzhong = value;
            }
        }
        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/image.png";
            }
        }
        /// <summary>
        /// 图片灰度化处理指针法
        /// </summary>
        /// <param name="img">待处理图片</param>
        /// <param name="type">1：最大值；2：平均值；3：加权平均；默认平均值</param>
        /// <returns>灰度处理后的图片</returns>
        public static System.Drawing.Image Gray(Bitmap img, int type = 2)
        {
            Func<int, int, int, int> getGrayValue;
            switch (type)
            {
                case 1:
                    getGrayValue = GetGrayValueByMax;
                    break;
                case 2:
                    getGrayValue = GetGrayValueByPingjunzhi;
                    break;
                default:
                    getGrayValue = GetGrayValueByQuanzhong;
                    break;
            }
            int height = img.Height;
            int width = img.Width;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int v = getGrayValue(ptr[0], ptr[1], ptr[2]);
                        ptr[0] = ptr[1] = ptr[2] = (byte)v;
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            img.UnlockBits(bdata);
            return img;
        }
        //最大值法
        private static int GetGrayValueByMax(int r, int g, int b)
        {
            int max = r;
            max = max > g ? max : g;
            max = max > b ? max : b;
            return max;
        }
        //平均值法
        private static int GetGrayValueByPingjunzhi(int r, int g, int b)
        {
            return (r + g + b) / 3;
        }
        //加权平均
        private static int GetGrayValueByQuanzhong(int b, int g, int r)
        {
            return (int)(r * 0.3 + g * 0.59 + b * 0.11);
        }     
       
        private int graytype = 2;
        private string imgGreied;
        System.Drawing.Image img;
        protected override void Execute(CodeActivityContext context)
        {
            string fileName = this.FileName.Get(context);
            img = image.Get(context);
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
                System.Drawing.Image img_grey = (System.Drawing.Image)img.Clone();
                Common com = new Common();
                string imgOriginal=com.Image2Num((Bitmap)img_grey);
                
                graytype = 2;
                if (_max)
                {
                    graytype = 1;
                }
                if (_quanzhong)
                {
                    graytype = 3;
                }
                img_grey = Gray((Bitmap)img_grey, graytype);
                imgGreied =com.Image2Num((Bitmap)img_grey);           
                
                _Image.Set(context, img_grey);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }     
        }
    }
}

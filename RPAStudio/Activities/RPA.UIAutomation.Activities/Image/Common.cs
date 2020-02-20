using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace RPA.UIAutomation.Activities.Image
{
    public class Common
    {
        //图片转换成像素数值
        public string Image2Num(Bitmap img)
        {
            StringBuilder sb = new StringBuilder();
            string imgPixel = "";
            int height = img.Height;
            int width = img.Width;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytelength = width * height * 4;
            byte[] bytes = new byte[bytelength];
            Marshal.Copy(bdata.Scan0, bytes, 0, bytelength);
            int row = 1;
            for (int i = 0; i < bytelength; i += 4)
            {
                imgPixel = bytes[i] + "," + bytes[i + 1] + "," + bytes[i + 2];
                if (imgPixel.Length < 11)
                {
                    for (int j = 0; j < 11 - imgPixel.Length; j++)
                    {
                        imgPixel += " ";
                    }
                }
                imgPixel += "\t";
                sb.Append(imgPixel);
                if (row % width == 0)
                {
                    sb.Append("\r\n");
                }
                row++;
            }
            img.UnlockBits(bdata);
            return sb.ToString();           
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPAStudio.Librarys
{
    /// <summary>
    /// 网络下载较长时，通过显示占位图来优化显示效果
    /// </summary>
    public class ImageExt : Image
    {
        private static Dictionary<string, BitmapImage> s_downloadDataMap = new Dictionary<string, BitmapImage>();

        
        public static void ClearCache()
        {
            s_downloadDataMap.Clear();
        }




        public string DefaultSource
        {
            get { return (string)GetValue(DefaultSourceProperty); }
            set { SetValue(DefaultSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultSourceProperty =
            DependencyProperty.Register("DefaultSource", typeof(string), typeof(ImageExt), new PropertyMetadata(DefaultSourceChangedCallback));

        private static void DefaultSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imgControl = d as ImageExt;
            var src = e.NewValue.ToString();

            if (string.IsNullOrEmpty(src))
            {
                return;
            }

            imgControl.Source = new BitmapImage(new Uri(src)); ;
        }



        public string UrlSource
        {
            get { return (string)GetValue(UrlSourceProperty); }
            set { SetValue(UrlSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UrlSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlSourceProperty =
            DependencyProperty.Register("UrlSource", typeof(string), typeof(ImageExt), new PropertyMetadata(UrlSourceChangedCallback));

        private static void UrlSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imgControl = d as ImageExt;
            var url = e.NewValue.ToString();

            if (string.IsNullOrEmpty(url))
            {                
                return;
            }


            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
            {
                if(s_downloadDataMap.ContainsKey(url))
                {
                    imgControl.Source = s_downloadDataMap[url];
                    return;
                }

                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadDataCompleted += (sender, args) =>
                    {
                        if(args.Error == null)
                        {
                            MemoryStream mem = new MemoryStream(args.Result);
                            BitmapImage bmp = null;
                            try
                            {
                                bmp = new BitmapImage();
                                bmp.BeginInit();
                                bmp.StreamSource = mem;
                                bmp.EndInit();
                            }
                            catch
                            {
                                bmp = null;
                            }

                            imgControl.Source = bmp;

                            s_downloadDataMap[url] = bmp;
                        }
                    };

                    webClient.DownloadDataAsync(new Uri(url));
                }
            }
            else
            {
                imgControl.Source = new BitmapImage(new Uri(url));
            }
        }

    }
}

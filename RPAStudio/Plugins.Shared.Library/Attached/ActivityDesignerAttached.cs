using System;
using System.Activities.Presentation;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plugins.Shared.Library.Attached
{
    public class ActivityDesignerAttached
    {


        /// <summary>
        /// Icon附加属性，值限定在特定目录下
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, string value)
        {
            obj.SetValue(IconProperty, value);
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(string), typeof(ActivityDesignerAttached), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIconChanged)));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var designer = d as ActivityDesigner;
            var newValue = e.NewValue as string;

            ImageDrawing img_drawing = new ImageDrawing();
            img_drawing.Rect = new Rect(0, 0, 16, 16);

            if(newValue.StartsWith("pack://", System.StringComparison.CurrentCultureIgnoreCase))
            {
                var name = d.GetType().Assembly.GetName().Name;
                img_drawing.ImageSource = new BitmapImage(new Uri(newValue));
            }
            else
            {
                var name = d.GetType().Assembly.GetName().Name;
                img_drawing.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/{name};Component/" + newValue));
            }
           
            designer.Icon = new DrawingBrush(img_drawing);
        }



        /// <summary>
        /// Resources附加属性，值限定在特定目录下
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetResources(DependencyObject obj)
        {
            return (string)obj.GetValue(ResourcesProperty);
        }

        public static void SetResources(DependencyObject obj, string value)
        {
            obj.SetValue(ResourcesProperty, value);
        }

        // Using a DependencyProperty as the backing store for Resources.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResourcesProperty =
            DependencyProperty.RegisterAttached("Resources", typeof(string), typeof(ActivityDesignerAttached), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnResourcesChanged)));

        private static void OnResourcesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var designer = d as ActivityDesigner;
            var newValue = e.NewValue as string;

            string[] sArray = newValue.Split('|');

            foreach(var item in sArray)
            {
                ResourceDictionary rd = null;
                if (item.StartsWith("pack://", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    rd = new ResourceDictionary() { Source = new Uri(item) };
                }
                else
                {
                    var name = d.GetType().Assembly.GetName().Name;
                    rd = new ResourceDictionary() { Source = new Uri($"pack://application:,,,/{name};Component/" + item) };
                }
                
                designer.Resources.MergedDictionaries.Add(rd);
            }

            
        }
    }
}

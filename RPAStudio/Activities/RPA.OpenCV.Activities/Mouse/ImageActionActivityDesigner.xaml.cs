using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library.Window;

namespace RPA.OpenCV.Activities.Mouse
{
    /// <summary>
    /// ImageActionActivityDesigner.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageActionActivityDesigner
    {
        public ImageActionActivityDesigner()
        {
            InitializeComponent();
        }
        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            //UiElement.OnSelected = UiElement_OnSelected;
            //UiElement.StartWindowHighlight();
            SnipTemplateImage();
        }

        private void SnipTemplateImage()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            System.Threading.Thread.Sleep(400);
            var img = Snip.SnippingTool.Snip2();
            if (img != null)
            {
                string templateFilePath = GetTemplateFilePath();
                SaveByFileStream(img, templateFilePath);
                setPropertyValue("SourceImgPath", templateFilePath);
                grid1.Visibility = System.Windows.Visibility.Hidden;
                setPropertyValue("visibility", System.Windows.Visibility.Visible);
            }
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private string GetTemplateFilePath()
        {
            var guid = Guid.NewGuid().ToString("N");
            var screenshotsDir = Plugins.Shared.Library.SharedObject.Instance.ProjectPath + @"\.screenshots";
            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }
            var fileName = guid + @".bmp"; // Do not use except .bmp
            return screenshotsDir + @"\" + fileName;
        }

        private void SaveByFileStream(Image img, string savePath)
        {
            if (img == null) return;
            
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                string lowerdPath = savePath.ToLower();
                if (lowerdPath.EndsWith(".bmp"))
                    img.Save(fs, ImageFormat.Bmp);
                else if (lowerdPath.EndsWith(".png"))
                    img.Save(fs, ImageFormat.Png);
                else if (lowerdPath.EndsWith(".jpg"))
                    img.Save(fs, ImageFormat.Jpeg);
                else
                    img.Save(fs, ImageFormat.Bmp);
                fs.Close();
            }
        }

        private void UiElement_OnSelected(UiElement uiElement)
        {
            var screenshotsPath = uiElement.CaptureInformativeScreenshotToFile();
            setPropertyValue("SourceImgPath", screenshotsPath);
            grid1.Visibility = System.Windows.Visibility.Hidden;
            setPropertyValue("visibility", System.Windows.Visibility.Visible);
            string displayName = getPropertyValue("_DisplayName") + " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";
            setPropertyValue("DisplayName", displayName);
            InArgument<string> _name = uiElement.Name;
            setPropertyValue("Title", _name);
        }

        private void setPropertyValue<T>(string propertyName, T value)
        {
            base.ModelItem.Properties[propertyName].SetValue(value);
        }

        private string getPropertyValue(string propertyName)
        {
            ModelProperty _property = base.ModelItem.Properties[propertyName];
            if (_property.Value == null)
                return "";
            return _property.Value.ToString();
        }

        private void HiddenNavigateTextBlock()
        {
            navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
        }

        //菜单按钮点击
        private void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
            contextMenu.PlacementTarget = this.navigateButton;
            contextMenu.Placement = PlacementMode.Top;
            contextMenu.IsOpen = true;
        }

        //菜单按钮初始化
        private void NavigateButtonInitialized(object sender, EventArgs e)
        {
            navigateButton.ContextMenu = null;
        }

        //菜单项点击测试
        private void meauItemClickOne(object sender, RoutedEventArgs e)
        {
            //UiElement.OnSelected = UiElement_OnSelected;
            //UiElement.StartWindowHighlight();
            SnipTemplateImage();
        }

        private void Button_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //string src = getPropertyValue("SourceImgPath");
            //ShowImageWindow imgShow = new ShowImageWindow();
            //imgShow.ShowImage(src);
        }

        private void ActivityDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            string src = getPropertyValue("SourceImgPath");
            if (src != "")
                grid1.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}

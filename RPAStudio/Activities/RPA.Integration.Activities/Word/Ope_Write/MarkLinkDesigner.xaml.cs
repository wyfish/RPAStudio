using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RPA.Integration.Activities.WordPlugins
{
    /// <summary>
    /// WriteText.xaml 的交互逻辑
    /// </summary>
    public partial class MarkLinkDesigner
    {
        public MarkLinkDesigner()
        {
            InitializeComponent();
        }

        private void SetPropertyVisibility(object obj, string propertyName, bool visible)
        {
            System.Diagnostics.Debug.WriteLine("SetPropertyVisibility...........");

            try
            {
                Type type = typeof(BrowsableAttribute);
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
                AttributeCollection attrs = props[propertyName].Attributes;
                FieldInfo fld = type.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
                fld.SetValue(attrs[type], visible);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SetPropertyVisibility Exception: " + ex);
            }
        }

        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }

        private void btnMark_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            //ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("Text"));
            //ModelItem item = _property.Value;
            //SetPropertyVisibility(ModelItem, "LinkMark", false);
        }

        private void btnMark_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnMark_Click");
            SetPropertyVisibility(ModelItem, "LinkMark", false);
            //ModelItem.
        }
    }
}

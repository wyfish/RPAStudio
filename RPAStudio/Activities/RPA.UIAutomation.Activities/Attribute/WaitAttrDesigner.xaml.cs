using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Activities;

namespace RPA.UIAutomation.Activities.Attribute
{
    public partial class WaitAttrDesigner
    {
        public WaitAttrDesigner()
        {
            InitializeComponent();
            Net.Surviveplus.Localization.WpfLocalization.ApplyResources(this, Properties.Resources.ResourceManager);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string comboBoxValue = comboBox.SelectedItem.ToString();
            if (comboBoxValue != null)
            {
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("Item"));
                InArgument<string> newValue = comboBoxValue;
                _property.SetValue(newValue);
            }
        }
    }
}

using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;


namespace RPA.UIAutomation.Activities.Find
{
    public partial class CAAnchorDesigner
    {
        public CAAnchorDesigner()
        {
            InitializeComponent();
        }
        private string getPropertyValue(string propertyName)
        {
            List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }
    }
}

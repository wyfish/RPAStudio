using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;

namespace RPA.Core.Activities.EnvironmentActivity
{
   

    public class CommonVariable
    {

        public static string getPropertyValue(string propertyName, ModelItem currItem)
        {
            List<ModelProperty> PropertyList = currItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }
    }
}

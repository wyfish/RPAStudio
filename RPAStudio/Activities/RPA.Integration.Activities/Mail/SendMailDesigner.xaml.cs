using Plugins.Shared.Library;
using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Activities;
using Microsoft.VisualBasic.Activities;
using System.Activities.Expressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Activities.Presentation.View;

namespace RPA.Integration.Activities.Mail
{
    public partial class SendMailDesigner
    {
        private string classID
        {
            get
            {
                return Guid.NewGuid().ToString("N");
            }
        }
        private string screenshotsPath { get; set; }

        public SendMailDesigner()
        {
            InitializeComponent();
        }

        private void setPropertyValue<T>(string propertyName, T value)
        {
            List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            _property.SetValue(value);
        }

        private object getPropertyValue(string propertyName)
        {
            List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.ComputedValue;
        }

        private string[] InArgumentConverter(ModelItem modelItem)
        {
            try
            {
                InArgument<string[]> inArgument = modelItem.GetCurrentValue() as InArgument<string[]>;
                
                if (inArgument != null)
                {
                    Activity<string[]> expression = inArgument.Expression;
                    VisualBasicValue<string[]> vbexpression = expression as VisualBasicValue<string[]>;
                    string arrayContent = vbexpression.ExpressionText.ToString();
                    arrayContent = arrayContent.TrimStart('{');
                    arrayContent = arrayContent.TrimEnd('}');
                    string[] sArray = arrayContent.Split('"');
                    List<string> list = new List<string>();
                    foreach (string str in sArray)
                    {
                        if (str != "," && str != "")
                            list.Add(str);
                    }
                    if (list.ToArray().Length != 0)
                        return list.ToArray();
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        //public static readonly DependencyProperty CheckedTextProperty = 
        //    DependencyProperty.Register("Expression", typeof(string), typeof(SendMailDesigner),
        //        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        private void PathSelect(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();


                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("AttachFiles"));
                ModelItem item = _property.Value;
                string[] fileArray = InArgumentConverter(item);
                string[] newArray;
                if (fileArray != null)
                {
                    List<String> list = new List<string>(fileArray);
                    list.Add(fName);
                    newArray = list.ToArray();
                }
                else
                {
                    newArray = new string[1];
                    newArray[0] = fName;
                }

                string str = null;
                for(int i=0; i < newArray.Length; i++)
                {
                    if (i == 0)
                        str += '{';
                    string filestr = '"' + newArray[i] + '"';
                    str += filestr;
                    if (i == newArray.Length - 1)
                        str += '}';
                    else
                        str += ',';
                }


                //使用MODELITEM PROPERTY赋值数组 会出现错误
                //expressTextBox3.ExpressionActivityEditor.
                expressTextBox3.Expression.Properties["ExpressionText"].SetValue(str);
            }
        }
    }
}

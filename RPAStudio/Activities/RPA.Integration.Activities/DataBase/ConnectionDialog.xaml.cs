using Microsoft.Data.ConnectionUI;
using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA.Integration.Activities.Database
{ 
    // ConnectionDialog.xaml 的交互逻辑
    public partial class ConnectionDialog
    {
        public ConnectionDialog(ModelItem modelItem)
        {
            InitializeComponent();
            ModelItem = modelItem;
            InitComboBox();
        }
        private void InitComboBox()
        {
            List<string> ProviderNames = new List<string>();
            var installedProviders = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow installedProvider in installedProviders.Rows)
            {
                ProviderNames.Add(installedProvider["InvariantName"] as string);
            }
            SourceList.ItemsSource = ProviderNames;
        }

        private void NewConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            DataConnectionDialog dcd = new DataConnectionDialog();
            DataSource.AddStandardDataSources(dcd);
            //不先打开更改数据源窗口设置
            dcd.SelectedDataSource = DataSource.SqlDataSource;
            dcd.SelectedDataProvider = DataProvider.SqlDataProvider;

            if (DataConnectionDialog.Show(dcd) == System.Windows.Forms.DialogResult.OK)
            {
                InArgument<string> connStr = dcd.ConnectionString;
                DataProvider provider = dcd.SelectedDataProvider;
                string providerName = provider.Name;
                InArgument<string> providerInarg = providerName;
                ModelItem.Properties["ConnectionString"].SetValue(connStr);
                ModelItem.Properties["ProviderName"].SetValue(providerInarg);
            }
        }

        private void ComboboxControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = (e.OriginalSource as System.Windows.Controls.ComboBox).SelectedValue;
            if (selectedValue != null)
            {
                ModelItem.Properties["ProviderName"].SetValue(new InArgument<string>(selectedValue.ToString()));
            }
        }
    }
}

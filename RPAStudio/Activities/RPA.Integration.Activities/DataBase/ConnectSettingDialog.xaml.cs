using System;
using System.Activities;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Data.ConnectionUI;
using System.Data.Common;
using System.Data;

namespace RPA.Integration.Activities.Database
{
    //原基于WINDOW设计的数据库连接窗口 已淘汰
    public partial class ConnectSettingDialog : Window
    {
        ModelProperty ProConnStr;
        ModelProperty ProProvider;
        ConnectDesigner ConnDesigner;
        ModelItem root;
        string ProviderName = null;

        public ConnectSettingDialog(ConnectDesigner designer, ModelItem item, string ProConnName, string ProviderName)
        {
            ConnDesigner = designer;
            root = item;
            ProConnStr = item.Properties[ProConnName];
            ProProvider = item.Properties[ProviderName];
            InitializeComponent();
            this.Loaded += ExpressionEditorDialog_Loaded;

            InitComboBox();
        }

        private void InitComboBox()
        {
            List<string>  ProviderNames = new List<string>();
            var installedProviders = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow installedProvider in installedProviders.Rows)
            {
                ProviderNames.Add(installedProvider["InvariantName"] as string);
            }
            comboBox.ItemsSource = ProviderNames;
        }

        public ModelItem ProConnValue
        {
            get
            {
                if (ProConnStr != null)
                    return ProConnStr.Value;
                else
                    return null;
            }
            set
            {
                ProConnStr.SetValue(value);
            }
        }

        public ModelItem ProviderValue
        {
            get
            {
                return ProProvider.Value;
            }
            set
            {
                ProProvider.SetValue(value);
            }
        }

        public enum ProviderEnums
        {
            SystemDataOdbc
        }

        public IEnumerable<ProviderEnums> ProviderList
        {
            get
            {
                return Enum.GetValues(typeof(ProviderEnums)).Cast<ProviderEnums>();
            }
        }

        private void ExpressionEditorDialog_Loaded(object sender, RoutedEventArgs e)
        {
            etb.OwnerActivity = root;
            etb.HintText = "Enter a VB Expression";
            etb.ExplicitCommit = false;

            System.Windows.Data.Binding binding1 = new System.Windows.Data.Binding();
            binding1.Source = this;
            binding1.ConverterParameter = ArgumentDirection.In;
            binding1.Converter = new ArgumentToExpressionConverter();
            binding1.Mode = BindingMode.TwoWay;
            binding1.Path = new PropertyPath("ProConnValue");
            etb.SetBinding(ExpressionTextBox.ExpressionProperty, binding1);

            etb.Loaded += new RoutedEventHandler(OnExpressionTextBoxLoaded);
        }

        void OnExpressionTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            (sender as ExpressionTextBox).BeginEdit();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            DesignerView.CommitCommand.Execute(etb);
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string comboBoxValue = comboBox.SelectedItem.ToString();
            if (comboBoxValue != null)
            {
                ProviderName = comboBoxValue;
                InArgument<string> newValue = comboBoxValue;
                ProProvider.SetValue(newValue);
            }
        }

        private void btnWizard_Click(object sender, RoutedEventArgs e)
        {
                runDataDialog();
        }

        //MYSQL连接窗口
        private void runMysqlDialog()
        {
            //ConnectDialog dialog = new ConnectDialog();
            //dialog.StartPosition = FormStartPosition.CenterScreen;

            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    try
            //    {
            //        string Text = dialog.Connection.ConnectionString;
            //    }
            //    catch (Exception e)
            //    {
            //        SharedObject.RunInUI(() =>
            //        {
            //            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取MySQL连接字符串出错", e.Message);
            //        });
            //    }
            //}
        }



        private void runDataDialog()
        {            
            DataConnectionDialog dcd = new DataConnectionDialog();
            DataSource.AddStandardDataSources(dcd);
            //DataConnectionConfiguration dataConnectionSetting = new DataConnectionConfiguration(null);
            //dataConnectionSetting.LoadConfiguration(dcd);

            //不先打开更改数据源窗口设置
            dcd.SelectedDataSource = DataSource.SqlDataSource;
            dcd.SelectedDataProvider = DataProvider.SqlDataProvider;

            if (DataConnectionDialog.Show(dcd) == System.Windows.Forms.DialogResult.OK)
            {
                InArgument<string> connStr = dcd.ConnectionString;
                ProConnStr.SetValue(connStr);
                //为刷新页面设置
                //ConnDesigner.reloadWindow();

                DataProvider provider = dcd.SelectedDataProvider;
                string providerName = provider.Name;
                InArgument<string> providerInarg = providerName;
                ProProvider.SetValue(providerInarg);
            }
        }
    }
}

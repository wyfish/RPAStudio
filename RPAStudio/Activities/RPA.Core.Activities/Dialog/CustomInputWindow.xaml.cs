using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RPA.Core.Activities.DialogActivity.Windows
{
    /// <summary>
    /// CustomInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomInputWindow : Window
    {
        public CustomInputWindow()
        {
            InitializeComponent();
        }

        public void setUrl(string url)
        {
            webBrowser.Navigate(url);
        }

        private string _resultValue=null;
        public void setResult(string _value)
        {
            _resultValue = _value;
            this.Hide();
        }

        public string getResult()
        {
            return _resultValue;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.webBrowser.ObjectForScripting = new OprateBasic(this);
        }
    }

    [System.Runtime.InteropServices.ComVisible(true)]
    public class OprateBasic
    {
        private CustomInputWindow instance;
        public OprateBasic(CustomInputWindow instance)
        {
            this.instance = instance;
        }

        public void setResult(string p)
        {
            instance.setResult(p);
        }
    }
}

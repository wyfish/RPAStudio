using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(SelectDesigner))]
    public sealed class SelectFileActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Select File";
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/selectfile.png";
            }
        }

        private string _filter = "All files (*.*)|*.*";
        [Category("Input")]
        [DisplayNameAttribute("Filter")]
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [DisplayNameAttribute("SelectedFile")]
        public OutArgument<string> SelectedFile { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                if(Filter == "") Filter = "All files (*.*)|*.*";
                ofd.Filter = Filter;
                if (ofd.ShowDialog() == true)
                {
                    SelectedFile.Set(context,ofd.FileName);
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

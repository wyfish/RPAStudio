using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(SelectDesigner))]
    public sealed class SelectFolderActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Select Folder";
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/selectfolder.png";
            }
        }
        
        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [DisplayNameAttribute("SelectedFolder")]
        public OutArgument<string> SelectedFolder { get; set; }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                latch = new CountdownEvent(1);
                string _path = "";
                Thread td = new Thread(() =>
                {
                    System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                    dialog.Description = "请选择文件夹";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _path = dialog.SelectedPath;

                        if (string.IsNullOrEmpty(dialog.SelectedPath))
                        {
                            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "文件夹路径不能为空!");
                            return;
                        }
                        refreshData(latch); 
                    }
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
                SelectedFolder.Set(context, _path);

            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using RPA.Core.Activities.DialogActivity;
using System.Windows.Forms;
using System.Threading;
using Plugins.Shared.Library;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(SelectDesigner))]
    public sealed class SelectSaveAsFileActivity : CodeActivity
    {
        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/selectsaveasfile.png";
            }
        }

        private string _filter = "All files (*.*)|*.*";
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName53")]//保存类型 //Save type //保存タイプ
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

        private string _fileName = "New";

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName54")]//默认文件名 //Default filename //デフォルトのファイル名
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName55")] //另存为位置 //Save as location //名前を付けて保存
        public OutArgument<string> SaveAsFilePath { get; set; }

        //选择保存路径(另存为)
        public bool ShowSaveAsFileDialog(out string user_sel_path, string show_file, string Filter)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = show_file;
            //设置文件类型 
            //Excel表格（*.xls）|*.xls"
            sfd.Filter = Filter;

            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                user_sel_path = sfd.FileName;
                return true;
            }

            user_sel_path = "";
            return false;
        }


        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var latch = new CountdownEvent(1);
                string _path = "";
                Thread td = new Thread(() =>
                {
                    string userSelPath;
                    bool ret = ShowSaveAsFileDialog(out userSelPath, FileName, Filter);

                    if (ret == true)
                    {
                        _path = userSelPath;
                    }

                    latch.Signal();
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();

                SaveAsFilePath.Set(context, _path);

            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }



        }
    }
}

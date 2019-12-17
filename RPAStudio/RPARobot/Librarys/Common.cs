
using GalaSoft.MvvmLight.Threading;
using log4net;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPARobot.Librarys
{
    public class Common
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void RunInUI(Action func)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                func();
            });
        }

        public static void AsyncRun(Action func)
        {
            func.BeginInvoke((ar) =>
            {
                func.EndInvoke(ar);
            },
            null);
        }

        public static void AsyncRun(Func<object> func, Action<object> callBackAction)
        {
            func.BeginInvoke((ar) =>
            {
                var result = func.EndInvoke(ar);
                callBackAction.Invoke(result);
            },
            null);
        }


        public static void LocateDirInExplorer(string dir)
        {
            Process.Start("explorer.exe", dir);
        }

        public static void LocateFileInExplorer(string file)
        {
            Process.Start("explorer.exe", "/select," + file);
        }


        public static bool DeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
                return false;
            }

            return true;
        }

        public static void SetAutoRun(bool isAutoRun)
        {
            //设置是否自动启动
            if (isAutoRun)
            {
                string path = System.Windows.Forms.Application.ExecutablePath;
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
                Microsoft.Win32.RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("RPARobot", path);
                rk2.Close();
                rk.Close();
            }
            else
            {
                string path = System.Windows.Forms.Application.ExecutablePath;
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
                Microsoft.Win32.RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue("RPARobot", false);
                rk2.Close();
                rk.Close();
            }
        }


        //选择保存目录
        private static string defaultSelectDirPath = "";//记录上次选择的目录  
        public static bool ShowSelectDirDialog(string desc, ref string select_dir)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = desc;
            if (defaultSelectDirPath != "")
            {
                dialog.SelectedPath = defaultSelectDirPath;
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    return false;
                }

                select_dir = dialog.SelectedPath;

                defaultSelectDirPath = dialog.SelectedPath;
                return true;
            }

            return false;
        }



        //选择保存路径(另存为)
        public static bool ShowSaveAsFileDialog(out string user_sel_path, string show_file, string filter_ext, string filter_desc)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = show_file;
            //设置文件类型 
            //Excel表格（*.xls）|*.xls"
            sfd.Filter = string.Format("{0}(*{1})|*{1}", filter_desc, filter_ext);

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

        public static List<string> ShowSelectMultiFileDialog(string filter = null, string title = "")
        {
            List<string> fileList = new List<string>();

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true; //可以选择多个文件
            fileDialog.Filter = filter;
            if (!string.IsNullOrEmpty(title))
            {
                fileDialog.Title = title;
            }

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in fileDialog.FileNames)
                {
                    fileList.Add(file);
                }
            }

            return fileList;
        }

        public static string ShowSelectSingleFileDialog(string filter = null, string title = "")
        {
            string select_file = "";

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false; //不可以选择多个文件
            fileDialog.Filter = filter;
            if (!string.IsNullOrEmpty(title))
            {
                fileDialog.Title = title;
            }

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in fileDialog.FileNames)
                {
                    select_file = file;
                }
            }

            return select_file;
        }


        public static string GetProgramVersion()
        {
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath);
            return myFileVersion.FileVersion;
        }
    }

}

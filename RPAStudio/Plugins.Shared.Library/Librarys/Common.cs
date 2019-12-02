using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plugins.Shared.Library.Librarys
{
    public class Common
    {
        public static void RunInUI(Action func)
        {
            System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                func();
            });
        }

        public static bool DeleteDir(string dir)//递归删除目录
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                di.Delete(true);
            }
            catch (Exception )
            {
                return false;
            }

            return true;
        }

        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        public static string MakeRelativePath(string baseDir, string filePath)
        {
            if (string.IsNullOrEmpty(baseDir)) throw new ArgumentNullException("baseDir");
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            if (!baseDir.EndsWith(@"\") && !baseDir.EndsWith(@"/"))
            {
                baseDir += @"\";
            }

            Uri fromUri = new Uri(baseDir);
            Uri toUri = new Uri(filePath);

            if (fromUri.Scheme != toUri.Scheme) { return filePath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }


        public static bool CheckAuthorization(string authorizationFilePath, byte[] publicKey,ref string expiresDate)
        {
            if (!string.IsNullOrEmpty(authorizationFilePath))
            {
                string authString = System.IO.File.ReadAllText(authorizationFilePath);
                JObject authJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(authString) as JObject;

                var signature = authJsonObj["signature"].ToString();
                var data = authJsonObj["data"].ToString();
                var isSignatureOkay = RSACommon.VerifyMessage(publicKey, signature, data);
                if (isSignatureOkay)
                {
                    var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
                    JObject registerObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json) as JObject;
                    if((bool)registerObj["IsRegister"])
                    {
                        //检查当前电脑硬件信息，匹配成功才合法
                        var computer = MyComputerInfo.Instance();

                        if(registerObj["CpuID"].ToString() == computer.CpuID
                            && computer.IsExistMacAddress(registerObj["MacAddress"].ToString())
                            && registerObj["DiskID"].ToString() == computer.DiskID
                            )
                        {
                            expiresDate = registerObj["ExpirationDate"].ToString();
                            return true;
                        }
                    }
                }
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

    }
}

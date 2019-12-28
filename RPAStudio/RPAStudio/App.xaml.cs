using GalaSoft.MvvmLight.Threading;
using log4net;
using Plugins.Shared.Library.Extensions;
using Plugins.Shared.Library.UiAutomation;
using RPAStudio.Librarys;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Reflection;
using Plugins.Shared.Library.Nuget;

namespace RPAStudio
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static App Instance = null;

        private Mutex instanceMutex = null;

        public static string LocalRPAStudioDir { get; set; }

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Instance = this;

            DispatcherHelper.Initialize();

#if DEBUG
            AllocConsole();
#else
            Console.SetOut(new LogToOutputWindowTextWriter());
#endif


            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            clearDynamicActivityGeneratorDll();

            initDirs();
            
            bool createdNew = false;
            instanceMutex = new Mutex(true, "{BCC9EF66-8C2D-44FD-A30F-11ECBD34D993}", out createdNew);
            if (createdNew)
            {
#if ENABLE_AUTHORIZATION_CHECK
                Logger.Debug("RPAStudio启动（带授权检测版本）……", logger);
#else
                Logger.Debug("RPAStudio启动（无授权检测版本）……", logger);
#endif

                UiElement.Init();
            }
            else
            {
                MessageBox.Show("该程序已经运行，不能重复运行！");
                Environment.Exit(0);
            }

        }

        private void initDirs()
        {
            initLocalRPAStudioDir();
            initLogsDir();
            initConfigDir();
            initUpdateDir();
        }

        private void clearDynamicActivityGeneratorDll()
        {
            var files = new DirectoryInfo(System.IO.Path.GetTempPath()).GetFiles("_DynamicActivityGenerator_" + "*");

            // 判断是否只读 并删除
            foreach (var fileInfo in files)
            {
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
                try
                {
                    fileInfo.Delete();
                }
                catch (Exception)
                {
                    //临时文件可能被占用，删除不掉，不处理
                }
                
            }
        }

        private void initUpdateDir()
        {
            var udpateDir = App.LocalRPAStudioDir + @"\Update";

            if (!System.IO.Directory.Exists(udpateDir))
            {
                System.IO.Directory.CreateDirectory(udpateDir);
            }
        }

        private void initLocalRPAStudioDir()
        {
            var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            App.LocalRPAStudioDir = localAppData + @"\RPAStudio";
        }

        private void initLogsDir()
        {
            var logsDir = App.LocalRPAStudioDir + @"\Logs";

            if (!System.IO.Directory.Exists(logsDir))
            {
                System.IO.Directory.CreateDirectory(logsDir);
            }
        }

        private void initConfigDir()
        {
            var configDir = App.LocalRPAStudioDir + @"\Config";

            if (!System.IO.Directory.Exists(configDir))
            {
                System.IO.Directory.CreateDirectory(configDir);
            }

            //以下的XML是用户可能会修改的配置，升级时一般要保留旧数据
            //TODO WJF 后期需要根据XML里的版本号对配置文件数据进行迁移
            CopyResourceToConfig(configDir, "CodeSnippets");
            CopyResourceToConfig(configDir, "FavoriteActivities");
            CopyResourceToConfig(configDir, "FavoriteActivities_en");
            CopyResourceToConfig(configDir, "FavoriteActivities_ja");
            CopyResourceToConfig(configDir, "ProjectUserConfig");
            CopyResourceToConfig(configDir, "RecentActivities");
            CopyResourceToConfig(configDir, "RecentActivities_en");
            CopyResourceToConfig(configDir, "RecentActivities_ja");
            CopyResourceToConfig(configDir, "RecentProjects");

            if (!System.IO.File.Exists(configDir + @"\RPAStudio.settings"))
            {
                byte[] data = RPAStudio.Properties.Resources.RPAStudio_settings;
                System.IO.File.WriteAllBytes(configDir + @"\RPAStudio.settings", data);
            }
            else
            {
                if (UpgradeSettings())
                {
                    Logger.Debug(string.Format("升级xml配置文件 {0} ……", App.LocalRPAStudioDir + @"\Config\RPAStudio.settings"), logger);
                }
            }
        }

        private void CopyResourceToConfig(string configDir, string resourceName)
        {
            string path = string.Format(@"{0}\{1}.xml", configDir, resourceName);
            if (!System.IO.File.Exists(path))
            {
                //byte[] data = RPAStudio.Properties.Resources.FavoriteActivities;
                byte[] data = RPAStudio.Properties.ResourceLocalizer.GetResourceByName(resourceName);
                System.IO.File.WriteAllBytes(path, data);
            }
        }

        private bool UpgradeSettings()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPAStudio.settings";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var schemaVersion = rootNode.GetAttribute("schemaVersion");

            XmlDocument docNew = new XmlDocument();

            using (var ms = new MemoryStream(RPAStudio.Properties.Resources.RPAStudio_settings))
            {
                ms.Flush();
                ms.Position = 0;
                docNew.Load(ms);
                ms.Close();
            }

            var rootNodeNew = docNew.DocumentElement;
            var schemaVersionNew = rootNodeNew.GetAttribute("schemaVersion");

            var schemaVersionTmp = schemaVersion;

            UpgradeSettings(ref schemaVersionTmp, schemaVersionNew);

            if (schemaVersion == schemaVersionTmp)
            {
                return false;
            }
            else
            {
                schemaVersion = schemaVersionTmp;
                return true;
            }
        }

        private void UpgradeSettings(ref string schemaVersion, string schemaVersionNew)
        {
            if (schemaVersion == schemaVersionNew)
            {
                return;
            }

            if (string.IsNullOrEmpty(schemaVersion))
            {
                //从空schemaVersion=>1.0.0
                XmlDocument doc = new XmlDocument();
                var path = App.LocalRPAStudioDir + @"\Config\RPAStudio.settings";
                doc.Load(path);
                var rootNode = doc.DocumentElement;
                rootNode.SetAttribute("schemaVersion", "1.0.0");

                var publishHistoryElement = rootNode.SelectSingleNode("PublishHistory") as XmlElement;

                var lastPublishBrowseInitialFolderElement = publishHistoryElement.SelectSingleNode("LastPublishBrowseInitialFolder") as XmlElement;
                if (lastPublishBrowseInitialFolderElement != null)
                {
                    publishHistoryElement.RemoveChild(lastPublishBrowseInitialFolderElement);
                }

                var lastPublishUriElement = publishHistoryElement.SelectSingleNode("LastPublishUri") as XmlElement;

                if (lastPublishUriElement == null)
                {
                    lastPublishUriElement = doc.CreateElement("LastPublishUri");
                    publishHistoryElement.AppendChild(lastPublishUriElement);
                }

                doc.Save(path);

                schemaVersion = "1.0.0";
            }
            else if (schemaVersion == "1.0.0")
            {
                //TODO WJF 1.0.0=>1.0.1升级
                schemaVersion = "1.0.1";
            }
            else if (schemaVersion == "1.0.1")
            {
                //TODO WJF 1.0.1=>1.0.2升级
                schemaVersion = "1.0.2";
            }

            UpgradeSettings(ref schemaVersion, schemaVersionNew);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //TODO WJF 后期如何处理DLL卸载的问题，是否用AppDomain域进行隔离，如何隔离，其它地方是否修改

            if (args.Name.Contains("_DynamicActivityGenerator_"))
            {
                //_DynamicActivityGenerator_cc0c3cdf-ff42-444e-b8b0-66eb65396c02, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
                var name = args.Name.Split(',')[0];

                var assembly = Assembly.LoadFrom(System.IO.Path.GetTempPath()+ name+".dll");
                return assembly;
            }else
            {
                var name = args.Name.Split(',')[0];

                var path = NuGetPackageController.Instance.TargetFolder + @"\" + name + ".dll";
                if (System.IO.File.Exists(path))
                {
                    var assembly = Assembly.LoadFrom(path);
                    return assembly;
                }
            }

            //System.Console.WriteLine("+++++AssemblyResolve+++++ "+ args.Name);

            return null;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Common.RunInUI(() =>
            {
                try
                {
                    var exception = e.ExceptionObject as Exception;
                    if (exception != null)
                    {
                        Logger.Error("非UI线程全局异常", logger);
                        Logger.Error(exception, logger);
                        MessageBox.Show("程序运行过程中出现了异常，请联系软件开发商！");
                    }

                    if(exception is System.OutOfMemoryException)
                    {
                        Environment.Exit(0);//内存不足直接退出，不然会持续弹窗
                    }
                }
                catch (Exception ex)
                {
                    Logger.Fatal("不可恢复的非UI线程全局异常", logger);
                    Logger.Fatal(ex, logger);
                    MessageBox.Show("程序运行过程中出现了严重错误，即将退出，请联系软件开发商！");

                    Environment.Exit(0);
                }
            });
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Common.RunInUI(() =>
            {
                try
                {
                    Logger.Error("UI线程全局异常", logger);
                    Logger.Error(e.Exception, logger);
                    e.Handled = true;
                    MessageBox.Show("程序运行过程中出现了异常，请联系软件开发商！");

                    if (e.Exception is System.OutOfMemoryException)
                    {
                        Environment.Exit(0);//内存不足直接退出，不然会持续弹窗
                    }
                }
                catch (Exception ex)
                {
                    Logger.Fatal("不可恢复的UI线程全局异常", logger);
                    Logger.Fatal(ex, logger);
                    MessageBox.Show("程序运行过程中出现了严重错误，即将退出，请联系软件开发商！");

                    Environment.Exit(0);
                }
            });
        }

        private void KillProcess(string processName)
        {
            Process[] myproc = Process.GetProcesses();
            foreach (Process item in myproc)
            {
                if (item.ProcessName == processName)
                {
                    item.Kill();
                }
             }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
#if DEBUG
            FreeConsole();
#endif
        }





    }
}

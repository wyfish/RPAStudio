using GalaSoft.MvvmLight.Threading;
using log4net;
using Plugins.Shared.Library.Nuget;
using Plugins.Shared.Library.UiAutomation;
using RPARobot.Librarys;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using RPARobot.ViewModel;

namespace RPARobot
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Mutex instanceMutex = null;

        public static string LocalRPAStudioDir { get; set; }

        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize();

#if DEBUG
            AllocConsole();
#endif

            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            bool createdNew = false;
            instanceMutex = new Mutex(true, "{CB178985-4DD7-41EE-81C5-00FC9A511649}", out createdNew);
            if (createdNew)
            {
#if ENABLE_AUTHORIZATION_CHECK
                Logger.Debug("RPARobot启动（带授权检测版本）……", logger);
#else
                Logger.Debug("RPARobot启动（无授权检测版本）……", logger);
#endif

                UiElement.Init();

                initLocalRPAStudioDir();
                initLogsDir();
                initConfigDir();
                initScreenRecorderDir();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 初始化屏幕录相目录
        /// </summary>
        private void initScreenRecorderDir()
        {
            var screenRecorderDir = LocalRPAStudioDir + @"\ScreenRecorder";

            if (!System.IO.Directory.Exists(screenRecorderDir))
            {
                System.IO.Directory.CreateDirectory(screenRecorderDir);
            }
        }

        /// <summary>
        /// 初始化配置目录
        /// </summary>
        private void initConfigDir()
        {
            var configDir = LocalRPAStudioDir + @"\Config";

            if (!System.IO.Directory.Exists(configDir))
            {
                System.IO.Directory.CreateDirectory(configDir);
            }


            //以下的XML是用户可能会修改的配置，升级时一般要保留旧数据
            //后期需要根据XML里的版本号对配置文件数据进行迁移

            if (!System.IO.File.Exists(configDir + @"\RPARobot.settings"))
            {
                byte[] data = RPARobot.Properties.Resources.RPARobot_settings;
                System.IO.File.WriteAllBytes(configDir + @"\RPARobot.settings", data);
            }
            else
            {
                if (UpgradeSettings())
                {
                    Logger.Debug(string.Format("升级xml配置文件 {0} ……", App.LocalRPAStudioDir + @"\Config\RPARobot.settings"), logger);
                }
            }
        }

        private bool UpgradeSettings()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var schemaVersion = rootNode.GetAttribute("schemaVersion");

            XmlDocument docNew = new XmlDocument();

            using (var ms = new MemoryStream(RPARobot.Properties.Resources.RPARobot_settings))
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

        /// <summary>
        /// 升级设置文件
        /// </summary>
        /// <param name="schemaVersion">schema版本</param>
        /// <param name="schemaVersionNew">新schema版本</param>
        private void UpgradeSettings(ref string schemaVersion, string schemaVersionNew)
        {
            if (schemaVersion == schemaVersionNew)
            {
                return;
            }

            if (schemaVersion == "1.0.0")
            {
                //TODO WJF 1.0.0=>1.0.1升级
                XmlDocument doc = new XmlDocument();
                var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
                doc.Load(path);
                var rootNode = doc.DocumentElement;
                rootNode.SetAttribute("schemaVersion", "1.0.1");

                var userSettingsElement = rootNode.SelectSingleNode("UserSettings") as XmlElement;

                var isEnableScreenRecorderElement = userSettingsElement.SelectSingleNode("IsEnableScreenRecorder") as XmlElement;
                if(isEnableScreenRecorderElement == null)
                {
                    isEnableScreenRecorderElement = doc.CreateElement("IsEnableScreenRecorder");
                    isEnableScreenRecorderElement.InnerText = "False";
                    userSettingsElement.AppendChild(isEnableScreenRecorderElement);
                }

                var fpsElement = userSettingsElement.SelectSingleNode("FPS") as XmlElement;
                if (fpsElement == null)
                {
                    fpsElement = doc.CreateElement("FPS");
                    fpsElement.InnerText = "30";
                    userSettingsElement.AppendChild(fpsElement);
                }

                var qualityElement = userSettingsElement.SelectSingleNode("Quality") as XmlElement;
                if (qualityElement == null)
                {
                    qualityElement = doc.CreateElement("Quality");
                    qualityElement.InnerText = "50";
                    userSettingsElement.AppendChild(qualityElement);
                }

                doc.Save(path);

                schemaVersion = "1.0.1";
            }else if (schemaVersion == "1.0.1")
            {
                //增加了控制中心相关配置
                XmlDocument doc = new XmlDocument();
                var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
                doc.Load(path);
                var rootNode = doc.DocumentElement;
                rootNode.SetAttribute("schemaVersion", "1.0.2");

                var userSettingsElement = rootNode.SelectSingleNode("UserSettings") as XmlElement;

                var isEnableControlServerElement = userSettingsElement.SelectSingleNode("IsEnableControlServer") as XmlElement;
                if (isEnableControlServerElement == null)
                {
                    isEnableControlServerElement = doc.CreateElement("IsEnableControlServer");
                    isEnableControlServerElement.InnerText = "False";
                    userSettingsElement.AppendChild(isEnableControlServerElement);
                }

                var controlServerUriElement = userSettingsElement.SelectSingleNode("ControlServerUri") as XmlElement;
                if (controlServerUriElement == null)
                {
                    controlServerUriElement = doc.CreateElement("ControlServerUri");
                    controlServerUriElement.InnerText = "";
                    userSettingsElement.AppendChild(controlServerUriElement);
                }

                doc.Save(path);

                schemaVersion = "1.0.2";
            }

             UpgradeSettings(ref schemaVersion, schemaVersionNew);
        }

        /// <summary>
        /// 初始化本地RPAStudio目录
        /// </summary>
        private void initLocalRPAStudioDir()
        {
            var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            LocalRPAStudioDir = localAppData + @"\RPAStudio";
        }

        private void initLogsDir()
        {
            var logsDir = LocalRPAStudioDir + @"\Logs";

            if (!System.IO.Directory.Exists(logsDir))
            {
                System.IO.Directory.CreateDirectory(logsDir);
            }
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
                }
                catch (Exception ex)
                {
                    Logger.Fatal("不可恢复的UI线程全局异常", logger);
                    Logger.Fatal(ex, logger);
                }
            });
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
                    }
                }
                catch (Exception ex)
                {
                    Logger.Fatal("不可恢复的非UI线程全局异常", logger);
                    Logger.Fatal(ex, logger);
                }
            });
        }
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //TODO WJF 后期如何处理DLL卸载的问题，是否用AppDomain域进行隔离，如何隔离，其它地方是否修改

            if (args.Name.Contains("_DynamicActivityGenerator_"))
            {
                //_DynamicActivityGenerator_cc0c3cdf-ff42-444e-b8b0-66eb65396c02, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
                var name = args.Name.Split(',')[0];

                var assembly = Assembly.LoadFrom(System.IO.Path.GetTempPath() + name + ".dll");
                return assembly;
            }
            else
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



        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (ViewModelLocator.Instance.Main.FFmpegService != null)
            {
                ViewModelLocator.Instance.Main.FFmpegService.StopCaptureScreen();
                ViewModelLocator.Instance.Main.FFmpegService = null;
            }
            
#if DEBUG
            FreeConsole();
#endif
        }




    }
}

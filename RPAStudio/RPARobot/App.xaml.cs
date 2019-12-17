using GalaSoft.MvvmLight.Threading;
using log4net;
using Plugins.Shared.Library.UiAutomation;
using RPARobot.Librarys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void initConfigDir()
        {
            var configDir = LocalRPAStudioDir + @"\Config";

            if (!System.IO.Directory.Exists(configDir))
            {
                System.IO.Directory.CreateDirectory(configDir);
            }


            //以下的XML是用户可能会修改的配置，升级时一般要保留旧数据
            //TODO WJF 后期需要根据XML里的版本号对配置文件数据进行迁移

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
            //TODO WJF 后期可能的配置文件升级，参考RPAStudio的升级方式即可
            return false;
        }

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




        private void Application_Exit(object sender, ExitEventArgs e)
        {
#if DEBUG
            FreeConsole();
#endif
        }




    }
}

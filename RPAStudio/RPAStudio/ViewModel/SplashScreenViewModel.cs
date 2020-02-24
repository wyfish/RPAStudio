using System;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using RPAStudio.Windows;
using RPAStudio.Librarys;
using RPAStudio.Localization;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SplashScreenViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Window m_view;

        /// <summary>
        /// Initializes a new instance of the SplashScreenViewModel class.
        /// </summary>
        public SplashScreenViewModel()
        {
        }

        private RelayCommand<RoutedEventArgs> _loadedCommand;

        /// <summary>
        /// Gets the LoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        Logger.Debug("+++++SplashScreen窗口启动+++++", logger);
                        m_view = (Window)p.Source;

                        Init();
                    }));
            }
        }


        private void Init()
        {
            Version = Common.GetProgramVersion();
            Task.Run(() =>
            {
                //授权检测
                DoAuthorizationCheck();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = new MainWindow();
                    App.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    m_view.Close();
                });
            });
        }

        /// <summary>
        /// The <see cref="Version" /> property's name.
        /// </summary>
        public const string VersionPropertyName = "Version";

        private string _versionProperty = "";

        /// <summary>
        /// Sets and gets the Version property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Version
        {
            get
            {
                return _versionProperty;
            }

            set
            {
                if (_versionProperty == value)
                {
                    return;
                }

                _versionProperty = value;
                RaisePropertyChanged(VersionPropertyName);
            }
        }


        public void DoAuthorizationCheck()
        {
            //授权检测
            if (!IsNotExpired())
            {
                // 软件未通过授权检测，请注册产品！
                var tip = ResxIF.GetString("TheSoftwareFailedAuthorizationTest");
                Logger.Debug(tip, logger);
                MessageBox.Show(tip, ResxIF.GetString("PronptText"), MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return;
            }
        }


        public bool IsNotExpired()
        {
#if ENABLE_AUTHORIZATION_CHECK
            string expiresDate = "";
            return IsNotExpired(ref expiresDate);
#else
            return true;
#endif
        }

        private bool IsNotExpired(ref string expiresDate)
        {
            var fileFullPath = App.LocalRPAStudioDir + @"\Authorization\license.authorization";

            return IsNotExpired(fileFullPath, ref expiresDate);
        }


        private bool IsNotExpired(string fileFullPath, ref string expiresDate)
        {
            bool isNotExpired = false;

            if (!System.IO.File.Exists(fileFullPath))
            {
                return isNotExpired;
            }

            if (Plugins.Shared.Library.Librarys.Common.CheckAuthorization(fileFullPath, RPAStudio.Properties.Resources.verify_public_rsa, ref expiresDate))
            {
                //授权合法，检查下有效期
                if (expiresDate == "forever")
                {
                    isNotExpired = true;
                }
                else
                {
                    DateTime current = DateTime.Now;
                    DateTime deadline = Convert.ToDateTime(expiresDate).AddDays(1);//截止日期得再加上一天，因为从当天00:00:00截止
                    if (current.CompareTo(deadline) < 0)
                    {
                        isNotExpired = true;
                    }
                }
            }

            return isNotExpired;
        }

        public string SplashImagePath {
            get {
                //return "pack://application:,,,/Resources/Image/Windows/SplashScreen/startup.png";
                if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("zh"))
                {
                    return "pack://application:,,,/Resources/Image/Windows/SplashScreen/startup.png";
                }
                return "pack://application:,,,/Resources/Image/Windows/SplashScreen/startup_en.png";
            }
        }





    }
}
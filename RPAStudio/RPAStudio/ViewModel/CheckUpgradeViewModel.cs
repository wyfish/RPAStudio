using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using RPAStudio.Librarys;
using RPAStudio.Localization;
using RPAUpdate.Librarys;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckUpgradeViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpDownloadFile m_downloader { get; set; }

        //RPAUpgradeClientConfig.xml相关配置
        private string m_rpaUpgradeServerConfigUrl { get; set; }
        private List<string> m_currentVersionUpdateLogList { get; set; } = new List<string>();

        //RPAUpgradeServerConfig.xml相关配置
        private string m_autoUpgradePackpageVersion { get; set; }
        private string m_autoUpgradePackpageMd5 { get; set; }
        private string m_autoUpgradePackpageUrl { get; set; }
        private List<string> m_latestVersionUpdateLogList { get; set; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the CheckUpgradeViewModel class.
        /// </summary>
        public CheckUpgradeViewModel()
        {
            initRPAUpgradeClientConfig();

            initCurrentVersionInfo();
        }

        private void OnDownloadFinished(HttpDownloadFile obj)
        {
            Common.RunInUI(() => {
                if(obj.IsDownloadSuccess)
                {
                    var fileMd5 = Common.GetMD5HashFromFile(obj.SaveFilePath);
                    if (m_autoUpgradePackpageMd5.ToLower() == fileMd5.ToLower())
                    {
                        //文件MD5校验一致，说明包下载成功且未被破坏，提示用户安装
                        var saveDir = Path.GetDirectoryName(obj.SaveFilePath);

                        var originFileName = Common.GetFileNameFromUrl(obj.Url);

                        var finishedFilePath = saveDir + @"\" + originFileName;

                        if (File.Exists(finishedFilePath))
                        {
                            File.Delete(finishedFilePath);
                        }

                        File.Move(obj.SaveFilePath, finishedFilePath);

                        if (!File.Exists(finishedFilePath))
                        {
                            //重命名失败
                            Logger.Error(string.Format("重命名{0}到{1}失败", obj.SaveFilePath, finishedFilePath), logger);

                            MessageBox.Show(App.Current.MainWindow, "升级包重命名操作出现异常，请检查！", ResxIF.GetString("ErrorText"), MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            doInstallUpgradePackage(finishedFilePath);
                        }
                    }
                    else
                    {
                        //安装包下载出现问题（可能安装包之前残留的断点数据不对，删除这个文件），提示用户重试
                        if (File.Exists(obj.SaveFilePath))
                        {
                            File.Delete(obj.SaveFilePath);
                        }

                        MessageBox.Show(App.Current.MainWindow, "升级包MD5校验不一致，请重试！", ResxIF.GetString("ErrorText"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(App.Current.MainWindow, "下载过程中出现异常，请检查并重试！", ResxIF.GetString("ErrorText"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            });
            
        }


        private void OnDownloading(HttpDownloadFile obj)
        {
            //进度条控制
            DownloadingProgressValue = (int)(100.0* obj.FileDownloadedBytes/obj.FileTotalBytes);
        }

        private void OnRunningChanged(HttpDownloadFile obj)
        {
            Common.RunInUI(() => {
                if(obj.IsRunning)
                {
                    IsShowProgressBar = true;
                }

                IsDoUpgradeEnable = !obj.IsRunning;
                DoUpgradeCommand.RaiseCanExecuteChanged();
            });
        }


        private void doInstallUpgradePackage(string upgradePackageFilePath)
        {
            //弹窗提示用户是否现在立刻更新程序
            var ret = MessageBox.Show(App.Current.MainWindow, "升级包已经下载好，是否现在更新？", ResxIF.GetString("ConfirmText"), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (ret == MessageBoxResult.Yes)
            {
                bool isDocumentNeedSave = false;
                foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
                {
                   if(doc.IsDirty)
                    {
                        isDocumentNeedSave = true;
                        break;
                    }
                }

                if(isDocumentNeedSave)
                {
                    MessageBox.Show(App.Current.MainWindow, "您有文档修改但未保存，请保存文档后再升级", ResxIF.GetString("PronptText"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Common.ShellExecute(upgradePackageFilePath);
                }
                
            }
            else if (ret == MessageBoxResult.No)
            {

            }
        }

        

        private void initRPAUpgradeClientConfig()
        {
            m_currentVersionUpdateLogList.Clear();

            XmlDocument doc = new XmlDocument();

            using (var ms = new MemoryStream(RPAStudio.Properties.Resources.RPAUpgradeClientConfig))
            {
                ms.Flush();
                ms.Position = 0;
                doc.Load(ms);
                ms.Close();
            }

            var rootNode = doc.DocumentElement;
            var rpaUpgradeServerConfigElement = rootNode.SelectSingleNode("RPAUpgradeServerConfig") as XmlElement;
            m_rpaUpgradeServerConfigUrl = rpaUpgradeServerConfigElement.GetAttribute("Url");

            var updateLogElement = rootNode.SelectSingleNode("UpdateLog");
            var items = updateLogElement.SelectNodes("Item");
            foreach(var item in items)
            {
                var text = (item as XmlElement).InnerText;
                m_currentVersionUpdateLogList.Add(text);
            }
        }

        private bool initRPAUpgradeServerConfig()
        {
            bool ret = true;
            var rpaUpgradeServerConfig = HttpRequest.Get(m_rpaUpgradeServerConfigUrl);

            if(!string.IsNullOrEmpty(rpaUpgradeServerConfig))
            {
                m_latestVersionUpdateLogList.Clear();

                XmlDocument doc = new XmlDocument();

                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rpaUpgradeServerConfig)))
                {
                    ms.Flush();
                    ms.Position = 0;
                    doc.Load(ms);
                    ms.Close();
                }

                var rootNode = doc.DocumentElement;
                var autoUpgradePackpageElement = rootNode.SelectSingleNode("AutoUpgradePackpage") as XmlElement;
                
                m_autoUpgradePackpageVersion = autoUpgradePackpageElement.GetAttribute("Version");
                m_autoUpgradePackpageMd5 = autoUpgradePackpageElement.GetAttribute("Md5");
                m_autoUpgradePackpageUrl = autoUpgradePackpageElement.GetAttribute("Url");

                var updateLogElement = rootNode.SelectSingleNode("UpdateLog");
                var items = updateLogElement.SelectNodes("Item");
                foreach (var item in items)
                {
                    var text = (item as XmlElement).InnerText;
                    m_latestVersionUpdateLogList.Add(text);
                }
            }
            else
            {
                ret = false;
                IsCheckUpgradeSuccess = false;
                Logger.Error("获取升级配置文件失败，请检查!url="+ m_rpaUpgradeServerConfigUrl, logger);
            }

            return ret; 
        }

        private void initCurrentVersionInfo()
        {
            CurrentVersionName = "v"+Common.GetProgramVersion();

            CurrentVersionUpdateLog = "";
            foreach (var item in m_currentVersionUpdateLogList)
            {
                CurrentVersionUpdateLog += " ● " + item + Environment.NewLine;
            }
           
        }

        private void initLatestVersionInfo()
        {
            Version currentVersion = new Version(Common.GetProgramVersion());
            Version latestVersion = new Version(m_autoUpgradePackpageVersion);

            if(latestVersion > currentVersion)
            {
                IsNeedUpgrade = true;
                LatestVersionName = "v" + m_autoUpgradePackpageVersion;

                LatestVersionUpdateLog = "";
                foreach (var item in m_latestVersionUpdateLogList)
                {
                    LatestVersionUpdateLog += " ● " + item + Environment.NewLine;
                }
            }
            else
            {
                IsNeedUpgrade = false;
            }

        }


        /// <summary>
        /// The <see cref="DownloadingProgressValue" /> property's name.
        /// </summary>
        public const string DownloadingProgressValuePropertyName = "DownloadingProgressValue";

        private int _downloadingProgressValueProperty = 0;

        /// <summary>
        /// Sets and gets the DownloadingProgressValue property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int DownloadingProgressValue
        {
            get
            {
                return _downloadingProgressValueProperty;
            }

            set
            {
                if (_downloadingProgressValueProperty == value)
                {
                    return;
                }

                _downloadingProgressValueProperty = value;
                RaisePropertyChanged(DownloadingProgressValuePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsShowProgressBar" /> property's name.
        /// </summary>
        public const string IsShowProgressBarPropertyName = "IsShowProgressBar";

        private bool _isShowProgressBarProperty = false;

        /// <summary>
        /// Sets and gets the IsShowProgressBar property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowProgressBar
        {
            get
            {
                return _isShowProgressBarProperty;
            }

            set
            {
                if (_isShowProgressBarProperty == value)
                {
                    return;
                }

                _isShowProgressBarProperty = value;
                RaisePropertyChanged(IsShowProgressBarPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsDoUpgradeEnable" /> property's name.
        /// </summary>
        public const string IsDoUpgradeEnablePropertyName = "IsDoUpgradeEnable";

        private bool _isDoUpgradeEnableProperty = true;

        /// <summary>
        /// Sets and gets the IsDoUpgradeEnable property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDoUpgradeEnable
        {
            get
            {
                return _isDoUpgradeEnableProperty;
            }

            set
            {
                if (_isDoUpgradeEnableProperty == value)
                {
                    return;
                }

                _isDoUpgradeEnableProperty = value;
                RaisePropertyChanged(IsDoUpgradeEnablePropertyName);
            }
        }


        /// <summary>
        /// 是否需要升级
        /// </summary>
        public const string IsNeedUpgradePropertyName = "IsNeedUpgrade";

        private bool _isNeedUpgradeProperty = false;

        /// <summary>
        /// Sets and gets the IsNeedUpgrade property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsNeedUpgrade
        {
            get
            {
                return _isNeedUpgradeProperty;
            }

            set
            {
                if (_isNeedUpgradeProperty == value)
                {
                    return;
                }

                _isNeedUpgradeProperty = value;
                RaisePropertyChanged(IsNeedUpgradePropertyName);

                ViewModelLocator.Instance.Main.IsNeedUpgrade = value;
            }
        }


        /// <summary>
        /// The <see cref="IsShowCurrentVersionUpdateLog" /> property's name.
        /// </summary>
        public const string IsShowCurrentVersionUpdateLogPropertyName = "IsShowCurrentVersionUpdateLog";

        private bool _isShowCurrentVersionUpdateLogProperty = true;

        /// <summary>
        /// Sets and gets the IsShowCurrentVersionUpdateLog property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowCurrentVersionUpdateLog
        {
            get
            {
                return _isShowCurrentVersionUpdateLogProperty;
            }

            set
            {
                if (_isShowCurrentVersionUpdateLogProperty == value)
                {
                    return;
                }

                _isShowCurrentVersionUpdateLogProperty = value;
                RaisePropertyChanged(IsShowCurrentVersionUpdateLogPropertyName);

                IsShowLatestVersionUpdateLog = !value;//互斥显示
            }
        }


        /// <summary>
        /// The <see cref="IsShowLatestVersionUpdateLog" /> property's name.
        /// </summary>
        public const string IsShowLatestVersionUpdateLogPropertyName = "IsShowLatestVersionUpdateLog";

        private bool _isShowLatestVersionUpdateLogProperty = false;

        /// <summary>
        /// Sets and gets the IsShowLatestVersionUpdateLog property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowLatestVersionUpdateLog
        {
            get
            {
                return _isShowLatestVersionUpdateLogProperty;
            }

            set
            {
                if (_isShowLatestVersionUpdateLogProperty == value)
                {
                    return;
                }

                _isShowLatestVersionUpdateLogProperty = value;
                RaisePropertyChanged(IsShowLatestVersionUpdateLogPropertyName);

                IsShowCurrentVersionUpdateLog = !value;//互斥显示
            }
        }

        /// <summary>
        /// The <see cref="CurrentVersionName" /> property's name.
        /// </summary>
        public const string CurrentVersionNamePropertyName = "CurrentVersionName";

        private string _currentVersionNameProperty = "";

        /// <summary>
        /// Sets and gets the CurrentVersionName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentVersionName
        {
            get
            {
                return _currentVersionNameProperty;
            }

            set
            {
                if (_currentVersionNameProperty == value)
                {
                    return;
                }

                _currentVersionNameProperty = value;
                RaisePropertyChanged(CurrentVersionNamePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="LatestVersionName" /> property's name.
        /// </summary>
        public const string LatestVersionNamePropertyName = "LatestVersionName";

        private string _latestVersionNameProperty = "";

        /// <summary>
        /// Sets and gets the LatestVersionName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LatestVersionName
        {
            get
            {
                return _latestVersionNameProperty;
            }

            set
            {
                if (_latestVersionNameProperty == value)
                {
                    return;
                }

                _latestVersionNameProperty = value;
                RaisePropertyChanged(LatestVersionNamePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="CurrentVersionUpdateLog" /> property's name.
        /// </summary>
        public const string CurrentVersionUpdateLogPropertyName = "CurrentVersionUpdateLog";

        private string _currentVersionUpdateLogProperty = "";

        /// <summary>
        /// Sets and gets the CurrentVersionUpdateLog property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentVersionUpdateLog
        {
            get
            {
                return _currentVersionUpdateLogProperty;
            }

            set
            {
                if (_currentVersionUpdateLogProperty == value)
                {
                    return;
                }

                _currentVersionUpdateLogProperty = value;
                RaisePropertyChanged(CurrentVersionUpdateLogPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="LatestVersionUpdateLog" /> property's name.
        /// </summary>
        public const string LatestVersionUpdateLogPropertyName = "LatestVersionUpdateLog";

        private string _latestVersionUpdateLogProperty = "";

        /// <summary>
        /// Sets and gets the LatestVersionUpdateLog property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LatestVersionUpdateLog
        {
            get
            {
                return _latestVersionUpdateLogProperty;
            }

            set
            {
                if (_latestVersionUpdateLogProperty == value)
                {
                    return;
                }

                _latestVersionUpdateLogProperty = value;
                RaisePropertyChanged(LatestVersionUpdateLogPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsCheckUpgradeSuccess" /> property's name.
        /// </summary>
        public const string IsCheckUpgradeSuccessPropertyName = "IsCheckUpgradeSuccess";

        private bool _isCheckUpgradeSuccessProperty = true;

        /// <summary>
        /// Sets and gets the IsCheckUpgradeSuccess property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsCheckUpgradeSuccess
        {
            get
            {
                return _isCheckUpgradeSuccessProperty;
            }

            set
            {
                if (_isCheckUpgradeSuccessProperty == value)
                {
                    return;
                }

                _isCheckUpgradeSuccessProperty = value;
                RaisePropertyChanged(IsCheckUpgradeSuccessPropertyName);
            }
        }



        private RelayCommand _showCurrentVersionUpdateLogCommand;

        /// <summary>
        /// Gets the ShowCurrentVersionUpdateLogCommand.
        /// </summary>
        public RelayCommand ShowCurrentVersionUpdateLogCommand
        {
            get
            {
                return _showCurrentVersionUpdateLogCommand
                    ?? (_showCurrentVersionUpdateLogCommand = new RelayCommand(
                    () =>
                    {
                        IsShowCurrentVersionUpdateLog = true;
                    }));
            }
        }



        private RelayCommand _showLatestVersionUpdateLogCommand;

        /// <summary>
        /// Gets the ShowLatestVersionUpdateLogCommand.
        /// </summary>
        public RelayCommand ShowLatestVersionUpdateLogCommand
        {
            get
            {
                return _showLatestVersionUpdateLogCommand
                    ?? (_showLatestVersionUpdateLogCommand = new RelayCommand(
                    () =>
                    {
                        IsShowLatestVersionUpdateLog = true;
                    }));
            }
        }


        


        private RelayCommand _doUpgradeCommand;

        /// <summary>
        /// Gets the DoUpgradeCommand.
        /// </summary>
        public RelayCommand DoUpgradeCommand
        {
            get
            {
                return _doUpgradeCommand
                    ?? (_doUpgradeCommand = new RelayCommand(
                    () =>
                    {
                        //判断升级包是否已经下载好了，如果已经下载，则直接安装
                        bool hasDownload = false;
                        var originFileName = Common.GetFileNameFromUrl(m_autoUpgradePackpageUrl);
                        var path = App.LocalRPAStudioDir + string.Format(@"\Update\{0}", originFileName);
                        if(File.Exists(path) && Common.GetMD5HashFromFile(path).ToLower() == m_autoUpgradePackpageMd5.ToLower())
                        {
                            hasDownload = true;
                        }

                        if(hasDownload)
                        {
                            IsShowProgressBar = true;
                            DownloadingProgressValue = 100;

                            doInstallUpgradePackage(path);
                        }
                        else
                        {
                            IsShowProgressBar = false;
                            var thread = new Thread(downloadThread);//创建一个线程
                            thread.Start();//开始一个线程
                        }
                    },
                    () => IsDoUpgradeEnable));
            }
        }









        private void downloadThread()
        {
            //执行升级，后台下载(断点下载)，下载完成后判断MD5是否合法，合法的话弹窗提示用户安装
            var path = App.LocalRPAStudioDir + string.Format(@"\Update\{0}.exe", m_autoUpgradePackpageMd5);

            if(m_downloader != null)
            {
                m_downloader.Stop();

                if(m_downloader.IsRunning)
                {
                    Thread.Sleep(500);
                }
            }

            m_downloader = new HttpDownloadFile();
            m_downloader.OnRunningChanged = OnRunningChanged;
            m_downloader.OnDownloadFinished = OnDownloadFinished;
            m_downloader.OnDownloading = OnDownloading;
            m_downloader.Download(m_autoUpgradePackpageUrl, path);
        }

      
        private RelayCommand _checkUpgradeCommand;

        /// <summary>
        /// Gets the CheckUpgradeCommand.
        /// </summary>
        public RelayCommand CheckUpgradeCommand
        {
            get
            {
                return _checkUpgradeCommand
                    ?? (_checkUpgradeCommand = new RelayCommand(
                    () =>
                    {
                        //检查是否要更新
                        Task.Run(() =>
                        {
                            if (initRPAUpgradeServerConfig())
                            {
                                initLatestVersionInfo();

                                if (IsNeedUpgrade)
                                {
                                    IsShowLatestVersionUpdateLog = true;
                                }
                            }
                        });
                       
                    }));
            }
        }

        
    }
}
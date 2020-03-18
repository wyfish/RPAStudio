using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RPARobot.Librarys;
using System.Windows;
using System.Xml;
using System;
using System.ComponentModel;
using RPARobot.Services;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserPreferencesViewModel : ViewModelBase
    {
        public Window m_view { get; set; }

        /// <summary>
        /// Initializes a new instance of the UserPreferencesViewModel class.
        /// </summary>
        public UserPreferencesViewModel()
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
                        m_view = (Window)p.Source;
                    }));
            }
        }

        public void LoadSettings()
        {
            //读取配置XML文件来初始化界面信息
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
            doc.Load(path);
            var rootNode = doc.DocumentElement;
            var userSettingsElement = rootNode.SelectSingleNode("UserSettings") as XmlElement;

            //根据自启动项设置IsAutoRun值
            var isAutoRunElement = userSettingsElement.SelectSingleNode("IsAutoRun") as XmlElement;
            if (isAutoRunElement.InnerText.ToLower().Trim() == "true")
            {
                //注册表设置自启动
                Common.SetAutoRun(true);
                this.IsAutoRun = true;
            }
            else
            {
                ////注册表取消自启动
                Common.SetAutoRun(false);
                this.IsAutoRun = false;
            }

            //根据配置XML信息设置IsAutoOpenMainWindow
            var isAutoOpenMainWindowElement = userSettingsElement.SelectSingleNode("IsAutoOpenMainWindow") as XmlElement;
            if (isAutoOpenMainWindowElement.InnerText.ToLower().Trim() == "true")
            {
                this.IsAutoOpenMainWindow = true;
            }
            else
            {
                this.IsAutoOpenMainWindow = false;
            }


            var isEnableScreenRecorderElement = userSettingsElement.SelectSingleNode("IsEnableScreenRecorder") as XmlElement;
            if (isEnableScreenRecorderElement.InnerText.ToLower().Trim() == "true")
            {
                this.IsEnableScreenRecorder = true;
            }
            else
            {
                this.IsEnableScreenRecorder = false;
            }

            var fpsElement = userSettingsElement.SelectSingleNode("FPS") as XmlElement;
            this.FPS = fpsElement.InnerText.Trim();


            var qualityElement = userSettingsElement.SelectSingleNode("Quality") as XmlElement;
            this.Quality = qualityElement.InnerText.Trim();


            var isEnableControlServerElement = userSettingsElement.SelectSingleNode("IsEnableControlServer") as XmlElement;
            if (isEnableControlServerElement.InnerText.ToLower().Trim() == "true")
            {
                this.IsEnableControlServer = true;
            }
            else
            {
                this.IsEnableControlServer = false;
            }

            var controlServerUriElement = userSettingsElement.SelectSingleNode("ControlServerUri") as XmlElement;
            this.ControlServerUri = controlServerUriElement.InnerText.Trim();
        }

        private RelayCommand _MouseLeftButtonDownCommand;

        /// <summary>
        /// Gets the MouseLeftButtonDownCommand.
        /// </summary>
        public RelayCommand MouseLeftButtonDownCommand
        {
            get
            {
                return _MouseLeftButtonDownCommand
                    ?? (_MouseLeftButtonDownCommand = new RelayCommand(
                    () =>
                    {
                        //点标题外的部分也能拖动，方便使用
                        m_view.DragMove();
                    }));
            }
        }



        private RelayCommand<CancelEventArgs> _closingCommand;

        /// <summary>
        /// Gets the ClosingCommand.
        /// </summary>
        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand
                    ?? (_closingCommand = new RelayCommand<CancelEventArgs>(
                    e =>
                    {
                        e.Cancel = true;//不关闭窗口
                        m_view.Hide();
                    }));
            }
        }

        /// <summary>
        /// The <see cref="IsAutoRun" /> property's name.
        /// </summary>
        public const string IsAutoRunPropertyName = "IsAutoRun";

        private bool _isAutoRunProperty = false;

        /// <summary>
        /// Sets and gets the IsAutoRun property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAutoRun
        {
            get
            {
                return _isAutoRunProperty;
            }

            set
            {
                if (_isAutoRunProperty == value)
                {
                    return;
                }

                _isAutoRunProperty = value;
                RaisePropertyChanged(IsAutoRunPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsAutoOpenMainWindow" /> property's name.
        /// </summary>
        public const string IsAutoOpenMainWindowPropertyName = "IsAutoOpenMainWindow";

        private bool _isAutoOpenMainWindowProperty = false;

        /// <summary>
        /// Sets and gets the IsAutoOpenMainWindow property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAutoOpenMainWindow
        {
            get
            {
                return _isAutoOpenMainWindowProperty;
            }

            set
            {
                if (_isAutoOpenMainWindowProperty == value)
                {
                    return;
                }

                _isAutoOpenMainWindowProperty = value;
                RaisePropertyChanged(IsAutoOpenMainWindowPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsEnableScreenRecorder" /> property's name.
        /// </summary>
        public const string IsEnableScreenRecorderPropertyName = "IsEnableScreenRecorder";

        private bool _isEnableScreenRecorderProperty = false;

        /// <summary>
        /// 是否启用屏幕录像
        /// </summary>
        public bool IsEnableScreenRecorder
        {
            get
            {
                return _isEnableScreenRecorderProperty;
            }

            set
            {
                if (_isEnableScreenRecorderProperty == value)
                {
                    return;
                }

                _isEnableScreenRecorderProperty = value;
                RaisePropertyChanged(IsEnableScreenRecorderPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="FPS" /> property's name.
        /// </summary>
        public const string FPSPropertyName = "FPS";

        private string _fpsProperty = "0";

        /// <summary>
        /// 录像的FPS
        /// </summary>
        public string FPS
        {
            get
            {
                return _fpsProperty;
            }

            set
            {
                if (_fpsProperty == value)
                {
                    return;
                }

                _fpsProperty = value;
                RaisePropertyChanged(FPSPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Quality" /> property's name.
        /// </summary>
        public const string QualityPropertyName = "Quality";

        private string _qualityProperty = "0";

        /// <summary>
        /// 录像质量
        /// </summary>
        public string Quality
        {
            get
            {
                return _qualityProperty;
            }

            set
            {
                if (_qualityProperty == value)
                {
                    return;
                }

                _qualityProperty = value;
                RaisePropertyChanged(QualityPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsEnableControlServer" /> property's name.
        /// </summary>
        public const string IsEnableControlServerPropertyName = "IsEnableControlServer";

        private bool _isEnableControlServerProperty = false;

        /// <summary>
        /// 是否启用控制中心
        /// </summary>
        public bool IsEnableControlServer
        {
            get
            {
                return _isEnableControlServerProperty;
            }

            set
            {
                if (_isEnableControlServerProperty == value)
                {
                    return;
                }

                _isEnableControlServerProperty = value;
                RaisePropertyChanged(IsEnableControlServerPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="ControlServerUri" /> property's name.
        /// </summary>
        public const string ControlServerUriPropertyName = "ControlServerUri";

        private string _controlServerUriProperty = "";

        /// <summary>
        /// 控制中心地址
        /// </summary>
        public string ControlServerUri
        {
            get
            {
                return _controlServerUriProperty;
            }

            set
            {
                if (_controlServerUriProperty == value)
                {
                    return;
                }

                _controlServerUriProperty = value;
                RaisePropertyChanged(ControlServerUriPropertyName);
            }
        }




        /// <summary>
        /// 升级配置文件
        /// </summary>
        private void UpdateSettings()
        {
            //写入到配置XML中，执行可能的相应的操作
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
            doc.Load(path);
            var rootNode = doc.DocumentElement;
            var userSettingsElement = rootNode.SelectSingleNode("UserSettings") as XmlElement;

            //根据自启动项设置IsAutoRun值
            var isAutoRunElement = userSettingsElement.SelectSingleNode("IsAutoRun") as XmlElement;
            isAutoRunElement.InnerText = IsAutoRun ? "True" : "False";

            var isAutoOpenMainWindowElement = userSettingsElement.SelectSingleNode("IsAutoOpenMainWindow") as XmlElement;
            isAutoOpenMainWindowElement.InnerText = IsAutoOpenMainWindow ? "True" : "False";

            var isEnableScreenRecorderElement = userSettingsElement.SelectSingleNode("IsEnableScreenRecorder") as XmlElement;
            isEnableScreenRecorderElement.InnerText = IsEnableScreenRecorder ? "True" : "False";

            var fpsElement = userSettingsElement.SelectSingleNode("FPS") as XmlElement;
            fpsElement.InnerText = FPS;

            var qualityElement = userSettingsElement.SelectSingleNode("Quality") as XmlElement;
            qualityElement.InnerText = Quality;

            var isEnableControlServerElement = userSettingsElement.SelectSingleNode("IsEnableControlServer") as XmlElement;
            isEnableControlServerElement.InnerText = IsEnableControlServer ? "True" : "False";

            var controlServerUriElement = userSettingsElement.SelectSingleNode("ControlServerUri") as XmlElement;
            controlServerUriElement.InnerText = ControlServerUri;

            doc.Save(path);

            Common.SetAutoRun(IsAutoRun);
        }



        private RelayCommand _resetSettingsCommand;

        /// <summary>
        /// Gets the ResetSettingsCommand.
        /// </summary>
        public RelayCommand ResetSettingsCommand
        {
            get
            {
                return _resetSettingsCommand
                    ?? (_resetSettingsCommand = new RelayCommand(
                    () =>
                    {
                        //重置默认设置
                        //确认重置为默认设置吗？ 询问
                        var ret = AutoCloseMessageBoxService.Show(m_view, Localization.ResxIF.GetString("AreYouSureToReset"), Localization.ResxIF.GetString("ConfirmText"), MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                        if (ret == MessageBoxResult.OK)
                        {
                            IsAutoRun = false;
                            IsAutoOpenMainWindow = true;

                            IsEnableScreenRecorder = false;
                            FPS = "30";
                            Quality = "50";

                            IsEnableControlServer = false;
                            ControlServerUri = "";

                            //立即生效
                            UpdateSettings();
                        }
                        
                    }));
            }
        }



        private RelayCommand _okCommand;

        /// <summary>
        /// Gets the OkCommand.
        /// </summary>
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand(
                    () =>
                    {
                        UpdateSettings();

                        m_view.Close();
                    }));
            }
        }


        private RelayCommand _cancelCommand;

        /// <summary>
        /// Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand(
                    () =>
                    {
                        m_view.Close();
                    }));
            }
        }






    }
}
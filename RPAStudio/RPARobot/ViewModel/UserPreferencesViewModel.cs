using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RPARobot.Librarys;
using System.Windows;
using System.Xml;
using System;

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



        private RelayCommand<System.ComponentModel.CancelEventArgs> _closingCommand;

        /// <summary>
        /// Gets the ClosingCommand.
        /// </summary>
        public RelayCommand<System.ComponentModel.CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand
                    ?? (_closingCommand = new RelayCommand<System.ComponentModel.CancelEventArgs>(
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
                        var ret = MessageBox.Show(m_view, "确认重置为默认设置吗？", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                        if (ret == MessageBoxResult.OK)
                        {
                            IsAutoRun = false;
                            IsAutoOpenMainWindow = true;

                            //立即生效
                            OkCommand.Execute(null);
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

                        doc.Save(path);

                        Common.SetAutoRun(IsAutoRun);

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
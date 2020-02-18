using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.ComponentModel;
using RPARobot.Services;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// 关于窗口的视图模型
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// 对应的视图
        /// </summary>
        public Window m_view { get; set; }

        /// <summary>
        /// Initializes a new instance of the AboutViewModel class.
        /// </summary>
        public AboutViewModel()
        {
        }

        /// <summary>
        /// 加载关于信息
        /// </summary>
        public void LoadAboutInfo()
        {
            var serv = new AboutInfoService();

            MachineName = serv.GetMachineName();
            IpAddress = serv.GetIp();
            ProgramVersion = serv.GetVersion();
        }

        private RelayCommand<RoutedEventArgs> _loadedCommand;

        /// <summary>
        /// 窗体加载完成后调用
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

        private RelayCommand _MouseLeftButtonDownCommand;

        /// <summary>
        /// 鼠标左键按下处理
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
        /// 窗体即将关闭时触发
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
        /// The <see cref="MachineName" /> property's name.
        /// </summary>
        public const string MachineNamePropertyName = "MachineName";

        private string _machineNameProperty = "";

        /// <summary>
        /// 机器名
        /// </summary>
        public string MachineName
        {
            get
            {
                return _machineNameProperty;
            }

            set
            {
                if (_machineNameProperty == value)
                {
                    return;
                }

                _machineNameProperty = value;
                RaisePropertyChanged(MachineNamePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IpAddress" /> property's name.
        /// </summary>
        public const string IpAddressPropertyName = "IpAddress";

        private string _ipAddressProperty = "";

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress
        {
            get
            {
                return _ipAddressProperty;
            }

            set
            {
                if (_ipAddressProperty == value)
                {
                    return;
                }

                _ipAddressProperty = value;
                RaisePropertyChanged(IpAddressPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="ProgramVersion" /> property's name.
        /// </summary>
        public const string ProgramVersionPropertyName = "ProgramVersion";

        private string _programVersionProperty = "";

        /// <summary>
        /// 程序版本
        /// </summary>
        public string ProgramVersion
        {
            get
            {
                return _programVersionProperty;
            }

            set
            {
                if (_programVersionProperty == value)
                {
                    return;
                }

                _programVersionProperty = value;
                RaisePropertyChanged(ProgramVersionPropertyName);
            }
        }




    }
}
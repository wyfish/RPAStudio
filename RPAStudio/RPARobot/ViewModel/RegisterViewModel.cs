using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System;
using RPARobot.Librarys;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        public Window m_view { get; set; }
        /// <summary>
        /// Initializes a new instance of the RegisterViewModel class.
        /// </summary>
        public RegisterViewModel()
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

        public void LoadRegisterInfo()
        {
            //加载注册信息
            Task.Run(()=> {
                //异步加载，避免卡顿
                bool isRegistered = false;
                string expiresDate = "";
                GetRegisterInfo(ref isRegistered, ref expiresDate);

                Common.RunInUI(()=> {
                    IsRegistered = isRegistered;
                    ExpiresDate = expiresDate;

                    IsNeverExpires = expiresDate == "forever" ? true : false;

                    ViewModelLocator.Instance.Startup.RefreshProgramStatus(IsRegistered);
                });
            });
                      
        }



        /// <summary>
        /// The <see cref="IsRegistered" /> property's name.
        /// </summary>
        public const string IsRegisteredPropertyName = "IsRegistered";

        private bool _isRegisteredProperty = false;

        /// <summary>
        /// Sets and gets the IsRegistered property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsRegistered
        {
            get
            {
                return _isRegisteredProperty;
            }

            set
            {
                if (_isRegisteredProperty == value)
                {
                    return;
                }

                _isRegisteredProperty = value;
                RaisePropertyChanged(IsRegisteredPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsNeverExpires" /> property's name.
        /// </summary>
        public const string IsNeverExpiresPropertyName = "IsNeverExpires";

        private bool _isNeverExpiresProperty = false;

        /// <summary>
        /// Sets and gets the IsNeverExpires property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsNeverExpires
        {
            get
            {
                return _isNeverExpiresProperty;
            }

            set
            {
                if (_isNeverExpiresProperty == value)
                {
                    return;
                }

                _isNeverExpiresProperty = value;
                RaisePropertyChanged(IsNeverExpiresPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="ExpiresDate" /> property's name.
        /// </summary>
        public const string ExpiresDatePropertyName = "ExpiresDate";

        private string _expiresDateProperty = "";

        /// <summary>
        /// Sets and gets the ExpiresDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ExpiresDate
        {
            get
            {
                return _expiresDateProperty;
            }

            set
            {
                if (_expiresDateProperty == value)
                {
                    return;
                }

                _expiresDateProperty = value;
                RaisePropertyChanged(ExpiresDatePropertyName);
            }
        }


        private RelayCommand _exportMachineCodeFileCommand;

        /// <summary>
        /// Gets the ExportMachineCodeFileCommand.
        /// </summary>
        public RelayCommand ExportMachineCodeFileCommand
        {
            get
            {
                return _exportMachineCodeFileCommand
                    ?? (_exportMachineCodeFileCommand = new RelayCommand(
                    () =>
                    {
                        //导出机器码
                        var d = DateTime.Now;
                        var fileName = $"机器码({d.Year}-{d.Month:D2}-{d.Day:D2} {d.Hour:D2}：{d.Minute:D2}：{d.Second:D2})";

                        //选择待保存的目录
                        string userSelPath;
                        bool ret = Common.ShowSaveAsFileDialog(out userSelPath, fileName, ".machine", "机器码文件");

                        if (ret == true)
                        {
                            //生成机器信息JSON格式数据
                            try
                            {
                                var computer = Plugins.Shared.Library.Librarys.MyComputerInfo.Instance();
                                string computerJson = Newtonsoft.Json.JsonConvert.SerializeObject(computer, Newtonsoft.Json.Formatting.Indented);

                                //RSA公钥加密
                                var secretStr = Plugins.Shared.Library.Librarys.RSACommon.EncryptString(computerJson, Properties.Resources.verify_public_rsa);
                                //保存文件
                                System.IO.File.WriteAllText(userSelPath, secretStr);

                                var result = MessageBox.Show(m_view, "导出机器码成功，是否定位到该机器码文件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                                if(result == MessageBoxResult.Yes)
                                {
                                    Common.LocateFileInExplorer(userSelPath);
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show(m_view, "导出机器码成功失败，请检查", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            
                        }
                        
                    }));
            }
        }

        public void GetRegisterInfo(ref bool isRegistered, ref string expiresDate)
        {
            if (IsNotExpired(ref expiresDate))
            {
                isRegistered = true;
            }
            else
            {
                isRegistered = false;
                expiresDate = "";
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

            return IsNotExpired(fileFullPath,ref expiresDate);
        }


        private bool IsNotExpired(string fileFullPath,ref string expiresDate)
        {
            bool isNotExpired = false;

            if (!System.IO.File.Exists(fileFullPath))
            {
                return isNotExpired;
            }

            if (Plugins.Shared.Library.Librarys.Common.CheckAuthorization(fileFullPath, Properties.Resources.verify_public_rsa, ref expiresDate))
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



        private RelayCommand _importAuthorizationFileCommand;

        /// <summary>
        /// Gets the ImportAuthorizationFileCommand.
        /// </summary>
        public RelayCommand ImportAuthorizationFileCommand
        {
            get
            {
                return _importAuthorizationFileCommand
                    ?? (_importAuthorizationFileCommand = new RelayCommand(
                    () =>
                    {
                        //导入授权码文件
                        var fileFullPath = Common.ShowSelectSingleFileDialog("授权码文件|*.authorization");

                        if(string.IsNullOrEmpty(fileFullPath))
                        {
                            return;
                        }

                        processImportAuthorizationFile(fileFullPath);
                    }));
            }
        }

        private void processImportAuthorizationFile(string fileFullPath)
        {
            string expiresDate = "";
            if (IsNotExpired(fileFullPath, ref expiresDate))
            {
                try
                {
                    //拷贝授权文件到特定目录下，然后设置全局的有效标志，以便程序其它地方判断
                    var authorizationDir = App.LocalRPAStudioDir + @"\Authorization";
                    if (!System.IO.Directory.Exists(authorizationDir))
                    {
                        System.IO.Directory.CreateDirectory(authorizationDir);
                    }
                    var sourcePath = fileFullPath;
                    var targetPath = authorizationDir + @"\license.authorization";

                    System.IO.File.Copy(sourcePath, targetPath, true);

                    LoadRegisterInfo();

                    MessageBox.Show(m_view, "授权成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show(m_view, "授权文件操作失败，请检查！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(m_view, "授权文件非法！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private RelayCommand _closeCommand;

        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(
                    () =>
                    {
                        m_view.Close();
                    }));
            }
        }



        private RelayCommand<DragEventArgs> _dropCommand;

        /// <summary>
        /// Gets the DropCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> DropCommand
        {
            get
            {
                return _dropCommand
                    ?? (_dropCommand = new RelayCommand<DragEventArgs>(
                    e =>
                    {
                        if (e.Data.GetDataPresent(DataFormats.FileDrop))
                        {
                            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                            foreach (string file in files)
                            {
                                string extension = System.IO.Path.GetExtension(file);
                                if(extension == ".authorization")
                                {
                                    processImportAuthorizationFile(file);
                                    break;
                                }
                               
                            }
                        }
                    }));
            }
        }








        
    }
}
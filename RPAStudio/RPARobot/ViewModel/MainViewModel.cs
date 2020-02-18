using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using NuGet;
using System.Linq;
using RPARobot.Librarys;
using System.Collections.ObjectModel;
using log4net;
using System.Collections.Generic;
using RPARobot.Executor;
using GalaSoft.MvvmLight.Messaging;
using System;
using RPARobot.Services;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using Flurl.Http;
using Plugins.Shared.Library;
using System.Threading;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RunManager m_runManager { get; set; }

        public Window m_view { get; set; }

        public string PackagesDir { get; set; }

        public string InstalledPackagesDir { get; set; }

        /// <summary>
        /// ffmpeg服务类
        /// </summary>
        public FFmpegService FFmpegService { get; set; }

        /// <summary>
        /// 包服务类
        /// </summary>
        public PackageService PackageService { get; set; }

        /// <summary>
        /// 控制中心服务类
        /// </summary>
        public ControlServerService ControlServerService { get; set; }

        /// <summary>
        /// 注册定时器
        /// </summary>
        private DispatcherTimer RegisterTimer { get; set; }

        /// <summary>
        /// 获取分配给本机器的所有流程定时器
        /// </summary>
        private DispatcherTimer GetProcessesTimer { get; set; }

        /// <summary>
        /// 获取当前应该运行的流程的定时器
        /// </summary>
        private DispatcherTimer GetRunProcesssTimer { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            var commonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            PackagesDir = commonApplicationData + @"\RPAStudio\Packages";//机器人默认读取nupkg包的位置
            InstalledPackagesDir = commonApplicationData + @"\RPAStudio\InstalledPackages";//机器人默认安装nupkg包的位置
            if(PackageService == null)
            {
                PackageService = new PackageService(this);
            }

            Messenger.Default.Register<RunManager>(this, "BeginRun", BeginRun);
            Messenger.Default.Register<RunManager>(this, "EndRun", EndRun);
        }

        /// <summary>
        /// 初始化控制中心
        /// </summary>
        public void InitControlServer()
        {
            if (ControlServerService == null)
            {
                ControlServerService = new ControlServerService();
            }

            RegisterTimer = new DispatcherTimer();
            RegisterTimer.Interval = TimeSpan.FromSeconds(60);
            RegisterTimer.Tick += RegisterTimer_Tick;
            RegisterTimer.Start();
            RegisterTimer_Tick(null, null);

            GetProcessesTimer = new DispatcherTimer();
            GetProcessesTimer.Interval = TimeSpan.FromSeconds(30);
            GetProcessesTimer.Tick += GetProcessesTimer_Tick;
            GetProcessesTimer.Start();
            GetProcessesTimer_Tick(null, null);

            //GetRunProcesssTimer
            GetRunProcesssTimer = new DispatcherTimer();
            GetRunProcesssTimer.Interval = TimeSpan.FromSeconds(30);
            GetRunProcesssTimer.Tick += GetRunProcesssTimer_Tick;
            GetRunProcesssTimer.Start();
            GetRunProcesssTimer_Tick(null, null);

            //TEST
            //ControlServerService.UpdateRunStatus("测试流程发布第二个", "2.0.8", ControlServerService.enProcessStatus.Exception);
            //ControlServerService.Log("test1","2.0.1","DEBUG","测试日志22222");
        }

       /// <summary>
       /// 注册定时器处理函数
       /// </summary>
       /// <param name="sender">发送者</param>
       /// <param name="e">参数</param>
        private void RegisterTimer_Tick(object sender, EventArgs e)
        {
            ControlServerService.Register();
        }

        /// <summary>
        /// 获取流程列表定时器处理函数
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">参数</param>
        private void GetProcessesTimer_Tick(object sender, EventArgs e)
        {
            GetProcessesTimer.Stop();

            Task.Run(async()=> {
                var jArr = await ControlServerService.GetProcesses();

                //下载安装包
                if(jArr != null)
                {
                    bool needRefresh = false;
                    for (int i = 0; i < jArr.Count; i++)
                    {
                        var jObj = jArr[i];
                        var nupkgName = jObj["PROCESSNAME"].ToString();
                        var nupkgVersion = jObj["PROCESSVERSION"].ToString();
                        var nupkgFileName = jObj["NUPKGFILENAME"].ToString();
                        var nupkgUrl = jObj["NUPKGURL"].ToString();

                        //判断本地是否存在该包，不存在则下载下来
                        if(!System.IO.File.Exists(System.IO.Path.Combine(PackagesDir, nupkgFileName)))
                        {
                            var downloadAndSavePath = await nupkgUrl.DownloadFileAsync(PackagesDir, nupkgFileName);
                            needRefresh = true;
                        }

                        //比当前包版本高的全删除（Robot默认只能运行高版本的，所以不删除会导致设计理念冲突）
                        var repo = PackageRepositoryFactory.Default.CreateRepository(PackagesDir);
                        var pkgNameList = repo.FindPackagesById(nupkgName);
                        foreach (var item in pkgNameList)
                        {
                            if(item.Version > new SemanticVersion(nupkgVersion))
                            {
                                //删除该包
                                var file = PackagesDir + @"\" + nupkgName + @"." + item.Version.ToString() + ".nupkg";
                                Common.DeleteFile(file);
                            }
                        }
                    }

                    if(needRefresh)
                    {
                        Common.RunInUI(() =>
                        {
                            RefreshAllPackages();
                        });
                    }
                }

                GetProcessesTimer.Start();
            });
            
        }

        /// <summary>
        /// 获取当前应运行的流程定时器触发函数
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">参数</param>
        private void GetRunProcesssTimer_Tick(object sender, EventArgs e)
        {
            GetRunProcesssTimer.Stop();

            Task.Run(async () =>
            {
                var jObj = await ControlServerService.GetRunProcess();
                if(jObj != null)
                {
                    var processName = jObj["PROCESSNAME"].ToString();
                    var processVersion = jObj["PROCESSVERSION"].ToString();

                    //停止所有其它正在运行的流程,启动这个流程（如果该流程已经启动，则不操作）
                    PackageService.Run(processName, processVersion);
                }

                GetRunProcesssTimer.Start();
            });

        }

        /// <summary>
        /// 开始执行工作流时触发
        /// </summary>
        /// <param name="obj">对象</param>
        private void BeginRun(RunManager obj)
        {
            SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "流程运行开始……");

            //不应该更新状态为START，不然和用户自己设置的标识冲突了
            //Task.Run(async()=> {
            //    await ControlServerService.UpdateRunStatus(obj.m_packageItem.Name, obj.m_packageItem.Version, ControlServerService.enProcessStatus.Start);
            //});


            if (FFmpegService != null)
            {
                FFmpegService.StopCaptureScreen();
                FFmpegService = null;
            }

            if(ViewModelLocator.Instance.UserPreferences.IsEnableScreenRecorder)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "屏幕录像开始……");
                var screenRecorderFilePath = App.LocalRPAStudioDir + @"\ScreenRecorder\" + obj.m_packageItem.Name + @"(" + DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒") + ").mp4";
                FFmpegService = new FFmpegService(screenRecorderFilePath, ViewModelLocator.Instance.UserPreferences.FPS, ViewModelLocator.Instance.UserPreferences.Quality);//默认存到%localappdata%下RPASTUDIO下的ScreenRecorder目录下

                Task.Run(() =>
                {
                    FFmpegService.StartCaptureScreen();
                });

                //等待屏幕录像ffmpeg进程启动
                int wait_count = 0;
                while(!FFmpegService.IsRunning())
                {
                    wait_count++;
                    Thread.Sleep(300);
                    if(wait_count == 10)
                    {
                        break;
                    }
                }
            }
            

            Common.RunInUI(()=> {
                m_view.Hide();

                obj.m_packageItem.IsRunning = true;

                IsWorkflowRunning = true;
                WorkflowRunningName = obj.m_packageItem.Name;
                WorkflowRunningToolTip = obj.m_packageItem.ToolTip;
                WorkflowRunningStatus = "正在运行";
            });
        }

        private void EndRun(RunManager obj)
        {
            SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "流程运行结束");

            Task.Run(async () =>
            {
                if(obj.HasException)
                {
                    await ControlServerService.UpdateRunStatus(obj.m_packageItem.Name, obj.m_packageItem.Version, ControlServerService.enProcessStatus.Exception);
                }
                else
                {
                    await ControlServerService.UpdateRunStatus(obj.m_packageItem.Name, obj.m_packageItem.Version, ControlServerService.enProcessStatus.Stop);
                }
                
            });

            if (ViewModelLocator.Instance.UserPreferences.IsEnableScreenRecorder)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "屏幕录像结束");
                FFmpegService.StopCaptureScreen();
                FFmpegService = null;
            }

            Common.RunInUI(() => {
                m_view.Show();
                m_view.Activate();

                obj.m_packageItem.IsRunning = false;

                //由于有可能列表已经刷新，所以需要重置IsRunning状态，为了方便，全部重置
                foreach (var pkg in PackageItems)
                {
                    pkg.IsRunning = false;
                }

                IsWorkflowRunning = false;
                WorkflowRunningName = "";
                WorkflowRunningStatus = "";
            });
        }


        public void RefreshAllPackages()
        {
            PackageItems.Clear();

            var repo = PackageRepositoryFactory.Default.CreateRepository(PackagesDir);
            var pkgList = repo.GetPackages();

            var pkgSet = new SortedSet<string>();
            foreach (var pkg in pkgList)
            {
                //通过set去重
                pkgSet.Add(pkg.Id);
            }

            Dictionary<string, IPackage> installedPkgDict = new Dictionary<string, IPackage>();

            var packageManager = new PackageManager(repo, InstalledPackagesDir);
            foreach (IPackage pkg in packageManager.LocalRepository.GetPackages())
            {
                installedPkgDict[pkg.Id] = pkg;
            }

            foreach (var name in pkgSet)
            {
                var item = new PackageItem();
                item.Name = name;

                var version = repo.FindPackagesById(name).Max(p => p.Version);
                item.Version = version.ToString();

                var pkgNameList = repo.FindPackagesById(name);
                foreach(var i in pkgNameList)
                {
                    item.VersionList.Add(i.Version.ToString());
                }

                bool isNeedUpdate = false;
                if(installedPkgDict.ContainsKey(item.Name))
                {
                    var installedVer = installedPkgDict[item.Name].Version;
                    if(version > installedVer)
                    {
                        isNeedUpdate = true;
                    }
                }
                else
                {
                    isNeedUpdate = true;
                }
                item.IsNeedUpdate = isNeedUpdate;

                var pkg = repo.FindPackage(name, version);
                item.Package = pkg;
                var publishedTime = pkg.Published.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                item.ToolTip = string.Format("名称：{0}\r\n版本：{1}\r\n发布说明：{2}\r\n项目描述：{3}\r\n发布时间：{4}", item.Name, item.Version,pkg.ReleaseNotes,pkg.Description, (publishedTime == null? "未知": publishedTime));

                if(IsWorkflowRunning && item.Name == WorkflowRunningName)
                {
                    item.IsRunning = true;//如果当前该包工程已经在运行，则要设置IsRunning
                }

                PackageItems.Add(item);
            }
            

            doSearch();
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
                        RefreshAllPackages();
                    }));
            }
        }




        public bool IsEnableAuthorizationCheck
        {
            get
            {
#if ENABLE_AUTHORIZATION_CHECK
                return true;
#else
                return false;
#endif
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

        private RelayCommand _activatedCommand;

        /// <summary>
        /// Gets the ActivatedCommand.
        /// </summary>
        public RelayCommand ActivatedCommand
        {
            get
            {
                return _activatedCommand
                    ?? (_activatedCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Normal;
                        RefreshAllPackages();
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




        private RelayCommand _refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(
                    () =>
                    {
                        RefreshAllPackages();
                    }));
            }
        }






        private RelayCommand _userPreferencesCommand;

        /// <summary>
        /// Gets the UserPreferencesCommand.
        /// </summary>
        public RelayCommand UserPreferencesCommand
        {
            get
            {
                return _userPreferencesCommand
                    ?? (_userPreferencesCommand = new RelayCommand(
                    () =>
                    {
                        if(!ViewModelLocator.Instance.Startup.UserPreferencesWindow.IsVisible)
                        {
                            var vm = ViewModelLocator.Instance.Startup.UserPreferencesWindow.DataContext as UserPreferencesViewModel;
                            vm.LoadSettings();

                            ViewModelLocator.Instance.Startup.UserPreferencesWindow.Show();
                        }

                        ViewModelLocator.Instance.Startup.UserPreferencesWindow.Activate();
                    }));
            }
        }


        private RelayCommand _viewLogsCommand;

        /// <summary>
        /// Gets the ViewLogsCommand.
        /// </summary>
        public RelayCommand ViewLogsCommand
        {
            get
            {
                return _viewLogsCommand
                    ?? (_viewLogsCommand = new RelayCommand(
                    () =>
                    {
                        //打开日志所在的目录
                        Common.LocateDirInExplorer(App.LocalRPAStudioDir + @"\Logs");
                    }));
            }
        }



        private RelayCommand _viewScreenRecordersCommand;

        /// <summary>
        /// 查看录像
        /// </summary>
        public RelayCommand ViewScreenRecordersCommand
        {
            get
            {
                return _viewScreenRecordersCommand
                    ?? (_viewScreenRecordersCommand = new RelayCommand(
                    () =>
                    {
                        //打开屏幕录像所在的目录
                        Common.LocateDirInExplorer(App.LocalRPAStudioDir + @"\ScreenRecorder");
                    }));
            }
        }





        private RelayCommand _registerProductCommand;

        /// <summary>
        /// Gets the RegisterProductCommand.
        /// </summary>
        public RelayCommand RegisterProductCommand
        {
            get
            {
                return _registerProductCommand
                    ?? (_registerProductCommand = new RelayCommand(
                    () =>
                    {
                        //弹出注册窗口
                        if (!ViewModelLocator.Instance.Startup.RegisterWindow.IsVisible)
                        {
                            var vm = ViewModelLocator.Instance.Startup.RegisterWindow.DataContext as RegisterViewModel;
                            vm.LoadRegisterInfo();

                            ViewModelLocator.Instance.Startup.RegisterWindow.Show();
                        }

                        ViewModelLocator.Instance.Startup.RegisterWindow.Activate();
                    }));
            }
        }



        private RelayCommand _aboutProductCommand;

        /// <summary>
        /// 关于产品窗口
        /// </summary>
        public RelayCommand AboutProductCommand
        {
            get
            {
                return _aboutProductCommand
                    ?? (_aboutProductCommand = new RelayCommand(
                    () =>
                    {
                        if (!ViewModelLocator.Instance.Startup.AboutWindow.IsVisible)
                        {
                            var vm = ViewModelLocator.Instance.Startup.AboutWindow.DataContext as AboutViewModel;
                            vm.LoadAboutInfo();

                            ViewModelLocator.Instance.Startup.AboutWindow.Show();
                        }

                        ViewModelLocator.Instance.Startup.AboutWindow.Activate();
                    }));
            }
        }





        /// <summary>
        /// The <see cref="PackageItems" /> property's name.
        /// </summary>
        public const string PackageItemsPropertyName = "PackageItems";

        private ObservableCollection<PackageItem> _packageItemsProperty = new ObservableCollection<PackageItem>();

        /// <summary>
        /// Sets and gets the PackageItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<PackageItem> PackageItems
        {
            get
            {
                return _packageItemsProperty;
            }

            set
            {
                if (_packageItemsProperty == value)
                {
                    return;
                }

                _packageItemsProperty = value;
                RaisePropertyChanged(PackageItemsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsSearchResultEmpty" /> property's name.
        /// </summary>
        public const string IsSearchResultEmptyPropertyName = "IsSearchResultEmpty";

        private bool _isSearchResultEmptyProperty = false;

        /// <summary>
        /// Sets and gets the IsSearchResultEmpty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSearchResultEmpty
        {
            get
            {
                return _isSearchResultEmptyProperty;
            }

            set
            {
                if (_isSearchResultEmptyProperty == value)
                {
                    return;
                }

                _isSearchResultEmptyProperty = value;
                RaisePropertyChanged(IsSearchResultEmptyPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="SearchText" /> property's name.
        /// </summary>
        public const string SearchTextPropertyName = "SearchText";

        private string _searchTextProperty = "";

        /// <summary>
        /// Sets and gets the SearchText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SearchText
        {
            get
            {
                return _searchTextProperty;
            }

            set
            {
                if (_searchTextProperty == value)
                {
                    return;
                }

                _searchTextProperty = value;
                RaisePropertyChanged(SearchTextPropertyName);

                doSearch();
            }
        }

        private void doSearch()
        {
            var searchContent = SearchText.Trim();
            if (string.IsNullOrEmpty(searchContent))
            {
                //还原起始显示
                foreach (var item in PackageItems)
                {
                    item.IsSearching = false;
                }

                foreach (var item in PackageItems)
                {
                    item.SearchText = searchContent;
                }

                IsSearchResultEmpty = false;
            }
            else
            {
                //根据搜索内容显示

                foreach (var item in PackageItems)
                {
                    item.IsSearching = true;
                }

                //预先全部置为不匹配
                foreach (var item in PackageItems)
                {
                    item.IsMatch = false;
                }


                foreach (var item in PackageItems)
                {
                    item.ApplyCriteria(searchContent);
                }

                IsSearchResultEmpty = true;
                foreach (var item in PackageItems)
                {
                    if (item.IsMatch)
                    {
                        IsSearchResultEmpty = false;
                        break;
                    }
                }

            }
        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The <see cref="IsWorkflowRunning" /> property's name.
        /// </summary>
        public const string IsWorkflowRunningPropertyName = "IsWorkflowRunning";

        private bool _isWorkflowRunningProperty = false;

        /// <summary>
        /// Sets and gets the IsWorkflowRunning property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWorkflowRunning
        {
            get
            {
                return _isWorkflowRunningProperty;
            }

            set
            {
                if (_isWorkflowRunningProperty == value)
                {
                    return;
                }

                _isWorkflowRunningProperty = value;
                RaisePropertyChanged(IsWorkflowRunningPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="WorkflowRunningToolTip" /> property's name.
        /// </summary>
        public const string WorkflowRunningToolTipPropertyName = "WorkflowRunningToolTip";

        private string _workflowRunningToolTipProperty = "";

        /// <summary>
        /// Sets and gets the WorkflowRunningToolTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WorkflowRunningToolTip
        {
            get
            {
                return _workflowRunningToolTipProperty;
            }

            set
            {
                if (_workflowRunningToolTipProperty == value)
                {
                    return;
                }

                _workflowRunningToolTipProperty = value;
                RaisePropertyChanged(WorkflowRunningToolTipPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="WorkflowRunningName" /> property's name.
        /// </summary>
        public const string WorkflowRunningNamePropertyName = "WorkflowRunningName";

        private string _workflowRunningNameProperty = "";

        /// <summary>
        /// Sets and gets the WorkflowRunningName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WorkflowRunningName
        {
            get
            {
                return _workflowRunningNameProperty;
            }

            set
            {
                if (_workflowRunningNameProperty == value)
                {
                    return;
                }

                _workflowRunningNameProperty = value;
                RaisePropertyChanged(WorkflowRunningNamePropertyName);
            }
        }





        /// <summary>
        /// The <see cref="WorkflowRunningStatus" /> property's name.
        /// </summary>
        public const string WorkflowRunningStatusPropertyName = "WorkflowRunningStatus";

        private string _workflowRunningStatusProperty = "";

        /// <summary>
        /// Sets and gets the WorkflowRunningStatus property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WorkflowRunningStatus
        {
            get
            {
                return _workflowRunningStatusProperty;
            }

            set
            {
                if (_workflowRunningStatusProperty == value)
                {
                    return;
                }

                _workflowRunningStatusProperty = value;
                RaisePropertyChanged(WorkflowRunningStatusPropertyName);
            }
        }




        private RelayCommand _stopCommand;

        /// <summary>
        /// Gets the StopCommand.
        /// </summary>
        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand
                    ?? (_stopCommand = new RelayCommand(
                    () =>
                    {
                        if(m_runManager!=null)
                        {
                            m_runManager.Stop();
                        }  
                    },
                    () => true));
            }
        }
    }
}
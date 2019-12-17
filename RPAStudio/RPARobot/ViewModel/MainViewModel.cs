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
            InstalledPackagesDir = commonApplicationData + @"\RPAStudio\InstalledPackages";//机器人默认读取nupkg包的位置

            Messenger.Default.Register<RunManager>(this, "BeginRun", BeginRun);
            Messenger.Default.Register<RunManager>(this, "EndRun", EndRun);
        }


        private void BeginRun(RunManager obj)
        {
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
                        m_runManager.Stop();
                    },
                    () => true));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using log4net;
using Plugins.Shared.Library;
using Plugins.Shared.Library.Nuget;
using RPAStudio.Librarys;
using RPAStudio.Windows;
using RPAStudio.DataManager;
using RPAStudio.DragDropHandler;
using RPAStudio.Localization;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ProjectViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int RemoveUnusedScreenshotsCount { get; private set; }

        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectMain { get; set; }

        public ProjectTreeItem MainProjectTreeItem { get; private set; }

        public ProjectTreeItem RootProjectTreeItem { get; set; }

        public string ProjectPath { get; private set; }

        public string CurrentProjectJsonFile { get; set; }


        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewModel()
        {
            Messenger.Default.Register<MessengerObjects.ProjectOpen>(this, OpenProject);
            Messenger.Default.Register<ProjectSettingsViewModel>(this, "ProjectSettingsModify", ProjectSettingsModify);
            Messenger.Default.Register<NewFolderViewModel>(this, "AddNewFolder", AddNewFolder);
            Messenger.Default.Register<RenameViewModel>(this, "Rename", Rename);
            Messenger.Default.Register<MessengerObjects.ProjectClose>(this, CloseProject);
        }


        private void Rename(RenameViewModel obj)
        {
            if(obj.IsMain)
            {
                //修改的是主XAML，需要改project.json里的main对应的xaml文件，再刷新
                var relativeMainXaml = Common.MakeRelativePath(ProjectPath, obj.Dir + @"\" + obj.DstName);
                updateProjectJsonMain(relativeMainXaml);
            }

            RefreshCommand.Execute(null);
        }

        private void AddNewFolder(NewFolderViewModel obj)
        {
            RefreshCommand.Execute(null);
        }

        private void updateProjectJsonMain(string mainFile)
        {
            //json更新
            string json = File.ReadAllText(CurrentProjectJsonFile);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["main"] = mainFile;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(CurrentProjectJsonFile, output);
        }


        public void SaveProjectJsonConfig(ProjectJsonConfig json_cfg)
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(json_cfg, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(CurrentProjectJsonFile, output);
        }


        private void ProjectSettingsModify(ProjectSettingsViewModel vm)
        {
            ProjectName = vm.ProjectName;
            ProjectDescription = vm.ProjectDescription;

            RootProjectTreeItem.Name = ProjectName;
        }

        private void CloseProject(MessengerObjects.ProjectClose obj)
        {
            ProjectSettingsDataManager.Instance.Unload();
        }

        private void OpenProject(MessengerObjects.ProjectOpen msg)
        {
            //根据project.json信息打开项目

            string json_file = msg.ProjectJsonFile;

            if(json_file == CurrentProjectJsonFile)
            {
                //当前项目早已经打开，只需要做一些必要操作即可
                if (!string.IsNullOrEmpty(msg.DefaultOpenXamlFile))
                {
                    //打开特定文档
                    var item = GetProjectTreeItemByFullPath(msg.DefaultOpenXamlFile);
                    if(item != null)
                    {
                        item.OpenXamlCommand.Execute(null);
                    }
                }

                //关闭起始页
                ViewModelLocator.Instance.Main.IsOpenStartScreen = false;
                ViewModelLocator.Instance.Main.IsBackButtonVisible = true;

                return;
            }

            ViewModelLocator.Instance.Output.ClearAllCommand.Execute(null);//打开新项目前清空旧项目的输出日志内容

            ViewModelLocator.Instance.Main.IsStartContentBusy = true;

            Task.Run(async() => {
                try
                {
                    CurrentProjectJsonFile = json_file;

                    ProcessProjectJsonConfig();//提前进行旧版本判断

                    //加载项目依赖项
                    await LoadDependencies();
                   
                    //调整最近项目顺序
                    if (msg.Sender is RecentProjectItem)
                    {
                        (msg.Sender as RecentProjectItem).RecentProjectsReorder();
                    }

                    ProjectSettingsDataManager.ResetInstance();
                    ProjectSettingsDataManager.Instance.Load(Path.GetDirectoryName(msg.ProjectJsonFile));

                    initProject(false);

                    var state_changed_msg = new MessengerObjects.ProjectStateChanged();
                    state_changed_msg.ProjectPath = Path.GetDirectoryName(CurrentProjectJsonFile);
                    state_changed_msg.ProjectName = ProjectName;
                    Messenger.Default.Send(state_changed_msg);

                    Common.RunInUI(() => {
                        //清空组件树历史条目
                        ViewModelLocator.Instance.Activities.ItemRecent.Children.Clear();

                        ProjectItems = new ObservableCollection<ProjectTreeItem>(ProjectItemsTemp);

                        if (string.IsNullOrEmpty(msg.DefaultOpenXamlFile))
                        {
                            //自动打开Main文档
                            if (MainProjectTreeItem != null)
                            {
                                MainProjectTreeItem.OpenXamlCommand.Execute(null);
                            }
                        }
                        else
                        {
                            //打开特定文档
                            var item = GetProjectTreeItemByFullPath(msg.DefaultOpenXamlFile);
                            if (item != null)
                            {
                                item.OpenXamlCommand.Execute(null);
                            }
                        }

                        //关闭起始页
                        ViewModelLocator.Instance.Main.IsOpenStartScreen = false;
                        ViewModelLocator.Instance.Main.IsBackButtonVisible = true;
                    });
                }
                catch (Exception err)
                {
                    Logger.Error(err, logger);
                    CurrentProjectJsonFile = "";
                    Common.RunInUI(() => {
                        MessageBox.Show(App.Current.MainWindow, ResxIF.GetString("ProjectOpenError"), ResxIF.GetString("ErrorText"), MessageBoxButton.OK, MessageBoxImage.Warning); ;
                    });
                }finally
                {
                    ViewModelLocator.Instance.Main.IsStartContentBusy = false;
                }
                
            });

           
        }

        public async Task LoadDependencies()
        {
            var json_cfg = ProcessProjectJsonConfig();
            foreach (JProperty jp in (JToken)json_cfg.dependencies)
            {
                var ver_range = VersionRange.Parse((string)jp.Value);
                if(ver_range.IsMinInclusive)
                {
                    var target_ver = NuGet.Versioning.NuGetVersion.Parse(ver_range.MinVersion.ToString());
                    await NuGetPackageController.Instance.DownloadAndInstall(new NuGet.Packaging.Core.PackageIdentity(jp.Name, target_ver));
                }
                else
                {
                    //TODO WJF 大于但不等于低版本时如何处理？
                }
            }

            //加载依赖的DLL
            var targetDir = NuGetPackageController.Instance.TargetFolder;
            string[] dll_files = Directory.GetFiles(targetDir, "*.dll", SearchOption.TopDirectoryOnly);//搜索TargetDir最外层目录下的dll文件

            foreach(var dll_file in dll_files)
            {
                //判断dll_file对应的文件是否在主程序所在目录存在，若存在，则加载主程序所在目录下的DLL，避免加载不同路径下的相同DLL文件
                var checkPath = System.Environment.CurrentDirectory + @"\" + Path.GetFileName(dll_file);
                if (System.IO.File.Exists(checkPath))
                {
                    Logger.Debug($"发现主程序所在目录下存在同名dll，忽略加载 {checkPath}",logger);
                    continue;//避免加载重复的DLL
                }

                var asm = Assembly.LoadFrom(dll_file);
                var dll_file_name_without_ext = Path.GetFileNameWithoutExtension(dll_file);
                try
                {
                    string activity_config_xml = null;
                    //var activity_config_info = Application.GetResourceStream(new Uri($"pack://application:,,,/{dll_file_name_without_ext};Component/activity.config.xml", UriKind.Absolute));
                    var activity_config_info = Get_Localized_activity_config_info(dll_file_name_without_ext);

                    try
                    {
                        // #9 An Activity must contains "activities" in its file name. (to avoid exception)
                        if (dll_file_name_without_ext.ToLower().Contains("activities"))
                        {
                            using (StreamReader reader = new StreamReader(activity_config_info.Stream))
                            {
                                activity_config_xml = reader.ReadToEnd();
                                Logger.Debug($"开始挂载{dll_file}中的活动配置信息……", logger);

                                ViewModelLocator.Instance.Activities.MountActivityConfig(activity_config_xml);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, $"挂载{dll_file}中的activity.config.xml信息出错", err);
                        //Mount Error in activity.config.xml information
                    }

                }
                catch (Exception)
                {
                    //dll中找不到activity.config.xml时会报错走到这里，无须打印错误日志
                    //When activity.config.xml is not found in the dll, it will report an error and go here without printing the error log
                }
            }
        }

        private System.Windows.Resources.StreamResourceInfo Get_Localized_activity_config_info(string dll_file_name_without_ext)
        {
            string locale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            try
            {
                return Application.GetResourceStream(new Uri($"pack://application:,,,/{dll_file_name_without_ext};Component/activity.config_{locale}.xml", UriKind.Absolute));
            }
            catch (Exception)
            {
                return Application.GetResourceStream(new Uri($"pack://application:,,,/{dll_file_name_without_ext};Component/activity.config.xml", UriKind.Absolute));
            }
        }

        public ProjectJsonConfig ProcessProjectJsonConfig()
        {
            var json_str = File.ReadAllText(CurrentProjectJsonFile);
            try
            {
                var json_cfg = JsonConvert.DeserializeObject<ProjectJsonConfig>(json_str);
                if (json_cfg.Upgrade())
                {
                    //文件格式需要升级转换
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(json_cfg, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(CurrentProjectJsonFile, output);

                    json_str = File.ReadAllText(CurrentProjectJsonFile);
                    json_cfg = JsonConvert.DeserializeObject<ProjectJsonConfig>(json_str);
                }

                return json_cfg;
            }
            catch (Exception)
            {
                throw;
            }
           
           
        }

        private void initProject(bool updateProjectItems = true)
        {
            ProjectItemsTemp.Clear();

            ProjectPath = Path.GetDirectoryName(CurrentProjectJsonFile);

            var json_cfg = ProcessProjectJsonConfig();

            ProjectName = json_cfg.name;
            ProjectDescription = json_cfg.description;
            ProjectMain = ProjectPath + @"\" + json_cfg.main;

            var projectItem = new ProjectTreeItem(this);
            RootProjectTreeItem = projectItem;
            projectItem.IsRoot = true;
            projectItem.IsExpanded = true;
            projectItem.Name = json_cfg.name;
            projectItem.Path = ProjectPath;
            projectItem.ToolTip = ProjectPath;
            projectItem.Icon = "pack://application:,,,/Resources/Image/Project/project.png";
            projectItem.ContextMenuName = "ProjectRootContextMenu";

            ProjectItemsTemp.Add(projectItem);

            InitDependencies(projectItem);
            InitDirectory(new DirectoryInfo(ProjectPath), projectItem);

            if (updateProjectItems)
            {
                ProjectItems = new ObservableCollection<ProjectTreeItem>(ProjectItemsTemp);
            }
        }

        private void InitDependencies(ProjectTreeItem projectItem)
        {
            var dependRootItem = new ProjectTreeItem(this);
            dependRootItem.IsDependRoot = true;
            dependRootItem.IsExpanded = true;
            //dependRootItem.Name = "依赖包";
            dependRootItem.Name = ResxIF.GetString("DependencyText");
            dependRootItem.Icon = "pack://application:,,,/Resources/Image/Project/dependencies.png";
            projectItem.Children.Add(dependRootItem);

            string json = File.ReadAllText(CurrentProjectJsonFile);
            var json_cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectJsonConfig>(json);
            foreach (JProperty jp in (JToken)json_cfg.dependencies)
            {
                var ver_range = VersionRange.Parse((string)jp.Value);

                string ver_desc = "";

                //目前只考虑等于和大于等于两种情况
                if(ver_range.MinVersion == ver_range.MaxVersion)
                {
                    ver_desc = $" = {ver_range.MinVersion}";
                }else
                {
                    if(ver_range.MinVersion != null && ver_range.IsMinInclusive )
                    {
                        ver_desc = $" >= {ver_range.MinVersion}";
                    }
                }

                var desc = jp.Name + ver_desc;

                var dependItem = new ProjectTreeItem(this);
                dependItem.IsDependItem = true;
                dependItem.Name = desc;
                dependItem.Icon = "pack://application:,,,/Resources/Image/Project/depend-item.png";
                dependRootItem.Children.Add(dependItem);
            }

        }

        private void InitDirectory(DirectoryInfo di, ProjectTreeItem parent)
        {
            //当前目录文件夹遍历
            DirectoryInfo[] dis = di.GetDirectories();
            for (int j = 0; j < dis.Length; j++)
            {
                var item = new ProjectTreeItem(this);
                item.Path = dis[j].FullName;
                item.Name = dis[j].Name;
                item.IsDirectory = true;
                item.ContextMenuName = "ProjectDirectoryContextMenu";

                if (item.Name != ".local")
                {
                    parent.Children.Add(item);

                    InitDirectory(dis[j], item);
                }

            }

            //当前目录文件遍历
            FileInfo[] fis = di.GetFiles();
            for (int i = 0; i < fis.Length; i++)
            {
                var item = new ProjectTreeItem(this);
                item.Path = fis[i].FullName;
                item.Name = fis[i].Name;
                item.IsFile = true;

                if (item.Path.EqualsIgnoreCase(CurrentProjectJsonFile))
                {
                    item.IsProjectJson = true;
                }
                
                if (fis[i].Extension.ToLower() == ".xaml")
                {
                    item.IsXaml = true;
                    item.Icon = "pack://application:,,,/Resources/Image/Project/xaml.png";

                    if(item.Path == ProjectMain)
                    {
                        item.IsMain = true;
                        item.ContextMenuName = "ProjectMainXamlContextMenu";

                        MainProjectTreeItem = item;
                    }
                    else
                    {
                        item.ContextMenuName = "ProjectXamlContextMenu";
                    }

                    parent.Children.Add(item);
                }
                else
                {
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(item.Path);
                    item.IconSource = Common.ToImageSource(icon);

                    if (fis[i].Extension.ToLower() == ".py")
                    {
                        item.IsPython = true;
                    }

                    if (item.IsProjectJson)
                    {

                    }
                    else
                    {
                        item.ContextMenuName = "ProjectFileContextMenu";
                    }
                    

                    if (IsShowAllFiles)
                    {
                        parent.Children.Add(item);
                    }
                }

            }

        }


        /// <summary>
        /// The <see cref="IsShowAllFiles" /> property's name.
        /// </summary>
        public const string IsShowAllFilesPropertyName = "IsShowAllFiles";

        private bool _isShowAllFilesProperty = false;

        /// <summary>
        /// Sets and gets the IsShowAllFiles property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowAllFiles
        {
            get
            {
                return _isShowAllFilesProperty;
            }

            set
            {
                if (_isShowAllFilesProperty == value)
                {
                    return;
                }

                _isShowAllFilesProperty = value;
                RaisePropertyChanged(IsShowAllFilesPropertyName);
            }
        }


        public List<ProjectTreeItem> ProjectItemsTemp = new List<ProjectTreeItem>();


        /// <summary>
        /// The <see cref="ProjectItems" /> property's name.
        /// </summary>
        public const string ProjectItemsPropertyName = "ProjectItems";

        private ObservableCollection<ProjectTreeItem> _projectItemsProperty = new ObservableCollection<ProjectTreeItem>();

        /// <summary>
        /// Sets and gets the ProjectItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ProjectTreeItem> ProjectItems
        {
            get
            {
                return _projectItemsProperty;
            }

            set
            {
                if (_projectItemsProperty == value)
                {
                    return;
                }

                _projectItemsProperty = value;
                RaisePropertyChanged(ProjectItemsPropertyName);
            }
        }


        public ProjectTreeItem GetProjectTreeItemByFullPath(string fullPath, ProjectTreeItem parent = null)
        {
            if(parent == null)
            {
                foreach (var item in ProjectItems)
                {
                    if(item.IsXaml && item.Path.EqualsIgnoreCase(fullPath))
                    {
                        return item;
                    }

                    var ret = GetProjectTreeItemByFullPath(fullPath,item);
                    if(ret != null)
                    {
                        return ret;
                    }
                }
            }
            else
            {
                foreach (var child in parent.Children)
                {
                    if (child.IsXaml && child.Path.EqualsIgnoreCase(fullPath))
                    {
                        return child;
                    }

                    var ret = GetProjectTreeItemByFullPath(fullPath, child);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }

            return null;
        }


        private void ProjectTreeItemSetAllIsExpanded(ProjectTreeItem item, bool IsExpanded)
        {
            item.IsExpanded = IsExpanded;
            foreach (var child in item.Children)
            {
                ProjectTreeItemSetAllIsExpanded(child, IsExpanded);
            }
        }

        private void ProjectTreeItemSetAllIsSearching(ProjectTreeItem item, bool IsSearching)
        {
            item.IsSearching = IsSearching;
            foreach (var child in item.Children)
            {
                ProjectTreeItemSetAllIsSearching(child, IsSearching);
            }
        }

        private void ProjectTreeItemSetAllIsMatch(ProjectTreeItem item, bool IsMatch)
        {
            item.IsMatch = IsMatch;
            foreach (var child in item.Children)
            {
                ProjectTreeItemSetAllIsMatch(child, IsMatch);
            }
        }

        private void ProjectTreeItemSetAllSearchText(ProjectTreeItem item, string SearchText)
        {
            item.SearchText = SearchText;
            foreach (var child in item.Children)
            {
                ProjectTreeItemSetAllSearchText(child, SearchText);
            }
        }


        private RelayCommand _expandAllCommand;

        /// <summary>
        /// Gets the ExpandAllCommand.
        /// </summary>
        public RelayCommand ExpandAllCommand
        {
            get
            {
                return _expandAllCommand
                    ?? (_expandAllCommand = new RelayCommand(
                    () =>
                    {
                        foreach (var item in ProjectItems)
                        {
                            ProjectTreeItemSetAllIsExpanded(item, true);
                        }
                    }));
            }
        }


        private RelayCommand _collapseAllCommand;

        /// <summary>
        /// Gets the CollapseAllCommand.
        /// </summary>
        public RelayCommand CollapseAllCommand
        {
            get
            {
                return _collapseAllCommand
                    ?? (_collapseAllCommand = new RelayCommand(
                    () =>
                    {
                        foreach (var item in ProjectItems)
                        {
                            ProjectTreeItemSetAllIsExpanded(item, false);
                        }
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
                        initProject();
                        doSearch();
                    }));
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


        private void doSearch()
        {
            var searchContent = SearchText.Trim();
            if (string.IsNullOrEmpty(searchContent))
            {
                //还原起始显示
                foreach (var item in ProjectItems)
                {
                    ProjectTreeItemSetAllIsSearching(item, false);
                }

                foreach (var item in ProjectItems)
                {
                    ProjectTreeItemSetAllSearchText(item, "");
                }

                IsSearchResultEmpty = false;
            }
            else
            {
                //根据搜索内容显示

                foreach (var item in ProjectItems)
                {
                    ProjectTreeItemSetAllIsSearching(item, true);
                }

                //预先全部置为不匹配
                foreach (var item in ProjectItems)
                {
                    ProjectTreeItemSetAllIsMatch(item, false);
                }


                foreach (var item in ProjectItems)
                {
                    item.ApplyCriteria(searchContent, new Stack<ProjectTreeItem>());
                }

                IsSearchResultEmpty = true;
                foreach (var item in ProjectItems)
                {
                    if (item.IsMatch)
                    {
                        IsSearchResultEmpty = false;
                        break;
                    }
                }

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


        /// <summary>
        /// 重置变量，以便打开新项目时不被旧变量干扰
        /// </summary>
        public void ResetAll()
        {
            ProjectItems.Clear();
            SearchText = "";
            IsShowAllFiles = false;
            CurrentProjectJsonFile = "";

            ProjectTreeItem.IsExpandedDict.Clear();
        }

        private RelayCommand _openProjectSettingsCommand;

        /// <summary>
        /// Gets the OpenProjectSettingsCommand.
        /// </summary>
        public RelayCommand OpenProjectSettingsCommand
        {
            get
            {
                return _openProjectSettingsCommand
                    ?? (_openProjectSettingsCommand = new RelayCommand(
                    () =>
                    {
                        var window = new ProjectSettingsWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as ProjectSettingsViewModel;
                        vm.ProjectName = ProjectName;
                        vm.ProjectDescription = ProjectDescription;
                        vm.CurrentProjectJsonFile = CurrentProjectJsonFile;
                        window.ShowDialog();
                    }));
            }
        }




        private RelayCommand _showAllFilesCommand;

        /// <summary>
        /// Gets the ShowAllFilesCommand.
        /// </summary>
        public RelayCommand ShowAllFilesCommand
        {
            get
            {
                return _showAllFilesCommand
                    ?? (_showAllFilesCommand = new RelayCommand(
                    () =>
                    {
                        IsShowAllFiles = !IsShowAllFiles;

                        RefreshCommand.Execute(null);
                    }));
            }
        }



        private RelayCommand _openProjectDirCommand;

        /// <summary>
        /// Gets the OpenProjectDirCommand.
        /// </summary>
        public RelayCommand OpenProjectDirCommand
        {
            get
            {
                return _openProjectDirCommand
                    ?? (_openProjectDirCommand = new RelayCommand(
                    () =>
                    {
                        Common.LocateDirInExplorer(ProjectPath);
                    }));
            }
        }


        public ProjectTreeItem GetProjectTreeItemIsSelected(ProjectTreeItem parent = null)
        {
            if (parent == null)
            {
                foreach (var item in ProjectItems)
                {
                    if (item.IsSelected)
                    {
                        return item;
                    }

                    var ret = GetProjectTreeItemIsSelected(item);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            else
            {
                foreach (var child in parent.Children)
                {
                    if (child.IsSelected)
                    {
                        return child;
                    }

                    var ret = GetProjectTreeItemIsSelected(child);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }

            return null;
        }



        private RelayCommand _renameSelectedItemCommand;

        /// <summary>
        /// Gets the RenameSelectedItemCommand.
        /// </summary>
        public RelayCommand RenameSelectedItemCommand
        {
            get
            {
                return _renameSelectedItemCommand
                    ?? (_renameSelectedItemCommand = new RelayCommand(
                    () =>
                    {
                        var item = GetProjectTreeItemIsSelected();
                        if (item != null && !item.IsProjectJson && !item.IsRoot)
                        {
                            if (item.IsDirectory)
                            {
                                item.RenameDirCommand.Execute(null);
                            }
                            else if(item.IsFile)
                            {
                                item.RenameFileCommand.Execute(null);
                            }
                        }
                    }));
            }
        }



        private RelayCommand _deleteSelectedItemCommand;

        /// <summary>
        /// Gets the DeleteSelectedItemCommand.
        /// </summary>
        public RelayCommand DeleteSelectedItemCommand
        {
            get
            {
                return _deleteSelectedItemCommand
                    ?? (_deleteSelectedItemCommand = new RelayCommand(
                    () =>
                    {
                        var item = GetProjectTreeItemIsSelected();
                        if (item != null && !item.IsProjectJson && !item.IsRoot)
                        {
                            if (item.IsDirectory)
                            {
                                item.DeleteDirCommand.Execute(null);
                            }
                            else if (item.IsFile)
                            {
                                item.DeleteFileCommand.Execute(null);
                            }
                        }
                    }));
            }
        }





        private RelayCommand _removeUnusedScreenshotsCommand;

        /// <summary>
        /// Gets the RemoveUnusedScreenshotsCommand.
        /// </summary>
        public RelayCommand RemoveUnusedScreenshotsCommand
        {
            get
            {
                return _removeUnusedScreenshotsCommand
                    ?? (_removeUnusedScreenshotsCommand = new RelayCommand(
                    () =>
                    {
                        ViewModelLocator.Instance.Main.SaveAllCommand.Execute(null);

                        RemoveUnusedScreenshotsCount = 0;

                        if(System.IO.Directory.Exists(ProjectPath + @"\.screenshots"))
                        {
                            Common.DirectoryChildrenForEach(new DirectoryInfo(ProjectPath + @"\.screenshots"), enumScreenshotsImage);
                        }

                        if(RemoveUnusedScreenshotsCount == 0)
                        {
                            // 找不到需要清理的未使用的截图
                            MessageBox.Show(App.Current.MainWindow, ResxIF.GetString("NoUnusedScreenshotsFound"), ResxIF.GetString("PronptText"), MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            RefreshCommand.Execute(null);
                            // {0}个未使用的截图已经被成功移除
                            MessageBox.Show(App.Current.MainWindow, string.Format(ResxIF.GetString("UnusedScreenshotsSuccessfullyRemoved"), RemoveUnusedScreenshotsCount), ResxIF.GetString("PronptText"), MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                    }));
            }
        }


        private bool enumScreenshotsImage(object item, object param)
        {
            if (item is FileInfo)
            {
                bool contains = Common.DirectoryChildrenForEach(new DirectoryInfo(ProjectPath), checkScreenshotsImage,item);
                if(!contains)
                {
                    //如果找不到，说明没有人引用当前这个图片，直接删除它即可
                    var fi = item as FileInfo;
                    try
                    {
                        fi.Delete();

                        RemoveUnusedScreenshotsCount++;
                    }
                    catch (Exception err)
                    {
                        //可能是刚创建的活动引用了图片，然后活动被删除了
                        Logger.Debug(err, logger);
                    }

                }
            }

            return false;
        }

        private bool checkScreenshotsImage(object item, object param)
        {
            if (item is FileInfo)
            {
                var fi_img = param as FileInfo;
                var fi_xaml = item as FileInfo;
                if(fi_xaml.Extension.ToLower() == ".xaml")
                {
                    //判断xaml对应的文件里能否找到fi_img的文件名，找到了说明被引用了，直接返回true
                    if(Common.IsStringInFile(fi_xaml.FullName, "\"" + fi_img.Name + "\""))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        //树节点拖拽功能
        public ProjectDropHandler ProjectDropHandler { get; set; } = new ProjectDropHandler();

        public ProjectDragHandler ProjectDragHandler { get; set; } = new ProjectDragHandler();

       
    }
}
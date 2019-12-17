using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Librarys;
using RPAStudio.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace RPAStudio.ViewModel
{
    public class ProjectTreeItem : ViewModelBase
    {
        public static Dictionary<string, bool> IsExpandedDict = new Dictionary<string, bool>();
        private ProjectViewModel m_projectViewModel;

        public bool IsProjectJson { get; internal set; }

        public bool IsRoot { get; set; }

        public ProjectTreeItem(ProjectViewModel projectViewModel)
        {
            m_projectViewModel = projectViewModel;
        }


        /// <summary>
        /// The <see cref="IsDependRoot" /> property's name.
        /// </summary>
        public const string IsDependRootPropertyName = "IsDependRoot";

        private bool _isDependRootProperty = false;

        /// <summary>
        /// Sets and gets the IsDependRoot property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDependRoot
        {
            get
            {
                return _isDependRootProperty;
            }

            set
            {
                if (_isDependRootProperty == value)
                {
                    return;
                }

                _isDependRootProperty = value;
                RaisePropertyChanged(IsDependRootPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsDependItem" /> property's name.
        /// </summary>
        public const string IsDependItemPropertyName = "IsDependItem";

        private bool _isDependItemProperty = false;

        /// <summary>
        /// Sets and gets the IsDependItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDependItem
        {
            get
            {
                return _isDependItemProperty;
            }

            set
            {
                if (_isDependItemProperty == value)
                {
                    return;
                }

                _isDependItemProperty = value;
                RaisePropertyChanged(IsDependItemPropertyName);
            }
        }





        /// <summary>
        /// The <see cref="IsXaml" /> property's name.
        /// </summary>
        public const string IsXamlPropertyName = "IsXaml";

        private bool _isXamlProperty = false;

        /// <summary>
        /// Sets and gets the IsXaml property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsXaml
        {
            get
            {
                return _isXamlProperty;
            }

            set
            {
                if (_isXamlProperty == value)
                {
                    return;
                }

                _isXamlProperty = value;
                RaisePropertyChanged(IsXamlPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsPython" /> property's name.
        /// </summary>
        public const string IsPythonPropertyName = "IsPython";

        private bool _isPythonProperty = false;

        /// <summary>
        /// Sets and gets the IsPython property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsPython
        {
            get
            {
                return _isPythonProperty;
            }

            set
            {
                if (_isPythonProperty == value)
                {
                    return;
                }

                _isPythonProperty = value;
                RaisePropertyChanged(IsPythonPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsExpanded" /> property's name.
        /// </summary>
        public const string IsExpandedPropertyName = "IsExpanded";

        private bool _isExpandedProperty = false;

        /// <summary>
        /// Sets and gets the IsExpanded property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                if(IsExpandedDict.ContainsKey(Path))
                {
                    return IsExpandedDict[Path];
                }

                return _isExpandedProperty;
            }

            set
            {
                if(!string.IsNullOrEmpty(Path))
                {
                    IsExpandedDict[Path] = value;
                }
                

                if (_isExpandedProperty == value)
                {
                    return;
                }

                _isExpandedProperty = value;
                RaisePropertyChanged(IsExpandedPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelectedProperty = false;

        /// <summary>
        /// Sets and gets the IsSelected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelectedProperty;
            }

            set
            {
                if (_isSelectedProperty == value)
                {
                    return;
                }

                _isSelectedProperty = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="Children" /> property's name.
        /// </summary>
        public const string ChildrenPropertyName = "Children";

        private ObservableCollection<ProjectTreeItem> _childrenProperty = new ObservableCollection<ProjectTreeItem>();

        /// <summary>
        /// Sets and gets the Children property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ProjectTreeItem> Children
        {
            get
            {
                return _childrenProperty;
            }

            set
            {
                if (_childrenProperty == value)
                {
                    return;
                }

                _childrenProperty = value;
                RaisePropertyChanged(ChildrenPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsMain" /> property's name.
        /// </summary>
        public const string IsMainPropertyName = "IsMain";

        private bool _isMainProperty = false;

        /// <summary>
        /// Sets and gets the IsMain property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsMain
        {
            get
            {
                return _isMainProperty;
            }

            set
            {
                if (_isMainProperty == value)
                {
                    return;
                }

                _isMainProperty = value;
                RaisePropertyChanged(IsMainPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _nameProperty = "";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _nameProperty;
            }

            set
            {
                if (_nameProperty == value)
                {
                    return;
                }

                _nameProperty = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="ToolTip" /> property's name.
        /// </summary>
        public const string ToolTipPropertyName = "ToolTip";

        private string _toolTipProperty = null;

        /// <summary>
        /// Sets and gets the ToolTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ToolTip
        {
            get
            {
                return _toolTipProperty;
            }

            set
            {
                if (_toolTipProperty == value)
                {
                    return;
                }

                _toolTipProperty = value;
                RaisePropertyChanged(ToolTipPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Icon" /> property's name.
        /// </summary>
        public const string IconPropertyName = "Icon";

        private string _iconProperty = null;

        /// <summary>
        /// Sets and gets the Icon property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Icon
        {
            get
            {
                return _iconProperty;
            }

            set
            {
                if (_iconProperty == value)
                {
                    return;
                }

                _iconProperty = value;
                RaisePropertyChanged(IconPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IconSource" /> property's name.
        /// </summary>
        public const string IconSourcePropertyName = "IconSource";

        private ImageSource _iconSourceProperty = null;

        /// <summary>
        /// Sets and gets the IconSource property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ImageSource IconSource
        {
            get
            {
                return _iconSourceProperty;
            }

            set
            {
                if (_iconSourceProperty == value)
                {
                    return;
                }

                _iconSourceProperty = value;
                RaisePropertyChanged(IconSourcePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsFile" /> property's name.
        /// </summary>
        public const string IsFilePropertyName = "IsFile";

        private bool _isFileProperty = false;

        /// <summary>
        /// Sets and gets the IsFile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsFile
        {
            get
            {
                return _isFileProperty;
            }

            set
            {
                if (_isFileProperty == value)
                {
                    return;
                }

                _isFileProperty = value;
                RaisePropertyChanged(IsFilePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsDirectory" /> property's name.
        /// </summary>
        public const string IsDirectoryPropertyName = "IsDirectory";

        private bool _isDirectoryProperty = false;

        /// <summary>
        /// Sets and gets the IsDirectory property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDirectory
        {
            get
            {
                return _isDirectoryProperty;
            }

            set
            {
                if (_isDirectoryProperty == value)
                {
                    return;
                }

                _isDirectoryProperty = value;
                RaisePropertyChanged(IsDirectoryPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="Path" /> property's name.
        /// </summary>
        public const string PathPropertyName = "Path";

        private string _pathProperty = "";

        /// <summary>
        /// Sets and gets the Path property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Path
        {
            get
            {
                return _pathProperty;
            }

            set
            {
                if (_pathProperty == value)
                {
                    return;
                }

                _pathProperty = value;
                RaisePropertyChanged(PathPropertyName);
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
            }
        }



        /// <summary>
        /// The <see cref="IsSearching" /> property's name.
        /// </summary>
        public const string IsSearchingPropertyName = "IsSearching";

        private bool _isSearchingProperty = false;

        /// <summary>
        /// Sets and gets the IsSearching property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSearching
        {
            get
            {
                return _isSearchingProperty;
            }

            set
            {
                if (_isSearchingProperty == value)
                {
                    return;
                }

                _isSearchingProperty = value;
                RaisePropertyChanged(IsSearchingPropertyName);
            }
        }







        /// <summary>
        /// The <see cref="IsMatch" /> property's name.
        /// </summary>
        public const string IsMatchPropertyName = "IsMatch";

        private bool _isMatchProperty = false;

        /// <summary>
        /// Sets and gets the IsMatch property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsMatch
        {
            get
            {
                return _isMatchProperty;
            }

            set
            {
                if (_isMatchProperty == value)
                {
                    return;
                }

                _isMatchProperty = value;
                RaisePropertyChanged(IsMatchPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="ContextMenuName" /> property's name.
        /// </summary>
        public const string ContextMenuNamePropertyName = "ContextMenuName";

        private string _contextMenuNameProperty = "";

        /// <summary>
        /// Sets and gets the ContextMenuName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ContextMenuName
        {
            get
            {
                return _contextMenuNameProperty;
            }

            set
            {
                if (_contextMenuNameProperty == value)
                {
                    return;
                }

                _contextMenuNameProperty = value;
                RaisePropertyChanged(ContextMenuNamePropertyName);
            }
        }



        private RelayCommand<MouseButtonEventArgs> _treeNodeMouseLeftButtonDownCommand;

        /// <summary>
        /// Gets the TreeNodeMouseLeftButtonDownCommand.
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> TreeNodeMouseLeftButtonDownCommand
        {
            get
            {
                return _treeNodeMouseLeftButtonDownCommand
                    ?? (_treeNodeMouseLeftButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(
                    p =>
                    {
                        if (!IsXaml)
                        {
                            return;
                        }

                        var treeViewItem = UIUtils.VisualUpwardSearch<TreeViewItem>(p.OriginalSource as DependencyObject) as TreeViewItem;
                        if (treeViewItem == null)
                        {
                            return;
                        }

                    }));
            }
        }






        private RelayCommand<MouseButtonEventArgs> _treeNodeMouseRightButtonDownCommand;

        /// <summary>
        /// Gets the TreeNodeMouseRightButtonDownCommand.
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> TreeNodeMouseRightButtonDownCommand
        {
            get
            {
                return _treeNodeMouseRightButtonDownCommand
                    ?? (_treeNodeMouseRightButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(
                    p =>
                    {
                        var treeViewItem = UIUtils.VisualUpwardSearch<TreeViewItem>(p.OriginalSource as DependencyObject) as TreeViewItem;
                        if (treeViewItem != null)
                        {
                            treeViewItem.Focus();
                        }
                    }));
            }
        }


        private RelayCommand<MouseButtonEventArgs> _treeNodeMouseRightButtonUpCommand;

        /// <summary>
        /// Gets the TreeNodeMouseRightButtonUpCommand.
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> TreeNodeMouseRightButtonUpCommand
        {
            get
            {
                return _treeNodeMouseRightButtonUpCommand
                    ?? (_treeNodeMouseRightButtonUpCommand = new RelayCommand<MouseButtonEventArgs>(
                    p =>
                    {
                        //控件右击动态弹出菜单
                        if(!string.IsNullOrEmpty(ContextMenuName))
                        {
                            var view = App.Current.MainWindow;
                            var cm = view.FindResource(ContextMenuName) as ContextMenu;
                            cm.DataContext = this;
                            cm.Placement = PlacementMode.MousePoint;
                            cm.IsOpen = true;

                            RaiseCanExecuteChanged();
                        }
                        
                    }));
            }
        }

        private void RaiseCanExecuteChanged()
        {
            RenameDirCommand.RaiseCanExecuteChanged();
            DeleteDirCommand.RaiseCanExecuteChanged();
            NewFolderCommand.RaiseCanExecuteChanged();

            RenameFileCommand.RaiseCanExecuteChanged();
            DeleteFileCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();
            SetAsMainCommand.RaiseCanExecuteChanged();
        }

        private RelayCommand<MouseButtonEventArgs> _treeNodeMouseDoubleClickCommand;
       
        /// <summary>
        /// Gets the TreeNodeMouseDoubleClickCommand.
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> TreeNodeMouseDoubleClickCommand
        {
            get
            {
                return _treeNodeMouseDoubleClickCommand
                    ?? (_treeNodeMouseDoubleClickCommand = new RelayCommand<MouseButtonEventArgs>(
                    p =>
                    {
                        if (IsXaml)
                        {
                            OpenXamlCommand.Execute(null);
                        }

                    }));
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

        public void ApplyCriteria(string criteria, Stack<ProjectTreeItem> ancestors)
        {
            SearchText = criteria;

            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                IsExpanded = true;

                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = true;
                    ancestor.IsExpanded = true;
                }

                //如果是组名匹配，则下面的子节点和子子等节点要把IsMatch都设置为true
                ProjectTreeItemSetAllIsMatch(this, true);
            }

            ancestors.Push(this);
            foreach (var child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }

        private bool IsCriteriaMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || Name.ContainsIgnoreCase(criteria);
        }



        private RelayCommand _openDirCommand;

        /// <summary>
        /// Gets the OpenDirCommand.
        /// </summary>
        public RelayCommand OpenDirCommand
        {
            get
            {
                return _openDirCommand
                    ?? (_openDirCommand = new RelayCommand(
                    () =>
                    {
                        Common.LocateDirInExplorer(Path);
                    }));
            }
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
                        m_projectViewModel.OpenProjectSettingsCommand.Execute(null);
                    }));
            }
        }



        private RelayCommand _newFolderCommand;

        /// <summary>
        /// Gets the NewFolderCommand.
        /// </summary>
        public RelayCommand NewFolderCommand
        {
            get
            {
                return _newFolderCommand
                    ?? (_newFolderCommand = new RelayCommand(
                    () =>
                    {
                        var window = new NewFolderWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as NewFolderViewModel;
                        vm.Path = Path;
                        vm.FolderName = Common.GetValidDirectoryName(Path, "新建文件夹");
                        window.ShowDialog();
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging));
            }
        }

        private RelayCommand _importWorkflowCommand;

        /// <summary>
        /// Gets the ImportWorkflowCommand.
        /// </summary>
        public RelayCommand ImportWorkflowCommand
        {
            get
            {
                return _importWorkflowCommand
                    ?? (_importWorkflowCommand = new RelayCommand(
                    () =>
                    {
                        //导入工作流
                        var fileList = Common.ShowSelectMultiFileDialog("工作流文件|*.xaml","选择工作流文件进行导入");

                        foreach (var item in fileList)
                        {
                            var sourceFileName = System.IO.Path.GetFileName(item);
                            var sourceFileDir = System.IO.Path.GetDirectoryName(item);
                            var sourcePath = item;
                            var targetPath = Path+ @"\"+ sourceFileName;
                            if(System.IO.File.Exists(targetPath))
                            {
                                targetPath = Path + @"\" + Common.GetValidFileName(Path, sourceFileName, " - Imported");
                            }

                            System.IO.File.Copy(sourcePath, targetPath, false);
                        }

                        m_projectViewModel.RefreshCommand.Execute(null);
                    }));
            }
        }




        private RelayCommand _renameDirCommand;

        /// <summary>
        /// Gets the RenameDirCommand.
        /// </summary>
        public RelayCommand RenameDirCommand
        {
            get
            {
                return _renameDirCommand
                    ?? (_renameDirCommand = new RelayCommand(
                    () =>
                    {
                        //重命名目录
                        var window = new RenameWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as RenameViewModel;
                        vm.Path = Path;
                        vm.Dir = System.IO.Path.GetDirectoryName(Path);
                        vm.SrcName = Name;
                        vm.DstName = Common.GetValidDirectoryName(vm.Dir,Common.GetDirectoryNameWithoutSuffixFormat(vm.SrcName));
                        vm.IsDirectory = true;
                        window.ShowDialog();
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging && !(Name == ".screenshots" && IsDirectory)  ));
            }
        }




        private RelayCommand _deleteDirCommand;

        /// <summary>
        /// Gets the DeleteDirCommand.
        /// </summary>
        public RelayCommand DeleteDirCommand
        {
            get
            {
                return _deleteDirCommand
                    ?? (_deleteDirCommand = new RelayCommand(
                    () =>
                    {
                        //删除目录
                        var ret = MessageBox.Show(App.Current.MainWindow, string.Format("确认删除目录\"{0}\"和它的所有内容吗？",Path),"询问", MessageBoxButton.OKCancel, MessageBoxImage.Question,MessageBoxResult.Cancel);
                        if(ret == MessageBoxResult.OK)
                        {
                            Common.DeleteDir(Path);

                            m_projectViewModel.RefreshCommand.Execute(null);

                            Messenger.Default.Send(this, "Delete");

                        }
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging));
            }
        }


        private RelayCommand _renameFileCommand;

        /// <summary>
        /// Gets the RenameFileCommand.
        /// </summary>
        public RelayCommand RenameFileCommand
        {
            get
            {
                return _renameFileCommand
                    ?? (_renameFileCommand = new RelayCommand(
                    () =>
                    {
                        //重命名文件
                        var window = new RenameWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as RenameViewModel;
                        vm.Path = Path;
                        vm.Dir = System.IO.Path.GetDirectoryName(Path);
                        vm.SrcName = Name;
                        vm.DstName = Common.GetValidFileName(vm.Dir, Common.GetFileNameWithoutSuffixFormat(vm.SrcName));
                        vm.IsDirectory = false;
                        vm.IsMain = IsMain;
                        window.ShowDialog();
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging));
            }
        }




        private RelayCommand _deleteFileCommand;

        /// <summary>
        /// Gets the DeleteFileCommand.
        /// </summary>
        public RelayCommand DeleteFileCommand
        {
            get
            {
                return _deleteFileCommand
                    ?? (_deleteFileCommand = new RelayCommand(
                    () =>
                    {
                        var ret = MessageBox.Show(App.Current.MainWindow, string.Format("确认删除文件\"{0}\"吗？", Path), "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                        if (ret == MessageBoxResult.OK)
                        {
                            Common.DeleteFile(Path);

                            m_projectViewModel.RefreshCommand.Execute(null);

                            Messenger.Default.Send(this, "Delete");
                        }
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging));
            }
        }


        private RelayCommand _openXamlCommand;

        /// <summary>
        /// Gets the OpenXamlCommand.
        /// </summary>
        public RelayCommand OpenXamlCommand
        {
            get
            {
                return _openXamlCommand
                    ?? (_openXamlCommand = new RelayCommand(
                    () =>
                    {
                        //判断是否存在，若存在，直接切换，否则打开新文档

                        bool isExist = false;
                        foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
                        {
                            if (doc.XamlPath == Path)
                            {
                                isExist = true;
                                doc.IsSelected = true;
                                break;
                            }
                        }

                        if (!isExist)
                        {
                            var NameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(Name);

                            bool isReadOnly = ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging;//程序正在运行或调试时，新打开的XAML应为只读
                            ViewModelLocator.Instance.Dock.NewSequenceDocument(NameWithoutExt, Path, isReadOnly);
                        }

                    }));
            }
        }






        private RelayCommand _runCommand;

        /// <summary>
        /// Gets the RunCommand.
        /// </summary>
        public RelayCommand RunCommand
        {
            get
            {
                return _runCommand
                    ?? (_runCommand = new RelayCommand(
                    () =>
                    {
                        //先打开相应的文档，再运行
                        OpenXamlCommand.Execute(null);
                        ViewModelLocator.Instance.Main.RunWorkflowCommand.Execute(null);
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging));
            }
        }




        private RelayCommand _setAsMainCommand;
        

        /// <summary>
        /// Gets the SetAsMainCommand.
        /// </summary>
        public RelayCommand SetAsMainCommand
        {
            get
            {
                return _setAsMainCommand
                    ?? (_setAsMainCommand = new RelayCommand(
                    () =>
                    {
                        //设置当前xaml为Main
                        var relativeMainXaml = Common.MakeRelativePath(m_projectViewModel.ProjectPath, Path);
                        updateProjectJsonMain(relativeMainXaml);

                        m_projectViewModel.RefreshCommand.Execute(null);
                    },
                    () => !ViewModelLocator.Instance.Main.IsWorkflowRunningOrDebugging));
            }
        }

        private void updateProjectJsonMain(string mainFile)
        {
            //json更新
            string json = File.ReadAllText(m_projectViewModel.CurrentProjectJsonFile);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["main"] = mainFile;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(m_projectViewModel.CurrentProjectJsonFile, output);
        }


















    }
}
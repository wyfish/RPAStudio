using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Librarys;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RPAStudio.ViewModel
{
    public class SnippetTreeItem : ViewModelBase
    {
        private static TreeViewItem previouseLeftButtonDownTreeViewItem;


        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private string _idProperty = System.Guid.NewGuid().ToString();

        /// <summary>
        /// Sets and gets the Id property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Id
        {
            get
            {
                return _idProperty;
            }

            set
            {
                if (_idProperty == value)
                {
                    return;
                }

                _idProperty = value;
                RaisePropertyChanged(IdPropertyName);
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
                return _isExpandedProperty;
            }

            set
            {
                if (_isExpandedProperty == value)
                {
                    return;
                }

                _isExpandedProperty = value;
                RaisePropertyChanged(IsExpandedPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="Children" /> property's name.
        /// </summary>
        public const string ChildrenPropertyName = "Children";

        private ObservableCollection<SnippetTreeItem> _childrenProperty = new ObservableCollection<SnippetTreeItem>();

        /// <summary>
        /// Sets and gets the Children property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<SnippetTreeItem> Children
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
        /// The <see cref="IsSnippet" /> property's name.
        /// </summary>
        public const string IsSnippetPropertyName = "IsSnippet";

        private bool _isSnippetProperty = false;

        /// <summary>
        /// Sets and gets the IsSnippet property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSnippet
        {
            get
            {
                return _isSnippetProperty;
            }

            set
            {
                if (_isSnippetProperty == value)
                {
                    return;
                }

                _isSnippetProperty = value;
                RaisePropertyChanged(IsSnippetPropertyName);
            }
        }


        /// <summary>
        /// 是否是用户手动添加进来的目录，右键菜单有移除选项
        /// </summary>
        public const string IsUserAddPropertyName = "IsUserAdd";

        private bool _isUserAddProperty = false;

        /// <summary>
        /// Sets and gets the IsUserAdd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsUserAdd
        {
            get
            {
                return _isUserAddProperty;
            }

            set
            {
                if (_isUserAddProperty == value)
                {
                    return;
                }

                _isUserAddProperty = value;
                RaisePropertyChanged(IsUserAddPropertyName);
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
                        previouseLeftButtonDownTreeViewItem = null;

                        if (!IsSnippet)
                        {
                            return;
                        }

                        var treeViewItem = UIUtils.VisualUpwardSearch<TreeViewItem>(p.OriginalSource as DependencyObject) as TreeViewItem;
                        if (treeViewItem == null)
                        {
                            return;
                        }

                        previouseLeftButtonDownTreeViewItem = treeViewItem;
                    }));
            }
        }



        private RelayCommand<MouseEventArgs> _treeNodeMouseMoveCommand;

        /// <summary>
        /// Gets the TreeNodeMouseMoveCommand.
        /// </summary>
        public RelayCommand<MouseEventArgs> TreeNodeMouseMoveCommand
        {
            get
            {
                return _treeNodeMouseMoveCommand
                    ?? (_treeNodeMouseMoveCommand = new RelayCommand<MouseEventArgs>(
                    p =>
                    {
                        if (p.LeftButton == MouseButtonState.Pressed)
                        {
                            if (!IsSnippet)
                            {
                                return;
                            }

                            var treeViewItem = UIUtils.VisualUpwardSearch<TreeViewItem>(p.OriginalSource as DependencyObject) as TreeViewItem;
                            if (treeViewItem == null)
                            {
                                return;
                            }

                            if (treeViewItem != previouseLeftButtonDownTreeViewItem)
                            {
                                return;
                            }

                            if (Path != "" && ViewModelLocator.Instance.Dock.ActiveDocument != null)
                            {
                                var designer = ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance;

                                Activity activity = getActivityWithoutDebugSymbol(Path);
 
                                Activity dragActivity = null;
                                if(activity is DynamicActivity)
                                {
                                    dragActivity = (activity as DynamicActivity).Implementation();
                                }
                                else
                                {
                                    dragActivity = activity;
                                }

                                Activity resultActivity = dragActivity;
                                if (Common.IsWorkflowDesignerEmpty(designer))
                                {
                                    resultActivity = Common.ProcessAutoSurroundWithSequence(dragActivity);
                                }
                                if (resultActivity != null)
                                {
                                    //ModelItem mi = designer.Context.Services.GetService<ModelTreeManager>().CreateModelItem(null, resultActivity);
                                    //DataObject data = new DataObject(DragDropHelper.ModelItemDataFormat, mi);
                                    //DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.All);

                                    //动态生成组件，以便解决拖动到flowchart报错的问题
                                    var dag = new DynamicActivityGenerator("_DynamicActivityGenerator_" + System.Guid.NewGuid().ToString());
                                    var t = dag.AppendSubWorkflowTemplate(System.Guid.NewGuid().ToString(), Common.ToXaml(resultActivity));
                                    try
                                    {
                                        dag.Save();
                                    }
                                    catch (Exception )
                                    {
                                    }

                                    DataObject data = new DataObject(System.Activities.Presentation.DragDropHelper.WorkflowItemTypeNameFormat, t.AssemblyQualifiedName);
                                    DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.All);
                                }
                                
                            }

                        }
                    }));
            }
        }

        private Activity getActivityWithoutDebugSymbol(string path)
        {
            //去除DebugSymbol.Symbol信息，否则拖动代码片断到新建立的XAML文档时无法运行
            var xamlContent = File.ReadAllText(path);
            XDocument xd = XDocument.Parse(xamlContent);
            XElement element = xd.XPathSelectElement("//*[local-name()='DebugSymbol.Symbol']");
            if (element != null)
            {
                element.SetValue(string.Empty);
            }
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;
                xws.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(stream, xws))
                {
                    xd.WriteTo(writer);
                    writer.Flush();
                    stream.Position = 0;
                    var activity = ActivityXamlServices.Load(stream);
                    return activity;
                }
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
                        var view = App.Current.MainWindow;
                        var cm = IsUserAdd && !IsSnippet ? view.FindResource("SnippetsUserAddContextMenu") as ContextMenu : view.FindResource("SnippetsContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        cm.Placement = PlacementMode.MousePoint;
                        cm.IsOpen = true;
                    }));
            }
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
                        if (IsSnippet)
                        {
                            OpenSnippetCommand.Execute(null);
                        }

                    }));
            }
        }




        private void SnippetTreeItemSetAllIsMatch(SnippetTreeItem item, bool IsMatch)
        {
            item.IsMatch = IsMatch;
            foreach (var child in item.Children)
            {
                SnippetTreeItemSetAllIsMatch(child, IsMatch);
            }
        }

        public void ApplyCriteria(string criteria, Stack<SnippetTreeItem> ancestors)
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
                SnippetTreeItemSetAllIsMatch(this, true);
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




        private RelayCommand _openSnippetCommand;

        /// <summary>
        /// Gets the OpenSnippetCommand.
        /// </summary>
        public RelayCommand OpenSnippetCommand
        {
            get
            {
                return _openSnippetCommand
                    ?? (_openSnippetCommand = new RelayCommand(
                    () =>
                    {
                        if(IsSnippet)
                        {
                            //打开代码片断文件
                            //判断是否存在，若存在，直接切换，否则打开新文档

                            bool isExist = false;
                            foreach(var doc in ViewModelLocator.Instance.Dock.Documents)
                            {
                                if(doc.XamlPath == Path)
                                {
                                    isExist = true;
                                    doc.IsSelected = true;
                                    break;
                                }
                            }

                            if(!isExist)
                            {
                                var NameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(Name);
                                ViewModelLocator.Instance.Dock.NewProcessDocument(NameWithoutExt, Path, true,true);
                            }
                            
                        }
                        else
                        {
                            //打开文件夹
                            Common.LocateDirInExplorer(Path);
                        }
                    }));
            }
        }



        private RelayCommand _removeSnippetCommand;

        /// <summary>
        /// Gets the RemoveSnippetCommand.
        /// </summary>
        public RelayCommand RemoveSnippetCommand
        {
            get
            {
                return _removeSnippetCommand
                    ?? (_removeSnippetCommand = new RelayCommand(
                    () =>
                    {
                        //移除用户选择的目录
                        XmlDocument doc = new XmlDocument();
                        var path = App.LocalRPAStudioDir + @"\Config\CodeSnippets.xml";
                        doc.Load(path);
                        var rootNode = doc.DocumentElement;

                        var directoryNodes = rootNode.SelectNodes("Directory");
                        foreach (XmlNode dir in directoryNodes)
                        {
                            var id = (dir as XmlElement).GetAttribute("Id");

                            if(Id == id)
                            {
                                rootNode.RemoveChild(dir);
                                break;
                            }
                        }

                        doc.Save(path);


                        //发消息通知代码片断视图刷新树节点
                        Messenger.Default.Send(this, "RemoveSnippet");
                    }));
            }
        }








    }

    

}
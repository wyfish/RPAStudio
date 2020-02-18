using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Librarys;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Activities.Presentation.Model;
using System.Activities.Presentation;
using System.Activities;
using log4net;

namespace RPAStudio.ViewModel
{
    public class ActivityTreeItem : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string NamePinYinAbbr { get; set; }//拼音首字母
        public string NamePinYin { get; set; }//全拼

        private static TreeViewItem previouseLeftButtonDownTreeViewItem;

        //该枚举标记了分组类型，也标记了顺序
        public enum enGroupType
        {
            Null = 0,
            Favorite,
            Recent,
            Available
        }

        public enGroupType GroupType = enGroupType.Null;//标记当前节点的组类型

       /// <summary>
       /// 可选的活动树里对应的父亲
       /// </summary>
        public ActivityTreeItem Parent { get; set; }

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
        /// The <see cref="IsSortable" /> property's name.
        /// </summary>
        public const string IsSortablePropertyName = "IsSortable";

        private bool? _isSortableProperty = false;

        /// <summary>
        /// Sets and gets the IsSortable property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool? IsSortable
        {
            get
            {
                return _isSortableProperty;
            }

            set
            {
                if (_isSortableProperty == value)
                {
                    return;
                }

                _isSortableProperty = value;
                RaisePropertyChanged(IsSortablePropertyName);
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

                IsToolTipEnabled = !string.IsNullOrEmpty(value);
            }
        }



        /// <summary>
        /// The <see cref="IsToolTipEnabled" /> property's name.
        /// </summary>
        public const string IsToolTipEnabledPropertyName = "IsToolTipEnabled";

        private bool _isToolTipEnabledProperty = false;

        /// <summary>
        /// Sets and gets the IsToolTipEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsToolTipEnabled
        {
            get
            {
                return _isToolTipEnabledProperty;
            }

            set
            {
                if (_isToolTipEnabledProperty == value)
                {
                    return;
                }

                _isToolTipEnabledProperty = value;
                RaisePropertyChanged(IsToolTipEnabledPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsFavorite" /> property's name.
        /// </summary>
        public const string IsFavoritePropertyName = "IsFavorite";

        private bool _isFavoriteProperty = false;

        /// <summary>
        /// Sets and gets the IsFavorite property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsFavorite
        {
            get
            {
                return _isFavoriteProperty;
            }

            set
            {
                if (_isFavoriteProperty == value)
                {
                    return;
                }

                _isFavoriteProperty = value;
                RaisePropertyChanged(IsFavoritePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsSystem" /> property's name.
        /// </summary>
        public const string IsSystemPropertyName = "IsSystem";

        private bool _isSystemProperty = false;

        /// <summary>
        /// Sets and gets the IsSystem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSystem
        {
            get
            {
                return _isSystemProperty;
            }

            set
            {
                if (_isSystemProperty == value)
                {
                    return;
                }

                _isSystemProperty = value;
                RaisePropertyChanged(IsSystemPropertyName);
            }
        }


        

        /// <summary>
        /// The <see cref="SystemIcon" /> property's name.
        /// </summary>
        public const string SystemIconPropertyName = "SystemIcon";

        private BitmapImage _systemIconProperty = null;

        /// <summary>
        /// Sets and gets the SystemIcon property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public BitmapImage SystemIcon
        {
            get
            {
                if(MainViewModel.SystemIconDic.ContainsKey(TypeOf))
                {
                    _systemIconProperty = MainViewModel.SystemIconDic[TypeOf];
                }
                
                return _systemIconProperty;
            }
        }


        /// <summary>
        /// The <see cref="Icon" /> property's name.
        /// </summary>
        public const string IconPropertyName = "Icon";

        private const string _iconDefaultProperty = "pack://application:,,,/Resources/Image/Dock/activities.png";

        private string _iconProperty = _iconDefaultProperty;

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
                if (value == "")
                {
                    value = _iconDefaultProperty;
                }

                if (_iconProperty == value)
                {
                    return;
                }

                if (value.StartsWith("pack://"))
                {
                    _iconProperty = value;
                }
                else
                {
                    var arr = TypeOf.Split(',');
                    if(arr.Length == 2)
                    {
                        var assemblyName = arr[1];
                        _iconProperty = $"pack://application:,,,/{assemblyName};Component/" + value;
                    }
                }

                RaisePropertyChanged(IconPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsActivity" /> property's name.
        /// </summary>
        public const string IsActivityPropertyName = "IsActivity";

        private bool _isActivityProperty = false;

        /// <summary>
        /// Sets and gets the IsActivity property.
        /// 是否是组件节点 
        /// </summary>
        public bool IsActivity
        {
            get
            {
                return _isActivityProperty;
            }

            set
            {
                if (_isActivityProperty == value)
                {
                    return;
                }

                _isActivityProperty = value;
                RaisePropertyChanged(IsActivityPropertyName);
            }
        }

        

        /// <summary>
        /// The <see cref="Children" /> property's name.
        /// </summary>
        public const string ChildrenPropertyName = "Children";

        private ObservableCollection<ActivityTreeItem> _childrenProperty = new ObservableCollection<ActivityTreeItem>();

        /// <summary>
        /// Sets and gets the Children property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ActivityTreeItem> Children
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
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _descriptionProperty = null;

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Description
        {
            get
            {
                return _descriptionProperty;
            }

            set
            {
                if (_descriptionProperty == value)
                {
                    return;
                }

                _descriptionProperty = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }



        

        
        /// <summary>
        /// The <see cref="TypeOf" /> property's name.
        /// </summary>
        public const string TypeOfPropertyName = "TypeOf";

        private string _typeOfProperty = "";

        /// <summary>
        /// Sets and gets the TypeOf property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TypeOf
        {
            get
            {
                return _typeOfProperty;
            }

            set
            {
                if (_typeOfProperty == value)
                {
                    return;
                }

                _typeOfProperty = value;
                RaisePropertyChanged(TypeOfPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="AssemblyQualifiedName" /> property's name.
        /// </summary>
        public const string AssemblyQualifiedNamePropertyName = "AssemblyQualifiedName";

        private string _assemblyQualifiedNameProperty = "";

        /// <summary>
        /// Sets and gets the AssemblyQualifiedName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string AssemblyQualifiedName
        {
            get
            {
                if(_assemblyQualifiedNameProperty == "" && TypeOf != "")
                {
                    //var test_type = typeof(System.Activities.Core.Presentation.Factories.ForEachWithBodyFactory<object>);
                    //var test_type_assembly_qualified_name = test_type.AssemblyQualifiedName.Replace(" ", "");

                    Type type = null;
                    if (IsSystem)
                    {
                        if(TypeOf == "FinalState")
                        {
                            //特殊处理
                            type = typeof(System.Activities.Core.Presentation.FinalState);
                        }else if(TypeOf == "ForEach")
                        {
                            type = typeof(System.Activities.Core.Presentation.Factories.ForEachWithBodyFactory<object>);
                        }else if(TypeOf == "ParallelForEach")
                        {
                            type = typeof(System.Activities.Core.Presentation.Factories.ParallelForEachWithBodyFactory<object>); 
                        }else if(TypeOf == "Switch")
                        {
                            type = typeof(System.Activities.Statements.Switch<string>);
                        }
                        else
                        {
                            type = Type.GetType(string.Format("System.Activities.Statements.{0},System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", TypeOf));
                            if (type == null)
                            {
                                type = Type.GetType(string.Format("System.Activities.Statements.{0}`1[[System.Object,mscorlib,Version=4.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089]],System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", TypeOf));
                            }

                            if (type == null)
                            {
                                type = Type.GetType(string.Format("System.Activities.Statements.{0}`2[[System.Object,mscorlib,Version=4.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089]],System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", TypeOf));
                            }
                        }
                    }
                    else
                    {
                        type = Type.GetType(TypeOf);
                        if (type == null)
                        {
                            //可能有泛型，添加`x再尝试下
                            string[] sArray = TypeOf.Split(',');
                            if(sArray.Length > 1)
                            {
                                sArray[0] += "`1[[System.Object,mscorlib,Version=4.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089]]";
                            }
                            type = Type.GetType(string.Join(",", sArray));
                        }

                        if (type == null)
                        {
                            //可能有泛型，添加`x再尝试下
                            string[] sArray = TypeOf.Split(',');
                            if (sArray.Length > 1)
                            {
                                sArray[0] += "`2[[System.Object,mscorlib,Version=4.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089]]";
                            }
                            type = Type.GetType(string.Join(",", sArray));
                        }
                    }

                    if (type != null)
                    {
                        _assemblyQualifiedNameProperty = type.AssemblyQualifiedName.Replace(" ","");
                    }
                    else
                    {
                        Logger.Debug(string.Format("{0} 类型未找到！", TypeOf),logger);
                    }


                }

                return _assemblyQualifiedNameProperty;
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
        /// The <see cref="HighlightText" /> property's name.
        /// </summary>
        public const string HighlightTextPropertyName = "HighlightText";

        private string _highlightTextProperty = "";

        /// <summary>
        /// Sets and gets the HighlightText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string HighlightText
        {
            get
            {
                return _highlightTextProperty;
            }

            set
            {
                if (_highlightTextProperty == value)
                {
                    return;
                }

                _highlightTextProperty = value;
                RaisePropertyChanged(HighlightTextPropertyName);
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

                        if (!IsActivity)
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
                            if (!IsActivity)
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

                            if (AssemblyQualifiedName != "")
                            {
                                var designer = ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance;
                               
                                if (Common.IsWorkflowDesignerEmpty(designer))
                                {
                                    var type = Type.GetType(AssemblyQualifiedName);
                                    var dragActivity = Activator.CreateInstance(type) as Activity;

                                    if(dragActivity == null)
                                    {
                                        DataObject _data = new DataObject(System.Activities.Presentation.DragDropHelper.WorkflowItemTypeNameFormat, AssemblyQualifiedName);
                                        DragDrop.DoDragDrop(treeViewItem, _data, DragDropEffects.All);

                                        return;//PickBranch会导致此种情况
                                    }

                                    if (ViewModelLocator.Instance.Activities.ActivityTreeItemAssemblyQualifiedNameDic.ContainsKey(AssemblyQualifiedName))
                                    {
                                        var item = ViewModelLocator.Instance.Activities.ActivityTreeItemAssemblyQualifiedNameDic[AssemblyQualifiedName];
                                        dragActivity.DisplayName = item.Name;
                                    }
                                        
                                    var resultActivity = Common.ProcessAutoSurroundWithSequence(dragActivity);

                                    ModelItem mi = designer.Context.Services.GetService<ModelTreeManager>().CreateModelItem(null, resultActivity);
                                    DataObject data = new DataObject(DragDropHelper.ModelItemDataFormat, mi);
                                    DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.All);
                                }
                                else
                                {
                                    DataObject data = new DataObject(System.Activities.Presentation.DragDropHelper.WorkflowItemTypeNameFormat, AssemblyQualifiedName);
                                    DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.All);
                                }
                            }

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
                        if (!IsActivity)
                        {
                            return;
                        }


                        //控件右击动态弹出菜单
                        var view = App.Current.MainWindow;
                        var cm = IsFavorite? view.FindResource("FavoriteActivitiyContextMenu") as ContextMenu : view.FindResource("ActivitiyContextMenu") as ContextMenu;
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
                        //TODO WJF 双击发送消息，当前激活的文档接收消息，自动添加组件
                    }));
            }
        }


        private RelayCommand _addToFavoritesCommand;

        /// <summary>
        /// Gets the AddToFavoritesCommand.
        /// </summary>
        public RelayCommand AddToFavoritesCommand
        {
            get
            {
                return _addToFavoritesCommand
                    ?? (_addToFavoritesCommand = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(this, "AddToFavorites");
                    }));
            }
        }




        private RelayCommand _removeFromFavoritesCommand;

        /// <summary>
        /// Gets the RemoveFromFavoritesCommand.
        /// </summary>
        public RelayCommand RemoveFromFavoritesCommand
        {
            get
            {
                return _removeFromFavoritesCommand
                    ?? (_removeFromFavoritesCommand = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(this, "RemoveFromFavorites");
                    }));
            }
        }

        private void ActivityTreeItemSetAllIsMatch(ActivityTreeItem item, bool IsMatch)
        {
            item.IsMatch = IsMatch;
            foreach (var child in item.Children)
            {
                ActivityTreeItemSetAllIsMatch(child, IsMatch);
            }
        }

        public void ApplyCriteria(string criteria, Stack<ActivityTreeItem> ancestors)
        {
            SearchText = criteria;
            HighlightText = criteria;

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
                ActivityTreeItemSetAllIsMatch(this, true);
            }

            ancestors.Push(this);
            foreach (var child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }

        private bool IsCriteriaMatched(string criteria)
        {
            bool isFound =  String.IsNullOrEmpty(criteria) || Name.ContainsIgnoreCase(criteria);

            if(!isFound)
            {
                //未找到则按拼音首字母或全拼进行查找
                if (NamePinYinAbbr == null)
                {
                    NamePinYinAbbr = NPinyin.Pinyin.GetInitials(Name);
                }

                if (NamePinYin == null)
                {
                    NamePinYin = NPinyin.Pinyin.GetPinyin(Name).Replace(" ","");
                }

                if (NamePinYinAbbr.ContainsIgnoreCase(criteria))
                {
                    isFound = true;
                }
                else if (NamePinYin.ContainsIgnoreCase(criteria))
                {
                    isFound = true;
                }
            }

            return isFound;
        }


    }
}
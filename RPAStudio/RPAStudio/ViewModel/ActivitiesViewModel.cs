using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using RPAStudio.Librarys;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml;
using System;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Plugins.Shared.Library;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ActivitiesViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //维护全局的AvailableActivities配置信息，动态加载或卸载组件时该配置信息内容会相应的修改
        public XmlDocument AvailableActivitiesXmlDocument { get; set; }

        public Dictionary<string, ActivityTreeItem> ActivityTreeItemTypeOfDic = new Dictionary<string, ActivityTreeItem>();
        public Dictionary<string, ActivityTreeItem> ActivityTreeItemAssemblyQualifiedNameDic = new Dictionary<string, ActivityTreeItem>();

        public ActivityTreeItem ItemFavorites { get; set; }

        public ActivityTreeItem ItemRecent { get; set; }

        public ActivityTreeItem ItemAvailable { get; set; }





        public ActivityTreeItem GetActivityTreeItemByType(Type type)
        {
            var assemblyQualifiedName = type.AssemblyQualifiedName.Replace(" ", "");

            if (ActivityTreeItemAssemblyQualifiedNameDic.ContainsKey(assemblyQualifiedName))
            {
                var activity_item = ViewModelLocator.Instance.Activities.ActivityTreeItemAssemblyQualifiedNameDic[assemblyQualifiedName];
                return activity_item;
            }

            return null;
        }

        /// <summary>
        /// The <see cref="ActivityItems" /> property's name.
        /// </summary>
        public const string ActivityItemsPropertyName = "ActivityItems";

        private ObservableCollection<ActivityTreeItem> _activityItemsProperty = new ObservableCollection<ActivityTreeItem>();

        /// <summary>
        /// Sets and gets the ActivityItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ActivityTreeItem> ActivityItems
        {
            get
            {
                return _activityItemsProperty;
            }

            set
            {
                if (_activityItemsProperty == value)
                {
                    return;
                }

                _activityItemsProperty = value;
                RaisePropertyChanged(ActivityItemsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsShowFavoritesView" /> property's name.
        /// </summary>
        public const string IsShowFavoritesViewPropertyName = "IsShowFavoritesView";

        private bool _isShowFavoritesViewProperty = true;

        /// <summary>
        /// Sets and gets the IsShowFavoritesView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowFavoritesView
        {
            get
            {
                return _isShowFavoritesViewProperty;
            }

            set
            {
                if (_isShowFavoritesViewProperty == value)
                {
                    return;
                }

                _isShowFavoritesViewProperty = value;
                RaisePropertyChanged(IsShowFavoritesViewPropertyName);

                if(value)
                {
                    if (!ActivityItems.Contains(ItemFavorites))
                    {
                        ActivityItems.Add(ItemFavorites);
                        activityItemsSortByGroupType();

                        doSearch();
                    }
                }
                else
                {
                    ActivityItems.Remove(ItemFavorites);
                }
            }
        }


        /// <summary>
        /// The <see cref="IsShowRecentView" /> property's name.
        /// </summary>
        public const string IsShowRecentViewPropertyName = "IsShowRecentView";

        private bool _isShowRecentViewProperty = true;

        /// <summary>
        /// Sets and gets the IsShowRecentView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowRecentView
        {
            get
            {
                return _isShowRecentViewProperty;
            }

            set
            {
                if (_isShowRecentViewProperty == value)
                {
                    return;
                }

                _isShowRecentViewProperty = value;
                RaisePropertyChanged(IsShowRecentViewPropertyName);

                if (value)
                {
                    if (!ActivityItems.Contains(ItemRecent))
                    {
                        ActivityItems.Add(ItemRecent);
                        activityItemsSortByGroupType();

                        doSearch();
                    }
                }
                else
                {
                    ActivityItems.Remove(ItemRecent);
                }
            }
        }



        /// <summary>
        /// The <see cref="IsShowAvailableView" /> property's name.
        /// </summary>
        public const string IsShowAvailableViewPropertyName = "IsShowAvailableView";

        private bool _isShowAvailableViewProperty = true;

        /// <summary>
        /// Sets and gets the IsShowAvailableView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowAvailableView
        {
            get
            {
                return _isShowAvailableViewProperty;
            }

            set
            {
                if (_isShowAvailableViewProperty == value)
                {
                    return;
                }

                _isShowAvailableViewProperty = value;
                RaisePropertyChanged(IsShowAvailableViewPropertyName);

                if (value)
                {
                    if (!ActivityItems.Contains(ItemAvailable))
                    {
                        ActivityItems.Add(ItemAvailable);
                        activityItemsSortByGroupType();

                        doSearch();
                    }
                }
                else
                {
                    ActivityItems.Remove(ItemAvailable);
                }
            }
        }



        /// <summary>
        /// Initializes a new instance of the ActivitiesViewModel class.
        /// </summary>
        public ActivitiesViewModel()
        {
            Messenger.Default.Register<ActivityTreeItem>(this, "AddToFavorites", AddToFavorites);
            Messenger.Default.Register<ActivityTreeItem>(this, "RemoveFromFavorites", RemoveFromFavorites);
            Messenger.Default.Register<ActivityTreeItem>(this, "AddToRecent", AddToRecent);
            

            initAvailableActivities();//必须先初始化，以便提前填充ActivityTreeItemTypeOfDic
            initFavoriteActivities();
            initRecentActivities();

            //排序
            activityItemsSortByGroupType();
        }

        private void activityItemsSortByGroupType()
        {
            ActivityItems.Sort((x, y) => x.GroupType.CompareTo(y.GroupType));
        }

        private void AddToRecent(ActivityTreeItem item)
        {
            if(!ItemRecent.Children.Contains(item))
            {
                ItemRecent.Children.Insert(0, item);
            }
        }

        private void RemoveFromFavorites(ActivityTreeItem item)
        {
            //从收藏夹移除
            item.IsFavorite = false;
            ItemFavorites.Children.Remove(item);

            updateFavoriteActivitiesConfig();
        }

        private void AddToFavorites(ActivityTreeItem item)
        {
            //添加到收藏夹
            item.IsFavorite = true;
            ItemFavorites.Children.Add(item);

            //收藏夹按名字排序
            ItemFavorites.Children.Sort((x, y) => x.Name.CompareTo(y.Name));

            updateFavoriteActivitiesConfig();
        }

        private void updateFavoriteActivitiesConfig()
        {
            //保存收藏夹内容到FavoriteActivities.xml
            var xmlPath = App.LocalRPAStudioDir + @"\Config\FavoriteActivities.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            var rootNode = doc.DocumentElement;
            rootNode.IsEmpty = true;

            foreach(var item in ItemFavorites.Children)
            {
                XmlElement activityElement = doc.CreateElement("Activity");
                activityElement.SetAttribute("TypeOf", item.TypeOf);
                rootNode.AppendChild(activityElement);
            }

            doc.Save(xmlPath);
        }

        public void MountActivityConfig(string activity_config_xml)
        {
            //将activity_config_xml挂载到当前的AvailableActivitiesXmlDocument中的相应位置
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(activity_config_xml);

            mergeXmlDocument(AvailableActivitiesXmlDocument.DocumentElement, doc.DocumentElement, AvailableActivitiesXmlDocument, doc);
            //var outputXml = AvailableActivitiesXmlDocument.OuterXml;

            Common.RunInUI(() =>{
                //重新初始化可用及收藏列表，最近列表保留旧的即可
                initAvailableActivities();
                initFavoriteActivities();
                //排序
                activityItemsSortByGroupType();
            });
            
        }

        private void mergeXmlDocument(XmlNode nodeCurrent, XmlNode nodeMount, XmlDocument docCurrent, XmlDocument docMount)
        {
            var groupNodesCurrent = nodeCurrent.SelectNodes("Group");
            var groupNodesMount = nodeMount.SelectNodes("Group");

            foreach (XmlNode groupNodeMount in groupNodesMount)
            {
                var nameMount = (groupNodeMount as XmlElement).GetAttribute("Name");

                bool isMountNameExist = false;
                XmlNode currentExistNode = null;

                foreach (XmlNode groupNodeCurrent in groupNodesCurrent)
                {
                    var nameCurrent = (groupNodeCurrent as XmlElement).GetAttribute("Name");

                    if (nameCurrent == nameMount)
                    {
                        isMountNameExist = true;
                        currentExistNode = groupNodeCurrent;
                        break;
                    }
                }

                if (isMountNameExist)
                {
                    //存在该组，继续深入下去
                    //组已经存在时，组上的属性值如果存在且不为空时，可覆盖旧值
                    var strName = (groupNodeMount as XmlElement).GetAttribute("Name");
                    var strIsExpanded = (groupNodeMount as XmlElement).GetAttribute("IsExpanded");
                    var strToolTip = (groupNodeMount as XmlElement).GetAttribute("ToolTip");
                    var strIsSortable = (groupNodeMount as XmlElement).GetAttribute("IsSortable");

                    if(!string.IsNullOrEmpty(strName))
                    {
                        (currentExistNode as XmlElement).SetAttribute("Name", strName);
                    }

                    if (!string.IsNullOrEmpty(strIsExpanded))
                    {
                        (currentExistNode as XmlElement).SetAttribute("IsExpanded", strIsExpanded);
                    }

                    if (!string.IsNullOrEmpty(strToolTip))
                    {
                        (currentExistNode as XmlElement).SetAttribute("ToolTip", strToolTip);
                    }

                    if (!string.IsNullOrEmpty(strIsSortable))
                    {
                        (currentExistNode as XmlElement).SetAttribute("IsSortable", strIsSortable);
                    }

                    mergeXmlDocument(currentExistNode, groupNodeMount, docCurrent, docMount);
                }
                else
                {
                    //不存在的组，直接附加上去
                    XmlNode tempNode = docCurrent.ImportNode(groupNodeMount, true);
                    nodeCurrent.AppendChild(tempNode);
                }
            }

            var activitiesNodesMount = nodeMount.SelectNodes("Activity");
            foreach (XmlNode activityNodeMount in activitiesNodesMount)
            {
                XmlNode tempNode = docCurrent.ImportNode(activityNodeMount, true);
                nodeCurrent.AppendChild(tempNode);
            }
        }


        private void initRecentActivities()
        {
            ItemRecent = new ActivityTreeItem();
            ItemRecent.GroupType = ActivityTreeItem.enGroupType.Recent;
            ActivityItemsRemove(ActivityTreeItem.enGroupType.Recent);
            ActivityItems.Add(ItemRecent);

            XmlDocument doc = new XmlDocument();
            doc.Load(App.LocalRPAStudioDir+ @"\Config\RecentActivities.xml");
            var rootNode = doc.DocumentElement;

            ItemRecent.Name = (rootNode as XmlElement).GetAttribute("Name");
            ItemRecent.IsExpanded = (rootNode as XmlElement).GetAttribute("IsExpanded").ToLower() == "true";
            ItemRecent.ToolTip = (rootNode as XmlElement).GetAttribute("ToolTip");
        }

        private void initFavoriteActivities()
        {
            ItemFavorites = new ActivityTreeItem();
            ItemFavorites.GroupType = ActivityTreeItem.enGroupType.Favorite;
            ActivityItemsRemove(ActivityTreeItem.enGroupType.Favorite);
            ActivityItems.Add(ItemFavorites);

            XmlDocument doc = new XmlDocument();
            doc.Load(App.LocalRPAStudioDir+ @"\Config\FavoriteActivities.xml");
            var rootNode = doc.DocumentElement;

            ItemFavorites.Name = (rootNode as XmlElement).GetAttribute("Name");
            ItemFavorites.IsExpanded = (rootNode as XmlElement).GetAttribute("IsExpanded").ToLower() == "true";
            ItemFavorites.ToolTip = (rootNode as XmlElement).GetAttribute("ToolTip");

            var activitiesNodes = rootNode.SelectNodes("Activity");
            foreach (XmlNode activityNode in activitiesNodes)
            {
                var TypeOf = (activityNode as XmlElement).GetAttribute("TypeOf");
                var item = GetActivityItemByTypeOf(TypeOf);
                if(item != null)
                {
                    item.IsFavorite = true;
                    ItemFavorites.Children.Add(item);
                }
                
            }
        }

        private ActivityTreeItem GetActivityItemByTypeOf(string typeOf)
        {
            if (typeOf != "" && ActivityTreeItemTypeOfDic.ContainsKey(typeOf))
            {
                ActivityTreeItem item = ActivityTreeItemTypeOfDic[typeOf];
                return item;
            }

            return null;
        }

        private void ActivityItemsRemove(ActivityTreeItem.enGroupType available)
        {
            foreach(var item in ActivityItems)
            {
                if(item.GroupType == available)
                {
                    ActivityItems.Remove(item);
                    break;
                }
            }
        }

        private void initAvailableActivities()
        {
            //初始化可用组件
            ItemAvailable = new ActivityTreeItem();
            ItemAvailable.GroupType = ActivityTreeItem.enGroupType.Available;
            ActivityItemsRemove(ActivityTreeItem.enGroupType.Available);
            ActivityItems.Add(ItemAvailable);

            if(AvailableActivitiesXmlDocument == null)
            {
                XmlDocument doc = new XmlDocument();

                using (var ms = new MemoryStream(RPAStudio.Properties.Resources.AvailableActivities))
                {
                    ms.Flush();
                    ms.Position = 0;
                    doc.Load(ms);
                    ms.Close();
                }

                AvailableActivitiesXmlDocument = doc;
            }

            var rootNode = AvailableActivitiesXmlDocument.DocumentElement;

            ItemAvailable.Name = (rootNode as XmlElement).GetAttribute("Name");
            ItemAvailable.IsExpanded = (rootNode as XmlElement).GetAttribute("IsExpanded").ToLower() == "true";
            ItemAvailable.ToolTip = (rootNode as XmlElement).GetAttribute("ToolTip");

            var strIsSortable = (rootNode as XmlElement).GetAttribute("IsSortable").ToLower();
            if (strIsSortable == "true")
            {
                ItemAvailable.IsSortable = true;
            }
            else if (strIsSortable == "false")
            {
                ItemAvailable.IsSortable = false;
            }
            else
            {
                ItemAvailable.IsSortable = null;
            }

            initAvailableGroup(rootNode, ItemAvailable);

        }

        private void initAvailableGroup(XmlNode node, ActivityTreeItem parent)
        {
            var groupNodes = node.SelectNodes("Group");
            foreach (XmlNode groupNode in groupNodes)
            {
                var item = new ActivityTreeItem();
                item.Name = (groupNode as XmlElement).GetAttribute("Name");
                item.IsExpanded = (groupNode as XmlElement).GetAttribute("IsExpanded").ToLower() == "true";
                item.ToolTip = (groupNode as XmlElement).GetAttribute("ToolTip");

                var strIsSortable = (groupNode as XmlElement).GetAttribute("IsSortable").ToLower();
                if(strIsSortable == "true")
                {
                    item.IsSortable = true;
                }else if(strIsSortable == "false")
                {
                    item.IsSortable = false;
                }
                else
                {
                    item.IsSortable = null;
                }

                item.Parent = parent;
                parent.Children.Add(item);

                if(activityTreeItemIsSortable(parent))
                {
                    activityItemSortChildrenByName(parent);
                }

                initAvailableGroup(groupNode, item);
            }

            var activitiesNodes = node.SelectNodes("Activity");
            foreach (XmlNode activityNode in activitiesNodes)
            {
                var item = new ActivityTreeItem();
                item.IsActivity = true;
                item.Name = (activityNode as XmlElement).GetAttribute("Name");
                item.TypeOf = (activityNode as XmlElement).GetAttribute("TypeOf");//放在Icon赋值前面，因为Icon要借用TypeOf的信息
                item.Icon = (activityNode as XmlElement).GetAttribute("Icon");
                item.IsFavorite = (activityNode as XmlElement).GetAttribute("IsFavorite").ToLower() == "true";
                item.IsSystem = (activityNode as XmlElement).GetAttribute("IsSystem").ToLower() == "true";
                item.ToolTip = (activityNode as XmlElement).GetAttribute("ToolTip");

                var strIsSortable = (activityNode as XmlElement).GetAttribute("IsSortable").ToLower();
                if (strIsSortable == "true")
                {
                    item.IsSortable = true;
                }
                else if (strIsSortable == "false")
                {
                    item.IsSortable = false;
                }
                else
                {
                    item.IsSortable = null;
                }

                item.Parent = parent;
                parent.Children.Add(item);

                if (activityTreeItemIsSortable(parent))
                {
                    activityItemSortChildrenByName(parent);
                }

                ActivityTreeItemTypeOfDic[item.TypeOf] = item;
                ActivityTreeItemAssemblyQualifiedNameDic[item.AssemblyQualifiedName] = item;
            }
        }

        private bool activityTreeItemIsSortable(ActivityTreeItem parent)
        {
            if(parent == null)
            {
                return false;
            }

            if(parent.IsSortable == null)
            {
                return activityTreeItemIsSortable(parent.Parent);
            }else
            {
                return parent.IsSortable.Value;
            }
        }

        private void activityItemSortChildrenByName(ActivityTreeItem parent)
        {
            parent.Children.Sort((x, y) => x.Name.CompareTo(y.Name));//子节点排序
        }

        private void ActivityTreeItemSetAllIsExpanded(ActivityTreeItem item, bool IsExpanded)
        {
            item.IsExpanded = IsExpanded;
            foreach (var child in item.Children)
            {
                ActivityTreeItemSetAllIsExpanded(child, IsExpanded);
            }
        }

        private void ActivityTreeItemSetAllIsSearching(ActivityTreeItem item, bool IsSearching)
        {
            item.IsSearching = IsSearching;
            foreach (var child in item.Children)
            {
                ActivityTreeItemSetAllIsSearching(child, IsSearching);
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

        private void ActivityTreeItemSetAllSearchText(ActivityTreeItem item, string SearchText)
        {
            item.SearchText = SearchText;
            item.HighlightText = SearchText;
            foreach (var child in item.Children)
            {
                ActivityTreeItemSetAllSearchText(child, SearchText);
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
                        foreach(var item in ActivityItems)
                        {
                            ActivityTreeItemSetAllIsExpanded(item,true);
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
                        foreach (var item in ActivityItems)
                        {
                            ActivityTreeItemSetAllIsExpanded(item,false);
                        }
                    }));
            }
        }


        private RelayCommand<object> _showActivitiesMenuCommand;

        /// <summary>
        /// Gets the ShowActivitiesMenuCommand.
        /// </summary>
        public RelayCommand<object> ShowActivitiesMenuCommand
        {
            get
            {
                return _showActivitiesMenuCommand
                    ?? (_showActivitiesMenuCommand = new RelayCommand<object>(
                    p =>
                    {
                        var view = App.Current.MainWindow;
                        var cm = view.FindResource("ActivitiesToolbarDropdownContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        cm.PlacementTarget = p as Button;
                        cm.Placement = PlacementMode.Bottom;
                        cm.IsOpen = true;
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
                foreach (var item in ActivityItems)
                {
                    ActivityTreeItemSetAllIsSearching(item, false);
                }

                foreach (var item in ActivityItems)
                {
                    ActivityTreeItemSetAllSearchText(item, "");
                }

                IsSearchResultEmpty = false;
            }
            else
            {
                //根据搜索内容显示

                foreach (var item in ActivityItems)
                {
                    ActivityTreeItemSetAllIsSearching(item, true);
                }

                //预先全部置为不匹配
                foreach (var item in ActivityItems)
                {
                    ActivityTreeItemSetAllIsMatch(item, false);
                }


                foreach (var item in ActivityItems)
                {
                    item.ApplyCriteria(searchContent, new Stack<ActivityTreeItem>());
                }

                IsSearchResultEmpty = true;
                foreach (var item in ActivityItems)
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

        
    }
}
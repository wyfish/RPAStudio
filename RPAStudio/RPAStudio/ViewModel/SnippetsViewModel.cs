using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Librarys;
using RPAStudio.Localization;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SnippetsViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the SnippetsViewModel class.
        /// </summary>
        public SnippetsViewModel()
        {
            initSnippets();

            Messenger.Default.Register<SnippetTreeItem>(this, "RemoveSnippet", RemoveSnippet);
        }

        private void RemoveSnippet(SnippetTreeItem obj)
        {
            initSnippets();
        }

        private void InitGroup(DirectoryInfo di, SnippetTreeItem parent = null)
        {
            //当前目录文件夹遍历
            DirectoryInfo[] dis = di.GetDirectories();
            for (int j = 0; j < dis.Length; j++)
            {
                var item = new SnippetTreeItem();
                item.Path = dis[j].FullName;
                item.Name = dis[j].Name;

                if (parent != null)
                {
                    parent.Children.Add(item);
                }
                else
                {
                    item.IsExpanded = true;//默认展开第一层
                    SnippetsItems.Add(item);
                }

                InitGroup(dis[j], item);
            }

            //当前目录文件遍历
            FileInfo[] fis = di.GetFiles();
            for (int i = 0; i < fis.Length; i++)
            {
                var item = new SnippetTreeItem();
                item.IsSnippet = true;
                item.Path = fis[i].FullName;
                item.Name = fis[i].Name;

                if(fis[i].Extension.ToLower() == ".xaml")
                {
                    if (parent != null)
                    {
                        parent.Children.Add(item);
                    }
                    else
                    {
                        SnippetsItems.Add(item);
                    }
                }

            }

        }

        private void initSnippets()
        {
            //从文件夹中初始化Snippets,并按文件名排序
            SnippetsItems.Clear();
            DirectoryInfo di = new DirectoryInfo(System.Environment.CurrentDirectory+@"\Snippets");
            InitGroup(di);

            InitUserSnippets();
        }

        private void InitUserSnippets()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\CodeSnippets.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var directoryNodes = rootNode.SelectNodes("Directory");
            foreach (XmlNode dir in directoryNodes)
            {
                var dirPath = (dir as XmlElement).GetAttribute("Path");
                var id = (dir as XmlElement).GetAttribute("Id");

                var userItem = new SnippetTreeItem();
                userItem.IsUserAdd = true;//标识为用户添加
                userItem.Id = id;
                userItem.Path = dirPath;
                userItem.Name = Path.GetFileName(dirPath);
                userItem.IsExpanded = true;

                SnippetsItems.Add(userItem);

                DirectoryInfo di = new DirectoryInfo(dirPath);
                InitGroup(di, userItem);
            }
        }


        /// <summary>
        /// The <see cref="SnippetsItems" /> property's name.
        /// </summary>
        public const string SnippetsItemsPropertyName = "SnippetsItems";

        private ObservableCollection<SnippetTreeItem> _snippetsItemsProperty = new ObservableCollection<SnippetTreeItem>();

        /// <summary>
        /// Sets and gets the SnippetsItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<SnippetTreeItem> SnippetsItems
        {
            get
            {
                return _snippetsItemsProperty;
            }

            set
            {
                if (_snippetsItemsProperty == value)
                {
                    return;
                }

                _snippetsItemsProperty = value;
                RaisePropertyChanged(SnippetsItemsPropertyName);
            }
        }

        private void SnippetTreeItemSetAllIsExpanded(SnippetTreeItem item, bool IsExpanded)
        {
            item.IsExpanded = IsExpanded;
            foreach (var child in item.Children)
            {
                SnippetTreeItemSetAllIsExpanded(child, IsExpanded);
            }
        }

        private void SnippetTreeItemSetAllIsSearching(SnippetTreeItem item, bool IsSearching)
        {
            item.IsSearching = IsSearching;
            foreach (var child in item.Children)
            {
                SnippetTreeItemSetAllIsSearching(child, IsSearching);
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

        private void SnippetTreeItemSetAllSearchText(SnippetTreeItem item, string SearchText)
        {
            item.SearchText = SearchText;
            foreach (var child in item.Children)
            {
                SnippetTreeItemSetAllSearchText(child, SearchText);
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
                        foreach (var item in SnippetsItems)
                        {
                            SnippetTreeItemSetAllIsExpanded(item, true);
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
                        foreach (var item in SnippetsItems)
                        {
                            SnippetTreeItemSetAllIsExpanded(item, false);
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
                        initSnippets();
                    }));
            }
        }


        private RelayCommand _addFolderCommand;

        /// <summary>
        /// Gets the AddFolderCommand.
        /// </summary>
        public RelayCommand AddFolderCommand
        {
            get
            {
                return _addFolderCommand
                    ?? (_addFolderCommand = new RelayCommand(
                    () =>
                    {
                        //让用户选择欲添加代码片断的文件夹
                        string dst_dir = "";
                        // 请选择一个目录添加到代码片断中
                        if (Common.ShowSelectDirDialog(ResxIF.GetString("SelectDirectoryToAddCodeSnippet"), ref dst_dir))
                        {
                            //添加目录到配置文件中
                            XmlDocument doc = new XmlDocument();
                            var path = App.LocalRPAStudioDir + @"\Config\CodeSnippets.xml";
                            doc.Load(path);
                            var rootNode = doc.DocumentElement;

                            XmlElement dirElement = doc.CreateElement("Directory");
                            dirElement.SetAttribute("Id", System.Guid.NewGuid().ToString());
                            dirElement.SetAttribute("Path", dst_dir);

                            rootNode.AppendChild(dirElement);

                            doc.Save(path);

                            initSnippets();
                        }

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
                foreach (var item in SnippetsItems)
                {
                    SnippetTreeItemSetAllIsSearching(item, false);
                }

                foreach (var item in SnippetsItems)
                {
                    SnippetTreeItemSetAllSearchText(item, "");
                }

                IsSearchResultEmpty = false;
            }
            else
            {
                //根据搜索内容显示

                foreach (var item in SnippetsItems)
                {
                    SnippetTreeItemSetAllIsSearching(item, true);
                }

                //预先全部置为不匹配
                foreach (var item in SnippetsItems)
                {
                    SnippetTreeItemSetAllIsMatch(item, false);
                }


                foreach (var item in SnippetsItems)
                {
                    item.ApplyCriteria(searchContent, new Stack<SnippetTreeItem>());
                }

                IsSearchResultEmpty = true;
                foreach (var item in SnippetsItems)
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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Plugins.Shared.Library;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class OutputViewModel : ViewModelBase
    {
        private ListBox m_listBox;
        private bool isListBoxFirstSizeChanged = false;

        private RelayCommand<RoutedEventArgs> _listBoxLoadedCommand;

        /// <summary>
        /// Gets the ListBoxLoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> ListBoxLoadedCommand
        {
            get
            {
                return _listBoxLoadedCommand
                    ?? (_listBoxLoadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        m_listBox = (ListBox)p.Source;
                    }));
            }
        }

        private RelayCommand _listBoxSizeChangedCommand;

        /// <summary>
        /// Gets the ListBoxSizeChangedCommand.
        /// </summary>
        public RelayCommand ListBoxSizeChangedCommand
        {
            get
            {
                return _listBoxSizeChangedCommand
                    ?? (_listBoxSizeChangedCommand = new RelayCommand(
                    () =>
                    {
                        if (m_listBox != null && !isListBoxFirstSizeChanged)
                        {
                            //由于输出窗口在未曾显示过时，列表的滚动条不会自动滚动，所以此处第一次要处理下以滚动到底部
                            isListBoxFirstSizeChanged = true;
                            var border = (Border)VisualTreeHelper.GetChild(m_listBox, 0);
                            var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                            scrollViewer.ScrollToBottom();
                        }
                    }));
            }
        }



        /// <summary>
        /// Initializes a new instance of the OutputViewModel class.
        /// </summary>
        public OutputViewModel()
        {

        }



        /// <summary>
        /// The <see cref="IsShowTimestamps" /> property's name.
        /// </summary>
        public const string IsShowTimestampsPropertyName = "IsShowTimestamps";

        private bool _isShowTimestampsProperty = false;

        /// <summary>
        /// Sets and gets the IsShowTimestamps property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowTimestamps
        {
            get
            {
                return _isShowTimestampsProperty;
            }

            set
            {
                if (_isShowTimestampsProperty == value)
                {
                    return;
                }

                _isShowTimestampsProperty = value;
                RaisePropertyChanged(IsShowTimestampsPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="IsShowError" /> property's name.
        /// </summary>
        public const string IsShowErrorPropertyName = "IsShowError";

        private bool _isShowErrorProperty = true;

        /// <summary>
        /// Sets and gets the IsShowError property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowError
        {
            get
            {
                return _isShowErrorProperty;
            }

            set
            {
                if (_isShowErrorProperty == value)
                {
                    return;
                }

                _isShowErrorProperty = value;
                RaisePropertyChanged(IsShowErrorPropertyName);

                ShowItemsFilter(value, (item) => { return item.IsError; });
            }
        }

       

        /// <summary>
        /// The <see cref="IsShowWarning" /> property's name.
        /// </summary>
        public const string IsShowWarningPropertyName = "IsShowWarning";

        private bool _isShowWarningProperty = true;

        /// <summary>
        /// Sets and gets the IsShowWarning property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowWarning
        {
            get
            {
                return _isShowWarningProperty;
            }

            set
            {
                if (_isShowWarningProperty == value)
                {
                    return;
                }

                _isShowWarningProperty = value;
                RaisePropertyChanged(IsShowWarningPropertyName);

                ShowItemsFilter(value, (item) => { return item.IsWarning; });
            }
        }

        /// <summary>
        /// The <see cref="IsShowInformation" /> property's name.
        /// </summary>
        public const string IsShowInformationPropertyName = "IsShowInformation";

        private bool _isShowInformationProperty = true;

        /// <summary>
        /// Sets and gets the IsShowInformation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowInformation
        {
            get
            {
                return _isShowInformationProperty;
            }

            set
            {
                if (_isShowInformationProperty == value)
                {
                    return;
                }

                _isShowInformationProperty = value;
                RaisePropertyChanged(IsShowInformationPropertyName);

                ShowItemsFilter(value, (item) => { return item.IsInformation; });
            }
        }

        /// <summary>
        /// The <see cref="IsShowTrace" /> property's name.
        /// </summary>
        public const string IsShowTracePropertyName = "IsShowTrace";

        private bool _isShowTraceProperty = true;

        /// <summary>
        /// Sets and gets the IsShowTrace property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsShowTrace
        {
            get
            {
                return _isShowTraceProperty;
            }

            set
            {
                if (_isShowTraceProperty == value)
                {
                    return;
                }

                _isShowTraceProperty = value;
                RaisePropertyChanged(IsShowTracePropertyName);

                ShowItemsFilter(value, (item) => { return item.IsTrace; });
            }
        }




        /// <summary>
        /// The <see cref="ErrorCount" /> property's name.
        /// </summary>
        public const string ErrorCountPropertyName = "ErrorCount";

        private int _errorCountProperty = 0;

        /// <summary>
        /// Sets and gets the ErrorCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return _errorCountProperty;
            }

            set
            {
                if (_errorCountProperty == value)
                {
                    return;
                }

                _errorCountProperty = value;
                RaisePropertyChanged(ErrorCountPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="WarningCount" /> property's name.
        /// </summary>
        public const string WarningCountPropertyName = "WarningCount";

        private int _warningCountProperty = 0;

        /// <summary>
        /// Sets and gets the WarningCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int WarningCount
        {
            get
            {
                return _warningCountProperty;
            }

            set
            {
                if (_warningCountProperty == value)
                {
                    return;
                }

                _warningCountProperty = value;
                RaisePropertyChanged(WarningCountPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="InformationCount" /> property's name.
        /// </summary>
        public const string InformationCountPropertyName = "InformationCount";

        private int _informationCountProperty = 0;

        /// <summary>
        /// Sets and gets the InformationCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int InformationCount
        {
            get
            {
                return _informationCountProperty;
            }

            set
            {
                if (_informationCountProperty == value)
                {
                    return;
                }

                _informationCountProperty = value;
                RaisePropertyChanged(InformationCountPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="TraceCount" /> property's name.
        /// </summary>
        public const string TraceCountPropertyName = "TraceCount";

        private int _traceCountProperty = 0;

        /// <summary>
        /// Sets and gets the TraceCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int TraceCount
        {
            get
            {
                return _traceCountProperty;
            }

            set
            {
                if (_traceCountProperty == value)
                {
                    return;
                }

                _traceCountProperty = value;
                RaisePropertyChanged(TraceCountPropertyName);
            }
        }















        private RelayCommand _showTimestampsCommand;

        /// <summary>
        /// Gets the ShowTimestampsCommand.
        /// </summary>
        public RelayCommand ShowTimestampsCommand
        {
            get
            {
                return _showTimestampsCommand
                    ?? (_showTimestampsCommand = new RelayCommand(
                    () =>
                    {
                        IsShowTimestamps = !IsShowTimestamps;

                        foreach(var item in OutputItems)
                        {
                            item.IsShowTimestamps = IsShowTimestamps;
                        }
                    }));
            }
        }




        private RelayCommand _showErrorCommand;

        /// <summary>
        /// Gets the ShowErrorCommand.
        /// </summary>
        public RelayCommand ShowErrorCommand
        {
            get
            {
                return _showErrorCommand
                    ?? (_showErrorCommand = new RelayCommand(
                    () =>
                    {
                        IsShowError = !IsShowError;
                    }));
            }
        }



        private RelayCommand _showWarningCommand;

        /// <summary>
        /// Gets the ShowWarningCommand.
        /// </summary>
        public RelayCommand ShowWarningCommand
        {
            get
            {
                return _showWarningCommand
                    ?? (_showWarningCommand = new RelayCommand(
                    () =>
                    {
                        IsShowWarning = !IsShowWarning;
                    }));
            }
        }

        private RelayCommand _showInformationCommand;

        /// <summary>
        /// Gets the ShowInformationCommand.
        /// </summary>
        public RelayCommand ShowInformationCommand
        {
            get
            {
                return _showInformationCommand
                    ?? (_showInformationCommand = new RelayCommand(
                    () =>
                    {
                        IsShowInformation = !IsShowInformation;
                    }));
            }
        }


        private RelayCommand _showTraceCommand;

        /// <summary>
        /// Gets the ShowTraceCommand.
        /// </summary>
        public RelayCommand ShowTraceCommand
        {
            get
            {
                return _showTraceCommand
                    ?? (_showTraceCommand = new RelayCommand(
                    () =>
                    {
                        IsShowTrace = !IsShowTrace;
                    }));
            }
        }



        private RelayCommand _clearAllCommand;

        /// <summary>
        /// Gets the ClearAllCommand.
        /// </summary>
        public RelayCommand ClearAllCommand
        {
            get
            {
                return _clearAllCommand
                    ?? (_clearAllCommand = new RelayCommand(
                    () =>
                    {
                        OutputItems.Clear();

                        ErrorCount = WarningCount = InformationCount = TraceCount = 0;
                    }));
            }
        }








        /// <summary>
        /// The <see cref="OutputItems" /> property's name.
        /// </summary>
        public const string OutputItemsPropertyName = "OutputItems";

        private ObservableCollection<OutputListItem> _outputItemsProperty = new ObservableCollection<OutputListItem>();

        /// <summary>
        /// Sets and gets the OutputItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<OutputListItem> OutputItems
        {
            get
            {
                return _outputItemsProperty;
            }

            set
            {
                if (_outputItemsProperty == value)
                {
                    return;
                }

                _outputItemsProperty = value;
                RaisePropertyChanged(OutputItemsPropertyName);
            }
        }


        private RelayCommand _copyItemMsgCommand;

        /// <summary>
        /// Gets the CopyItemMsgCommand.
        /// </summary>
        public RelayCommand CopyItemMsgCommand
        {
            get
            {
                return _copyItemMsgCommand
                    ?? (_copyItemMsgCommand = new RelayCommand(
                    () =>
                    {
                        //获取选中的条目并执行拷贝命令
                        foreach(var item in OutputItems)
                        {
                            if(item.IsSelected)
                            {
                                item.CopyItemMsgCommand.Execute(null);
                                break;
                            }
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
                foreach (var item in OutputItems)
                {
                    item.IsSearching = false;
                }

                foreach (var item in OutputItems)
                {
                    item.SearchText = searchContent;
                }

                IsSearchResultEmpty = false;

                //搜索结果选中项在清除搜索结果时自动滚动到选中项，以方便使用
                m_listBox.ScrollIntoView(m_listBox.SelectedItem);
            }
            else
            {
                //根据搜索内容显示

                foreach (var item in OutputItems)
                {
                    item.IsSearching = true;
                }

                //预先全部置为不匹配
                foreach (var item in OutputItems)
                {
                    item.IsMatch = false;
                }


                foreach (var item in OutputItems)
                {
                    item.ApplyCriteria(searchContent);
                }

                IsSearchResultEmpty = true;
                foreach (var item in OutputItems)
                {
                    if (item.IsMatch)
                    {
                        IsSearchResultEmpty = false;
                        break;
                    }
                }

            }
        }

        public void Log(SharedObject.enOutputType type, string msg, string msgDetails)
        {
            //如果消息详情为空，则默认详情复用消息的内容
            if (string.IsNullOrEmpty(msgDetails))
            {
                msgDetails = msg;
            }


            var item = new OutputListItem();

            switch(type)
            {
                case SharedObject.enOutputType.Error:
                    item.IsError = true;
                    ErrorCount++;
                    item.TextForeground = "#e91530";
                    break;
                case SharedObject.enOutputType.Information:
                    item.IsInformation = true;
                    InformationCount++;
                    item.TextForeground = "Black";
                    break;
                case SharedObject.enOutputType.Warning:
                    item.IsWarning = true;
                    WarningCount++;
                    item.TextForeground = "#f59f36";
                    break;
                case SharedObject.enOutputType.Trace:
                    item.IsTrace = true;
                    TraceCount++;
                    item.TextForeground = "#696969";
                    break;
                default:
                    break;
            }

            item.IsShowTimestamps = IsShowTimestamps;
            item.Timestamps = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            item.Msg = msg;
            item.MsgDetails = msgDetails;
            OutputItems.Add(item);

            doSearch();
        }


        private void ShowItemsFilter(bool isVisible, Func<OutputListItem, bool> compare)
        {
            foreach (var item in OutputItems)
            {
                if (compare(item))
                {
                    item.IsVisible = isVisible;
                }
            }
        }


    }
}
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RPAStudio.Librarys;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using RPAStudio.Windows;
using System.Text.RegularExpressions;

namespace RPAStudio.ViewModel
{
    public class OutputListItem : ViewModelBase
    {
        /// <summary>
        /// The <see cref="IsVisible" /> property's name.
        /// </summary>
        public const string IsVisiblePropertyName = "IsVisible";

        private bool _isVisibleProperty = true;

        /// <summary>
        /// Sets and gets the IsVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _isVisibleProperty;
            }

            set
            {
                if (_isVisibleProperty == value)
                {
                    return;
                }

                _isVisibleProperty = value;
                RaisePropertyChanged(IsVisiblePropertyName);
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

                updateToolTip();
            }
        }

        private void updateToolTip()
        {
            if (IsShowTimestamps)
            {
                ToolTip = string.Format("日志时间：{0}\n日志内容：{1}", Timestamps, Msg);
            }
            else
            {
                ToolTip = string.Format("日志内容：{0}", Msg);
            }
        }



        /// <summary>
        /// The <see cref="Timestamps" /> property's name.
        /// </summary>
        public const string TimestampsPropertyName = "Timestamps";

        private string _timestampsProperty = "";

        /// <summary>
        /// Sets and gets the Timestamps property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Timestamps
        {
            get
            {
                return _timestampsProperty;
            }

            set
            {
                if (_timestampsProperty == value)
                {
                    return;
                }

                _timestampsProperty = value;
                RaisePropertyChanged(TimestampsPropertyName);
            }
        }





        /// <summary>
        /// The <see cref="IsError" /> property's name.
        /// </summary>
        public const string IsErrorPropertyName = "IsError";

        private bool _isErrorProperty = false;

        /// <summary>
        /// Sets and gets the IsError property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsError
        {
            get
            {
                return _isErrorProperty;
            }

            set
            {
                if (_isErrorProperty == value)
                {
                    return;
                }

                _isErrorProperty = value;
                RaisePropertyChanged(IsErrorPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsWarning" /> property's name.
        /// </summary>
        public const string IsWarningPropertyName = "IsWarning";

        private bool _isWarningProperty = false;

        /// <summary>
        /// Sets and gets the IsWarning property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWarning
        {
            get
            {
                return _isWarningProperty;
            }

            set
            {
                if (_isWarningProperty == value)
                {
                    return;
                }

                _isWarningProperty = value;
                RaisePropertyChanged(IsWarningPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsInformation" /> property's name.
        /// </summary>
        public const string IsInformationPropertyName = "IsInformation";

        private bool _isInformationProperty = false;

        /// <summary>
        /// Sets and gets the IsInformation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsInformation
        {
            get
            {
                return _isInformationProperty;
            }

            set
            {
                if (_isInformationProperty == value)
                {
                    return;
                }

                _isInformationProperty = value;
                RaisePropertyChanged(IsInformationPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsTrace" /> property's name.
        /// </summary>
        public const string IsTracePropertyName = "IsTrace";

        private bool _isTraceProperty = false;

        /// <summary>
        /// Sets and gets the IsTrace property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsTrace
        {
            get
            {
                return _isTraceProperty;
            }

            set
            {
                if (_isTraceProperty == value)
                {
                    return;
                }

                _isTraceProperty = value;
                RaisePropertyChanged(IsTracePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="TextForeground" /> property's name.
        /// </summary>
        public const string TextForegroundPropertyName = "TextForeground";

        private string _textForegroundProperty = "Black";

        /// <summary>
        /// Sets and gets the TextForeground property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TextForeground
        {
            get
            {
                return _textForegroundProperty;
            }

            set
            {
                if (_textForegroundProperty == value)
                {
                    return;
                }

                _textForegroundProperty = value;
                RaisePropertyChanged(TextForegroundPropertyName);
            }
        }






        /// <summary>
        /// The <see cref="Msg" /> property's name.
        /// </summary>
        public const string MsgPropertyName = "Msg";

        private string _msgProperty = "";

        /// <summary>
        /// Sets and gets the Msg property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Msg
        {
            get
            {
                return _msgProperty;
            }

            set
            {
                if (_msgProperty == value)
                {
                    return;
                }

                _msgProperty = value;
                RaisePropertyChanged(MsgPropertyName);

                updateToolTip();


            }
        }


        /// <summary>
        /// The <see cref="MsgDetails" /> property's name.
        /// </summary>
        public const string MsgDetailsPropertyName = "MsgDetails";

        private string _msgDetailsProperty = "";

        /// <summary>
        /// Sets and gets the MsgDetails property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string MsgDetails
        {
            get
            {
                return _msgDetailsProperty;
            }

            set
            {
                if (_msgDetailsProperty == value)
                {
                    return;
                }

                _msgDetailsProperty = value;
                RaisePropertyChanged(MsgDetailsPropertyName);
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


        public void ApplyCriteria(string criteria)
        {
            SearchText = criteria;

            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
               
            }
        }

        private static string wildCardToRegular(string value)
        {
            return ".*" + Regex.Escape(value).Replace("\\ ", ".*") + ".*";
        }

        private bool IsCriteriaMatched(string criteria)
        {
            return string.IsNullOrEmpty(criteria) || Regex.IsMatch(Msg, wildCardToRegular(criteria), RegexOptions.IgnoreCase);
        }




        private RelayCommand _mouseRightButtonUpCommand;

        /// <summary>
        /// Gets the MouseRightButtonUpCommand.
        /// </summary>
        public RelayCommand MouseRightButtonUpCommand
        {
            get
            {
                return _mouseRightButtonUpCommand
                    ?? (_mouseRightButtonUpCommand = new RelayCommand(
                    () =>
                    {
                        var view = App.Current.MainWindow;
                        var cm = view.FindResource("OutputItemContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        cm.Placement = PlacementMode.MousePoint;
                        cm.IsOpen = true;
                    }));
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
                        Clipboard.SetDataObject(Msg);
                    }));
            }
        }



        private RelayCommand _mouseDoubleClickCommand;

        /// <summary>
        /// Gets the MouseDoubleClickCommand.
        /// </summary>
        public RelayCommand MouseDoubleClickCommand
        {
            get
            {
                return _mouseDoubleClickCommand
                    ?? (_mouseDoubleClickCommand = new RelayCommand(
                    () =>
                    {
                        ViewItemMsgDetailCommand.Execute(null);
                    }));
            }
        }


        private RelayCommand _viewItemMsgDetailCommand;

        /// <summary>
        /// Gets the ViewItemMsgDetailCommand.
        /// </summary>
        public RelayCommand ViewItemMsgDetailCommand
        {
            get
            {
                return _viewItemMsgDetailCommand
                    ?? (_viewItemMsgDetailCommand = new RelayCommand(
                    () =>
                    {
                        //弹出详细信息窗口
                        var window = new MessageDetailsWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as MessageDetailsViewModel;
                        vm.Title = "消息详情";
                        vm.MsgDetails = MsgDetails;
                        window.ShowDialog();
                    }));
            }
        }


       







    }
}
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Activities.Tracking;
using System.Collections.ObjectModel;
using System;
using RPAStudio.Librarys;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using RPAStudio.UserControls;
using System.Linq;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LocalsViewModel : ViewModelBase
    {
        private LocalsContent m_localsContentUserControl;
        /// <summary>
        /// Initializes a new instance of the LocalsViewModel class.
        /// </summary>
        public LocalsViewModel()
        {
            Messenger.Default.Register<ActivityStateRecord>(this, "ShowLocals", ShowLocals);
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
                        m_localsContentUserControl = (LocalsContent)p.Source;
                        MonitorListViewColumnWidthChanged();
                    }));
            }
        }

        

        private RelayCommand _sizeChangedCommand;

        /// <summary>
        /// Gets the SizeChangedCommand.
        /// </summary>
        public RelayCommand SizeChangedCommand
        {
            get
            {
                return _sizeChangedCommand
                    ?? (_sizeChangedCommand = new RelayCommand(
                    () =>
                    {
                        AdjustListViewColumnWidthByRatio();
                    }));
            }
        }

        /// <summary>
        /// 列表头拖拽改变宽度时,调用AdjustListViewColumnWidth()改变列宽
        /// </summary>
        private void MonitorListViewColumnWidthChanged()
        {
            foreach (var column in m_localsContentUserControl.gridView.Columns)
            {
                ((System.ComponentModel.INotifyPropertyChanged)column).PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "ActualWidth")
                    {
                        AdjustListViewColumnWidth();
                    }
                };
            }
        }


        /// <summary>
        /// 窗口宽度变化时，两个列按所占的比例进行空间分配
        /// </summary>
        private void AdjustListViewColumnWidthByRatio()
        {
            var col0 = m_localsContentUserControl.gridView.Columns[0];
            var col1 = m_localsContentUserControl.gridView.Columns[1];

            var ratio = col0.ActualWidth / col1.ActualWidth;
            var col0_new_width = m_localsContentUserControl.ActualWidth * col0.ActualWidth / (col0.ActualWidth + col1.ActualWidth);
            var col1_new_width = m_localsContentUserControl.ActualWidth * col1.ActualWidth / (col0.ActualWidth + col1.ActualWidth);

            col0.Width = col0_new_width;
            col1.Width = col1_new_width;
        }

        /// <summary>
        /// 调整列宽（目前限制为两列），最后一列要填满剩余空间，并且每列最小宽度要设置
        /// </summary>
        private void AdjustListViewColumnWidth()
        {
            double min_width = 30.0f;
            var col0 = m_localsContentUserControl.gridView.Columns[0];
            var col1 = m_localsContentUserControl.gridView.Columns[1];

            if(col0.ActualWidth < min_width)
            {
                col0.Width = min_width;
                col1.Width = m_localsContentUserControl.ActualWidth - col0.Width;
            }

            if (col1.ActualWidth < min_width)
            {
                col1.Width = min_width;
                col0.Width = m_localsContentUserControl.ActualWidth - col1.Width;
            }
            
            var reamining_width = m_localsContentUserControl.ActualWidth - col0.Width;
            if(reamining_width < min_width)
            {
                col1.Width = min_width;
                col0.Width = m_localsContentUserControl.ActualWidth - col1.Width;
            }
            else
            {
                col1.Width = m_localsContentUserControl.ActualWidth - col0.Width; ;
            }

        }

        private void ShowLocals(ActivityStateRecord obj)
        {
            var vars = obj.Variables;
            var args = obj.Arguments;

            Common.RunInUI(() =>
            {
                LocalsList.Clear();
                foreach (var item in vars)
                {
                    var localsItem = new LocalsItem();
                    localsItem.Name = item.Key;
                    localsItem.Value = translateValue(item.Value);

                    LocalsList.Add(localsItem);
                }

                foreach (var item in args)
                {
                    var localsItem = new LocalsItem();
                    localsItem.Name = item.Key;
                    localsItem.Value = translateValue(item.Value);

                    LocalsList.Add(localsItem);
                }
            });

        }

        private string translateValue(object value)
        {
            if(value == null)
            {
                return "null";
            }

            if(value is string)
            {
                return string.Format("\"{0}\"",value.ToString());
            }

            if(value is Array)
            {
                var arr = value as Array;

                //int[4] { 1, 2, 3, 5 }
                //int[4]
                //{
                //  1,
                //  2,
                //  3,
                //  5
                //}

                var children_str = "";
                int index = 0;
                foreach (var element in arr)
                {
                    children_str += "  "+ translateValue(element);
                    
                    if (index < arr.Length - 1)
                    {
                        children_str += ",";
                    }
                    else
                    {
                        children_str += " ";
                    }

                    children_str += Environment.NewLine;
                    index++;
                }

                var typeName = Common.GetFriendlyTypeName(arr.GetType().GetElementType());
                //string.format转义{时用{{
                var format_output = string.Format("{0}[{1}]"+Environment.NewLine+"{{" + Environment.NewLine+"{2}}}"+ Environment.NewLine, typeName, arr.Length.ToString(), children_str);

                foreach (var element in arr)
                    Console.WriteLine(element.ToString());

                return format_output;
            }

            return value.ToString();
        }


        /// <summary>
        /// The <see cref="LocalsList" /> property's name.
        /// </summary>
        public const string LocalsListPropertyName = "LocalsList";

        private ObservableCollection<LocalsItem> _localsListProperty = new ObservableCollection<LocalsItem>();

        /// <summary>
        /// Sets and gets the LocalsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<LocalsItem> LocalsList
        {
            get
            {
                return _localsListProperty;
            }

            set
            {
                if (_localsListProperty == value)
                {
                    return;
                }

                _localsListProperty = value;
                RaisePropertyChanged(LocalsListPropertyName);
            }
        }




















    }
}
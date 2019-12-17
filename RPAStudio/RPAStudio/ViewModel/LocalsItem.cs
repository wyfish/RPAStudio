using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RPAStudio.Windows;
using System.Windows;

namespace RPAStudio.ViewModel
{
    public class LocalsItem : ViewModelBase
    {
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
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        private string _valueProperty = "";

        /// <summary>
        /// Sets and gets the Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Value
        {
            get
            {
                return _valueProperty;
            }

            set
            {
                if (_valueProperty == value)
                {
                    return;
                }

                _valueProperty = value;
                RaisePropertyChanged(ValuePropertyName);
            }
        }



        private RelayCommand _viewValueDetailCommand;

        /// <summary>
        /// Gets the ViewValueDetailCommand.
        /// </summary>
        public RelayCommand ViewValueDetailCommand
        {
            get
            {
                return _viewValueDetailCommand
                    ?? (_viewValueDetailCommand = new RelayCommand(
                    () =>
                    {
                        //查看值详情
                        var window = new MessageDetailsWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as MessageDetailsViewModel;
                        vm.Title = "属性值";
                        vm.MsgDetails = Value;
                        window.ShowDialog();
                    }));
            }
        }



       






    }
}
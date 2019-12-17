using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MessageDetailsViewModel : ViewModelBase
    {
        private Window m_view;

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
                        m_view = (Window)p.Source;
                    }));
            }
        }


        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _titleProperty = "";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get
            {
                return _titleProperty;
            }

            set
            {
                if (_titleProperty == value)
                {
                    return;
                }

                _titleProperty = value;
                RaisePropertyChanged(TitlePropertyName);
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


        private RelayCommand _copyCommand;

        /// <summary>
        /// Gets the CopyCommand.
        /// </summary>
        public RelayCommand CopyCommand
        {
            get
            {
                return _copyCommand
                    ?? (_copyCommand = new RelayCommand(
                    () =>
                    {
                        Clipboard.SetDataObject(MsgDetails);
                    }));
            }
        }









        /// <summary>
        /// Initializes a new instance of the MessageDetailsViewModel class.
        /// </summary>
        public MessageDetailsViewModel()
        {
        }

    }
}
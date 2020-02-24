using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SetUpViewModel : ViewModelBase
    {
        // 对应的视图
        public UserControl m_view { get; set; }
        private RelayCommand<RoutedEventArgs> _loadedCommand;
        // 窗体加载完成后触发
        public RelayCommand<RoutedEventArgs> LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        m_view = (UserControl)p.Source;
                    }));
            }
        }

        /// <summary>
        /// Initializes a new instance of the SetUpViewModel class.
        /// </summary>
        public SetUpViewModel()
        {
        }
    }
}
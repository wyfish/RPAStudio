/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RPARobot"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance;
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            Instance = this;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<StartupViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<UserPreferencesViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
        }

        public StartupViewModel Startup
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartupViewModel>();
            }
        }


        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }


        public UserPreferencesViewModel UserPreferences
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UserPreferencesViewModel>();
            }
        }

        public RegisterViewModel Register
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RegisterViewModel>();
            }
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
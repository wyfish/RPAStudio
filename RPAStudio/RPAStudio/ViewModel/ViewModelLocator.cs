/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RPAStudio"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace RPAStudio.ViewModel
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
            SimpleIoc.Default.Register<SplashScreenViewModel>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ActivitiesViewModel>();
            SimpleIoc.Default.Register<DockViewModel>();
            SimpleIoc.Default.Register<StartViewModel>();
            SimpleIoc.Default.Register<SnippetsViewModel>();
            SimpleIoc.Default.Register<NewProjectViewModel>();
            SimpleIoc.Default.Register<ProjectViewModel>();
            SimpleIoc.Default.Register<ProjectSettingsViewModel>();
            SimpleIoc.Default.Register<NewFolderViewModel>();
            SimpleIoc.Default.Register<RenameViewModel>();
            SimpleIoc.Default.Register<NewXamlFileViewModel>();
            SimpleIoc.Default.Register<OutputViewModel>();
            SimpleIoc.Default.Register<MessageDetailsViewModel>();
            SimpleIoc.Default.Register<LocalsViewModel>();
            SimpleIoc.Default.Register<CheckUpgradeViewModel>();
            SimpleIoc.Default.Register<PublishProjectViewModel>();
            SimpleIoc.Default.Register<PackageManagerViewModel>();
            SimpleIoc.Default.Register<RecordingViewModel>();
            SimpleIoc.Default.Register<SourceCodeViewModel>(); 
        }

        public SplashScreenViewModel SplashScreen
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SplashScreenViewModel>();
            }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ActivitiesViewModel Activities
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ActivitiesViewModel>();
            }
        }

        public SnippetsViewModel Snippets
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SnippetsViewModel>();
            }
        }



        public DockViewModel Dock
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DockViewModel>();
            }
        }

        public StartViewModel Start
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartViewModel>();
            }
        }

        public ProjectViewModel Project
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProjectViewModel>();
            }
        }

        public OutputViewModel Output
        {
            get
            {
                return ServiceLocator.Current.GetInstance<OutputViewModel>();
            }
        }

        public CheckUpgradeViewModel CheckUpgrade
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CheckUpgradeViewModel>();
            }
        }


        public SourceCodeViewModel SourceCode
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SourceCodeViewModel>();
            }
        }

        //注意，这些窗口非长久存在，每次打开后要重建DataContext，所以要传唯一KEY System.Guid.NewGuid().ToString(){{{{{
        public NewProjectViewModel NewProject
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewProjectViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public ProjectSettingsViewModel ProjectSettings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProjectSettingsViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public NewFolderViewModel NewFolder
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewFolderViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public RenameViewModel Rename
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RenameViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public NewXamlFileViewModel NewXamlFile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewXamlFileViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public MessageDetailsViewModel MessageDetails
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MessageDetailsViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public LocalsViewModel Locals
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LocalsViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public PublishProjectViewModel PublishProject
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PublishProjectViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public PackageManagerViewModel PackageManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PackageManagerViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public RecordingViewModel Recording
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RecordingViewModel>(System.Guid.NewGuid().ToString());
            }
        }


        ///}}}}}}}}}}



        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
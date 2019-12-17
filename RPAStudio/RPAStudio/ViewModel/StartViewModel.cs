using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;
using System.Xml;
using RPAStudio.Windows;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using RPAStudio.Librarys;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class StartViewModel : ViewModelBase
    {
        public UserControl m_view { get; set; }

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
                        m_view = (UserControl)p.Source;
                    }));
            }
        }


        /// <summary>
        /// Initializes a new instance of the StartViewModel class.
        /// </summary>
        public StartViewModel()
        {
            var item = new RecentProjectItem();

            InitRecentProjects();

            Messenger.Default.Register<MessengerObjects.RecentProjectsModify> (this, (obj)=> {
                Common.RunInUI(()=> {
                    InitRecentProjects();
                });
            });


            Messenger.Default.Register<ProjectSettingsViewModel>(this, "ProjectSettingsModify", (obj) => {
                Common.RunInUI(() =>
                {
                    ProjectSettingsModify(obj);
                });
            });
        }

        private void ProjectSettingsModify(ProjectSettingsViewModel obj)
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RecentProjects.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var projectNodes = rootNode.SelectNodes("Project");
            foreach (XmlElement dir in projectNodes)
            {
                var filePath = dir.GetAttribute("FilePath");
                var name = dir.GetAttribute("Name");
                var description = dir.GetAttribute("Description");

                if (obj.CurrentProjectJsonFile == filePath)
                {
                    if (obj.ProjectName != name || obj.ProjectDescription != description)
                    {
                        dir.SetAttribute("Name", obj.ProjectName);
                        dir.SetAttribute("Description", obj.ProjectDescription);

                        doc.Save(path);

                        InitRecentProjects();
                        break;
                    }
                }
            }
        }

        private void InitRecentProjects()
        {
            RecentProjects.Clear();

            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RecentProjects.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var maxShowCount = Convert.ToInt32(rootNode.GetAttribute("MaxShowCount"));
            var projectNodes = rootNode.SelectNodes("Project");
            foreach (XmlNode dir in projectNodes)
            {
                var filePath = (dir as XmlElement).GetAttribute("FilePath");
                var name = (dir as XmlElement).GetAttribute("Name");
                var description = (dir as XmlElement).GetAttribute("Description");

                var item = new RecentProjectItem();
                item.ProjectFilePath = filePath;
                item.ProjectName = name;
                item.ProjectDescription = description;

                RecentProjects.Add(item);

                maxShowCount--;
                if(maxShowCount == 0)
                {
                    break;
                }
            }
        }


        /// <summary>
        /// The <see cref="RecentProjects" /> property's name.
        /// </summary>
        public const string RecentProjectsPropertyName = "RecentProjects";

        private ObservableCollection<RecentProjectItem> _recentProjectsProperty = new ObservableCollection<RecentProjectItem>();

        /// <summary>
        /// Sets and gets the RecentProjects property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<RecentProjectItem> RecentProjects
        {
            get
            {
                return _recentProjectsProperty;
            }

            set
            {
                if (_recentProjectsProperty == value)
                {
                    return;
                }

                _recentProjectsProperty = value;
                RaisePropertyChanged(RecentProjectsPropertyName);
            }
        }




        private RelayCommand _newProcessCommand;

        /// <summary>
        /// Gets the NewProcessCommand.
        /// </summary>
        public RelayCommand NewProcessCommand
        {
            get
            {
                return _newProcessCommand
                    ?? (_newProcessCommand = new RelayCommand(
                    () =>
                    {
                        //弹出新建项目对话框
                        var window = new NewProjectWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as NewProjectViewModel;
                        vm.ProjectType = NewProjectViewModel.enProjectType.Process;
                        window.ShowDialog();
                    }));
            }
        }







    }
}
using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Librarys;
using RPAStudio.Localization;

namespace RPAStudio.ViewModel
{
    public class RecentProjectItem : ViewModelBase
    {
        /// <summary>
        /// The <see cref="ProjectFilePath" /> property's name.
        /// </summary>
        public const string ProjectFilePathPropertyName = "ProjectFilePath";

        private string _projectFilePathProperty = "";

        /// <summary>
        /// Sets and gets the ProjectFilePath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectFilePath
        {
            get
            {
                return _projectFilePathProperty;
            }

            set
            {
                if (_projectFilePathProperty == value)
                {
                    return;
                }

                _projectFilePathProperty = value;
                RaisePropertyChanged(ProjectFilePathPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="ProjectName" /> property's name.
        /// </summary>
        public const string ProjectNamePropertyName = "ProjectName";

        private string _projectNameProperty = "";

        /// <summary>
        /// Sets and gets the ProjectName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectName
        {
            get
            {
                return _projectNameProperty;
            }

            set
            {
                if (_projectNameProperty == value)
                {
                    return;
                }

                _projectNameProperty = value;
                RaisePropertyChanged(ProjectNamePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="ProjectDescription" /> property's name.
        /// </summary>
        public const string ProjectDescriptionPropertyName = "ProjectDescription";

        private string _projectDescriptionProperty = "";

        /// <summary>
        /// Sets and gets the ProjectDescription property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectDescription
        {
            get
            {
                return _projectDescriptionProperty;
            }

            set
            {
                if (_projectDescriptionProperty == value)
                {
                    return;
                }

                _projectDescriptionProperty = value;
                RaisePropertyChanged(ProjectDescriptionPropertyName);
            }
        }




        private RelayCommand _openProjectCommand;

        /// <summary>
        /// Gets the OpenProjectCommand.
        /// </summary>
        public RelayCommand OpenProjectCommand
        {
            get
            {
                return _openProjectCommand
                    ?? (_openProjectCommand = new RelayCommand(
                    () =>
                    {
                        //判断文件是否还存在
                        if(System.IO.File.Exists(ProjectFilePath))
                        {
                            //关闭已经打开的项目(如果当前打开的和已经打开的是同一个项目，则不要执行关闭项目命令)
                            if (!string.IsNullOrEmpty(ViewModelLocator.Instance.Project.CurrentProjectJsonFile)
                                && ProjectFilePath != ViewModelLocator.Instance.Project.CurrentProjectJsonFile)
                            {
                                if (!ViewModelLocator.Instance.Main.DoCloseProject())
                                {
                                    return;//用户取消了关闭，直接返回
                                }
                            }

                           
                            //显示新打开的项目
                            var msg = new MessengerObjects.ProjectOpen();
                            msg.ProjectJsonFile = ProjectFilePath;
                            msg.Sender = this;
                            Messenger.Default.Send(msg);
                        }
                        else
                        {
                            //不存在则提示用户移除条目
                            // 无法打开项目配置文件\"{0}\"，是否从最近项目列表中移除该条目？
                            var ret = MessageBox.Show(App.Current.MainWindow, string.Format(ResxIF.GetString("MB_CannotOpenProjectConfigFile"), ProjectFilePath), ResxIF.GetString("ConfirmText"), MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                            if (ret == MessageBoxResult.OK)
                            {
                                removeMyself();

                                //广播RecentProjects.xml改变的消息，以重刷最近项目列表
                                Messenger.Default.Send(new MessengerObjects.RecentProjectsModify());
                            }
                        }

                        
                    }));
            }
        }



        private RelayCommand _showProjectMenuCommand;

        /// <summary>
        /// Gets the ShowProjectMenuCommand.
        /// </summary>
        public RelayCommand ShowProjectMenuCommand
        {
            get
            {
                return _showProjectMenuCommand
                    ?? (_showProjectMenuCommand = new RelayCommand(
                    () =>
                    {
                        var view = App.Current.MainWindow;
                        var cm = view.FindResource("RecentProjectItemContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        cm.Placement = PlacementMode.MousePoint;
                        cm.IsOpen = true;
                    }));
            }
        }




        private RelayCommand _openProjectDirCommand;

        /// <summary>
        /// Gets the OpenProjectDirCommand.
        /// </summary>
        public RelayCommand OpenProjectDirCommand
        {
            get
            {
                return _openProjectDirCommand
                    ?? (_openProjectDirCommand = new RelayCommand(
                    () =>
                    {
                        Common.LocateDirInExplorer(System.IO.Path.GetDirectoryName(ProjectFilePath));
                    }));
            }
        }


        private RelayCommand _removeFromListCommand;

        /// <summary>
        /// Gets the RemoveFromListCommand.
        /// </summary>
        public RelayCommand RemoveFromListCommand
        {
            get
            {
                return _removeFromListCommand
                    ?? (_removeFromListCommand = new RelayCommand(
                    () =>
                    {
                        removeMyself();

                        //广播RecentProjects.xml改变的消息，以重刷最近项目列表
                        Messenger.Default.Send(new MessengerObjects.RecentProjectsModify());
                    }));
            }
        }






        private void removeMyself()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RecentProjects.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var projectNodes = rootNode.SelectNodes("Project");

            foreach (XmlElement item in projectNodes)
            {
                var filePath = item.GetAttribute("FilePath");
                if (ProjectFilePath == filePath)
                {
                    rootNode.RemoveChild(item);
                    break;
                }
            }

            doc.Save(path);
        }

        public void RecentProjectsReorder()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RecentProjects.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var projectNodes = rootNode.SelectNodes("Project");

            string filePath="", name="", description="";
            foreach (XmlElement item in projectNodes)
            {
                filePath = item.GetAttribute("FilePath");
                if (ProjectFilePath == filePath)
                {
                    name = item.GetAttribute("Name");
                    description = item.GetAttribute("Description");

                    rootNode.RemoveChild(item);
                    break;
                }
            }

            XmlElement projectElement = doc.CreateElement("Project");
            projectElement.SetAttribute("FilePath", filePath);
            projectElement.SetAttribute("Name", name);
            projectElement.SetAttribute("Description", description);

            rootNode.PrependChild(projectElement);

            doc.Save(path);

            //广播RecentProjects.xml改变的消息，以重刷最近项目列表
            Messenger.Default.Send(new MessengerObjects.RecentProjectsModify());
        }
    }
}
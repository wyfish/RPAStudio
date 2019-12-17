using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using RPAStudio.Librarys;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Xml;
using NuGet;
using System.IO;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PublishProjectViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Window m_view;

        private string m_nupkgLocation;//nupkg包的保存位置

        private string m_robotPackagesLocation;

        /// <summary>
        /// Initializes a new instance of the PublishProjectViewModel class.
        /// </summary>
        public PublishProjectViewModel()
        {
            
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
                        m_view = (Window)p.Source;

                        Init();
                    }));
            }
        }

        private void Init()
        {
            //初始化相关信息
            initRobotDefaults();
            initPublishHistory();
            
            initVersionInfo();
        }

        private void initPublishHistory()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPAStudio.settings";
            doc.Load(path);
            var rootNode = doc.DocumentElement;
            var publishHistoryElement = rootNode.SelectSingleNode("PublishHistory") as XmlElement;
            var lastPublishUriElement = publishHistoryElement.SelectSingleNode("LastPublishUri") as XmlElement;
            CustomLocation = lastPublishUriElement.InnerText;

            var workflowUriHistoryElement = publishHistoryElement.SelectSingleNode("WorkflowUriHistory") as XmlElement;
            var publishUriEntryList = workflowUriHistoryElement.SelectNodes("PublishUriEntry");
            foreach(XmlElement item in publishUriEntryList)
            {
                var publishUriElement = item.SelectSingleNode("PublishUri") as XmlElement;
                CustomLocations.Add(publishUriElement.InnerText);
            }

            if(string.IsNullOrEmpty(CustomLocation))
            {
                CustomLocation = m_robotPackagesLocation;
            }
        }

        private void initRobotDefaults()
        {
            var commonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            var packagesDir = commonApplicationData + @"\RPAStudio\Packages";//机器人默认读取nupkg包的位置
            if(!System.IO.Directory.Exists(packagesDir))
            {
                System.IO.Directory.CreateDirectory(packagesDir);
            }

            m_robotPackagesLocation = packagesDir;
        }

        private void initVersionInfo()
        {
            string json = System.IO.File.ReadAllText(ViewModelLocator.Instance.Project.CurrentProjectJsonFile);
            var jsonCfg = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectJsonConfig>(json);
            CurrentProjectVersion = jsonCfg.projectVersion;

            Version currentVersion = new Version(CurrentProjectVersion);
            Version newVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build + 1);
            NewProjectVersion = newVersion.ToString();
        }



        /// <summary>
        /// The <see cref="IsPublishToRobotDefault" /> property's name.
        /// </summary>
        public const string IsPublishToRobotDefaultPropertyName = "IsPublishToRobotDefault";

        private bool _isPublishToRobotDefaultProperty = true;

        /// <summary>
        /// Sets and gets the IsPublishToRobotDefault property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsPublishToRobotDefault
        {
            get
            {
                return _isPublishToRobotDefaultProperty;
            }

            set
            {
                if (_isPublishToRobotDefaultProperty == value)
                {
                    return;
                }

                _isPublishToRobotDefaultProperty = value;
                RaisePropertyChanged(IsPublishToRobotDefaultPropertyName);

                OkCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// The <see cref="IsCustomLocationCorrect" /> property's name.
        /// </summary>
        public const string IsCustomLocationCorrectPropertyName = "IsCustomLocationCorrect";

        private bool _isCustomLocationCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsCustomLocationCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsCustomLocationCorrect
        {
            get
            {
                return _isCustomLocationCorrectProperty;
            }

            set
            {
                if (_isCustomLocationCorrectProperty == value)
                {
                    return;
                }

                _isCustomLocationCorrectProperty = value;
                RaisePropertyChanged(IsCustomLocationCorrectPropertyName);

                if(value)
                {
                    CustomLocationPadding = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    CustomLocationPadding = new Thickness(2, 2, 22, 2);
                }

                OkCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The <see cref="CustomLocationPadding" /> property's name.
        /// </summary>
        public const string CustomLocationPaddingPropertyName = "CustomLocationPadding";

        private Thickness _customLocationPaddingProperty = new Thickness(2,2,2,2);

        /// <summary>
        /// Sets and gets the CustomLocationPadding property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Thickness CustomLocationPadding
        {
            get
            {
                return _customLocationPaddingProperty;
            }

            set
            {
                if (_customLocationPaddingProperty == value)
                {
                    return;
                }

                _customLocationPaddingProperty = value;
                RaisePropertyChanged(CustomLocationPaddingPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="CustomLocations" /> property's name.
        /// </summary>
        public const string CustomLocationsPropertyName = "CustomLocations";

        private ObservableCollection<string> _customLocationsProperty = new ObservableCollection<string>();

        /// <summary>
        /// Sets and gets the CustomLocations property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<string> CustomLocations
        {
            get
            {
                return _customLocationsProperty;
            }

            set
            {
                if (_customLocationsProperty == value)
                {
                    return;
                }

                _customLocationsProperty = value;
                RaisePropertyChanged(CustomLocationsPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="CustomLocation" /> property's name.
        /// </summary>
        public const string CustomLocationPropertyName = "CustomLocation";

        private string _customLocationProperty = "";

        /// <summary>
        /// Sets and gets the CustomLocation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CustomLocation
        {
            get
            {
                return _customLocationProperty;
            }

            set
            {
                if (_customLocationProperty == value)
                {
                    return;
                }

                _customLocationProperty = value;
                RaisePropertyChanged(CustomLocationPropertyName);

                IsCustomLocationCorrect = System.IO.Directory.Exists(value);
                if(!IsCustomLocationCorrect)
                {
                    CustomLocationValidatedWrongTip = "指定的路径不存在";
                }
            }
        }


        /// <summary>
        /// The <see cref="CustomLocationValidatedWrongTip" /> property's name.
        /// </summary>
        public const string CustomLocationValidatedWrongTipPropertyName = "CustomLocationValidatedWrongTip";

        private string _customLocationValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the CustomLocationValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CustomLocationValidatedWrongTip
        {
            get
            {
                return _customLocationValidatedWrongTipProperty;
            }

            set
            {
                if (_customLocationValidatedWrongTipProperty == value)
                {
                    return;
                }

                _customLocationValidatedWrongTipProperty = value;
                RaisePropertyChanged(CustomLocationValidatedWrongTipPropertyName);
            }
        }





        /// <summary>
        /// The <see cref="ReleaseNotes" /> property's name.
        /// </summary>
        public const string ReleaseNotesPropertyName = "ReleaseNotes";

        private string _releaseNotesProperty = "";

        /// <summary>
        /// Sets and gets the ReleaseNotes property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ReleaseNotes
        {
            get
            {
                return _releaseNotesProperty;
            }

            set
            {
                if (_releaseNotesProperty == value)
                {
                    return;
                }

                _releaseNotesProperty = value;
                RaisePropertyChanged(ReleaseNotesPropertyName);
            }
        }







        private RelayCommand _browserFolderCommand;

        /// <summary>
        /// Gets the BrowserFolderCommand.
        /// </summary>
        public RelayCommand BrowserFolderCommand
        {
            get
            {
                return _browserFolderCommand
                    ?? (_browserFolderCommand = new RelayCommand(
                    () =>
                    {
                        string dst_dir = "";
                        if (Common.ShowSelectDirDialog("请选择一个位置来发布项目", ref dst_dir))
                        {
                            CustomLocation = dst_dir;
                        }
                    }));
            }
        }



        /// <summary>
        /// The <see cref="CurrentProjectVersion" /> property's name.
        /// </summary>
        public const string CurrentProjectVersionPropertyName = "CurrentProjectVersion";

        private string _currentProjectVersionProperty = "";

        /// <summary>
        /// Sets and gets the CurrentProjectVersion property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentProjectVersion
        {
            get
            {
                return _currentProjectVersionProperty;
            }

            set
            {
                if (_currentProjectVersionProperty == value)
                {
                    return;
                }

                _currentProjectVersionProperty = value;
                RaisePropertyChanged(CurrentProjectVersionPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="NewProjectVersion" /> property's name.
        /// </summary>
        public const string NewProjectVersionPropertyName = "NewProjectVersion";

        private string _newProjectVersionProperty = "";

        /// <summary>
        /// Sets and gets the NewProjectVersion property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NewProjectVersion
        {
            get
            {
                return _newProjectVersionProperty;
            }

            set
            {
                if (_newProjectVersionProperty == value)
                {
                    return;
                }

                _newProjectVersionProperty = value;
                RaisePropertyChanged(NewProjectVersionPropertyName);

                IsNewProjectVersionCorrect = !string.IsNullOrWhiteSpace(value);
                if(!IsNewProjectVersionCorrect)
                {
                    NewProjectVersionValidatedWrongTip = "版本号不能为空";
                    return;
                }

                try
                {
                    var ver = new Version(value);
                    if(ver.Major >=0 && ver.Minor >=0 && ver.Build >=0 && ver .Revision <0)
                    {
                        IsNewProjectVersionCorrect = true;
                    }
                    else
                    {
                        IsNewProjectVersionCorrect = false;
                        NewProjectVersionValidatedWrongTip = "版本号须为a.b.c形式";
                    }
                }
                catch (Exception)
                {
                    IsNewProjectVersionCorrect = false;
                    NewProjectVersionValidatedWrongTip = "版本号非法";
                }
                
            }
        }


        /// <summary>
        /// The <see cref="NewProjectVersionValidatedWrongTip" /> property's name.
        /// </summary>
        public const string NewProjectVersionValidatedWrongTipPropertyName = "NewProjectVersionValidatedWrongTip";

        private string _newProjectVersionValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the NewProjectVersionValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NewProjectVersionValidatedWrongTip
        {
            get
            {
                return _newProjectVersionValidatedWrongTipProperty;
            }

            set
            {
                if (_newProjectVersionValidatedWrongTipProperty == value)
                {
                    return;
                }

                _newProjectVersionValidatedWrongTipProperty = value;
                RaisePropertyChanged(NewProjectVersionValidatedWrongTipPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="NewProjectVersionPadding" /> property's name.
        /// </summary>
        public const string NewProjectVersionPaddingPropertyName = "NewProjectVersionPadding";

        private Thickness _newProjectVersionPaddingProperty = new Thickness();

        /// <summary>
        /// Sets and gets the NewProjectVersionPadding property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Thickness NewProjectVersionPadding
        {
            get
            {
                return _newProjectVersionPaddingProperty;
            }

            set
            {
                if (_newProjectVersionPaddingProperty == value)
                {
                    return;
                }

                _newProjectVersionPaddingProperty = value;
                RaisePropertyChanged(NewProjectVersionPaddingPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsNewProjectVersionCorrect" /> property's name.
        /// </summary>
        public const string IsNewProjectVersionCorrectPropertyName = "IsNewProjectVersionCorrect";

        private bool _isNewProjectVersionCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsNewProjectVersionCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsNewProjectVersionCorrect
        {
            get
            {
                return _isNewProjectVersionCorrectProperty;
            }

            set
            {
                if (_isNewProjectVersionCorrectProperty == value)
                {
                    return;
                }

                _isNewProjectVersionCorrectProperty = value;
                RaisePropertyChanged(IsNewProjectVersionCorrectPropertyName);

                if(value)
                {
                    NewProjectVersionPadding = new Thickness();
                }
                else
                {
                    NewProjectVersionPadding = new Thickness(0,0,20,0);
                }

                OkCommand.RaiseCanExecuteChanged();

            }
        }








        private RelayCommand _okCommand;

        /// <summary>
        /// Gets the OkCommand.
        /// </summary>
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand(
                    () =>
                    {
                        if (IsPublishToRobotDefault)
                        {
                            m_nupkgLocation = m_robotPackagesLocation;
                        }
                        else
                        {
                            m_nupkgLocation = CustomLocation;
                        }


                        m_view.Hide();

                        try
                        {
                            var publishAuthors = Environment.UserName;
                            var publishVersion = NewProjectVersion;
                            var projectPath = ViewModelLocator.Instance.Project.ProjectPath;

                            ManifestMetadata metadata = new ManifestMetadata()
                            {
                                Authors = publishAuthors,
                                Owners = publishAuthors,
                                Version = publishVersion,
                                Id = ViewModelLocator.Instance.Project.ProjectName,//项目名称
                                Description = ViewModelLocator.Instance.Project.ProjectDescription,//项目描述
                                ReleaseNotes = ReleaseNotes,
                            };

                            PackageBuilder builder = new PackageBuilder();
                            builder.PopulateFiles(projectPath, new[] { new ManifestFile() { Source = @"**", Target = @"lib/net452" } });
                            builder.Populate(metadata);

                            var outputPath = System.IO.Path.Combine(m_nupkgLocation, ViewModelLocator.Instance.Project.ProjectName) + "." + publishVersion + ".nupkg";
                            using (FileStream stream = File.Open(outputPath, FileMode.OpenOrCreate))
                            {
                                builder.Save(stream);
                            }

                            //更新当前项目的projectVersion为新版本号
                            updateProjectVersion();

                            //xml更新发布历史（发布到本地机器人时无须更新历史记录）
                            if (!IsPublishToRobotDefault)
                            {
                                updateSettings(CustomLocation);
                            }

                            //弹窗生成成功
                            if (System.IO.File.Exists(outputPath))
                            {
                                var info = string.Format("项目发布成功。\n名称：{0}\n版本：{1}\n位置：{2}\n", ViewModelLocator.Instance.Project.ProjectName, publishVersion, m_nupkgLocation);
                                MessageBox.Show(App.Current.MainWindow, info, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        catch (Exception err)
                        {
                            Logger.Debug(err, logger);
                            MessageBox.Show(App.Current.MainWindow, "发布项目失败！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        m_view.Close();
                    },
                    () => (IsPublishToRobotDefault || IsCustomLocationCorrect) && IsNewProjectVersionCorrect));
            }
        }

        private void updateSettings(string publishUri)
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPAStudio.settings";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var publishHistoryElement = rootNode.SelectSingleNode("PublishHistory") as XmlElement;
            var lastPublishUriElement = publishHistoryElement.SelectSingleNode("LastPublishUri") as XmlElement;
            lastPublishUriElement.InnerText = publishUri;

            bool bFound = false;
            foreach(var item in CustomLocations)
            {
                if(item.ContainsIgnoreCase(publishUri))
                {
                    bFound = true;
                    break;
                }
            }

            if(!bFound)
            {
                CustomLocations.Insert(0, publishUri);
            }


            var workflowUriHistoryElement = publishHistoryElement.SelectSingleNode("WorkflowUriHistory") as XmlElement;
            workflowUriHistoryElement.RemoveAll();

            int count = 0;
            foreach(var item in CustomLocations)
            {
                XmlElement publishUriEntryElement = doc.CreateElement("PublishUriEntry");
                workflowUriHistoryElement.AppendChild(publishUriEntryElement);

                XmlElement publishUriElement = doc.CreateElement("PublishUri");
                publishUriElement.InnerText = item;
                publishUriEntryElement.AppendChild(publishUriElement);

                if(++count >= 10)
                {
                    //限制历史记录最多N条
                    break;
                }
            }

            doc.Save(path);
        }

        private void updateProjectVersion()
        {
            //json更新
            string json = File.ReadAllText(ViewModelLocator.Instance.Project.CurrentProjectJsonFile);
            var jsonCfg = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectJsonConfig>(json);
            jsonCfg.projectVersion = NewProjectVersion;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonCfg, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(ViewModelLocator.Instance.Project.CurrentProjectJsonFile, output);
        }


        private RelayCommand _cancelCommand;

        /// <summary>
        /// Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand(
                    () =>
                    {
                        m_view.Close();
                    }));
            }
        }





        



    }
}
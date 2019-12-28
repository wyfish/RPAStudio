using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using RPAStudio.Librarys;
using log4net;
using System.Windows;
using Newtonsoft.Json;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using NuGet;
using NuGet.Versioning;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NewProjectViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

        private RelayCommand<RoutedEventArgs> _projectNameLoadedCommand;

        /// <summary>
        /// Gets the ProjectNameLoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> ProjectNameLoadedCommand
        {
            get
            {
                return _projectNameLoadedCommand
                    ?? (_projectNameLoadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        var textBox = (TextBox)p.Source;
                        textBox.Focus();
                        textBox.SelectAll();
                    }));
            }
        }



        public enum enProjectType
        {
            Null = 0,
            Process,
        }

        /// <summary>
        /// The <see cref="ProjectType" /> property's name.
        /// </summary>
        public const string ProjectTypePropertyName = "ProjectType";

        private enProjectType _projectTypeProperty = enProjectType.Null;

        /// <summary>
        /// Sets and gets the ProjectType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public enProjectType ProjectType
        {
            get
            {
                return _projectTypeProperty;
            }

            set
            {
                if (_projectTypeProperty == value)
                {
                    return;
                }

                _projectTypeProperty = value;
                RaisePropertyChanged(ProjectTypePropertyName);

                initInfoByProjectType(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the NewProjectViewModel class.
        /// </summary>
        public NewProjectViewModel()
        {
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
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _descriptionProperty = "";

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Description
        {
            get
            {
                return _descriptionProperty;
            }

            set
            {
                if (_descriptionProperty == value)
                {
                    return;
                }

                _descriptionProperty = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsProjectNameCorrect" /> property's name.
        /// </summary>
        public const string IsProjectNameCorrectPropertyName = "IsProjectNameCorrect";

        private bool _isProjectNameCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsProjectNameCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsProjectNameCorrect
        {
            get
            {
                return _isProjectNameCorrectProperty;
            }

            set
            {
                if (_isProjectNameCorrectProperty == value)
                {
                    return;
                }

                _isProjectNameCorrectProperty = value;
                RaisePropertyChanged(IsProjectNameCorrectPropertyName);
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

                projectNameValidate(value);
            }
        }

     
        /// <summary>
        /// The <see cref="ProjectNameValidatedWrongTip" /> property's name.
        /// </summary>
        public const string ProjectNameValidatedWrongTipPropertyName = "ProjectNameValidatedWrongTip";

        private string _projectNameValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the ProjectNameValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectNameValidatedWrongTip
        {
            get
            {
                return _projectNameValidatedWrongTipProperty;
            }

            set
            {
                if (_projectNameValidatedWrongTipProperty == value)
                {
                    return;
                }

                _projectNameValidatedWrongTipProperty = value;
                RaisePropertyChanged(ProjectNameValidatedWrongTipPropertyName);               
            }
        }

        private void projectNameValidate(string value)
        {
            IsProjectNameCorrect = true;
            if(string.IsNullOrEmpty(value))
            {
                IsProjectNameCorrect = false;
                ProjectNameValidatedWrongTip = "名称不能为空";
            }
            else
            {
                if (value.Contains(@"\") || value.Contains(@"/"))
                {
                    IsProjectNameCorrect = false;
                    ProjectNameValidatedWrongTip = "名称不能有非法字符";
                }
                else
                {
                    System.IO.FileInfo fi = null;
                    try
                    {
                        fi = new System.IO.FileInfo(value);
                    }
                    catch (ArgumentException) { }
                    catch (System.IO.PathTooLongException) { }
                    catch (NotSupportedException) { }
                    if (ReferenceEquals(fi, null))
                    {
                        // file name is not valid
                        IsProjectNameCorrect = false;
                        ProjectNameValidatedWrongTip = "名称不能有非法字符";
                    }
                    else
                    {
                        // file name is valid... May check for existence by calling fi.Exists.
                    }
                }
            }

            if(Directory.Exists(ProjectPath+@"\"+ProjectName))
            {
                IsProjectNameCorrect = false;
                ProjectNameValidatedWrongTip = "已经存在同名称的项目";
            }

            CreateProjectCommand.RaiseCanExecuteChanged();
        }




        /// <summary>
        /// The <see cref="IsProjectPathCorrect" /> property's name.
        /// </summary>
        public const string IsProjectPathCorrectPropertyName = "IsProjectPathCorrect";

        private bool _isProjectPathCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsProjectPathCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsProjectPathCorrect
        {
            get
            {
                return _isProjectPathCorrectProperty;
            }

            set
            {
                if (_isProjectPathCorrectProperty == value)
                {
                    return;
                }

                _isProjectPathCorrectProperty = value;
                RaisePropertyChanged(IsProjectPathCorrectPropertyName);
            }
        }



        /// <summary>
        /// 此路径为项目创建时所在的目录
        /// </summary>
        public const string ProjectPathPropertyName = "ProjectPath";

        private string _projectPathProperty = "";

        /// <summary>
        /// Sets and gets the ProjectPath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectPath
        {
            get
            {
                return _projectPathProperty;
            }

            set
            {
                if (_projectPathProperty == value)
                {
                    return;
                }

                _projectPathProperty = value;
                RaisePropertyChanged(ProjectPathPropertyName);

                projectPathValidate(value);
                projectNameValidate(ProjectName);//路径改变了同样要检查名称
            }
        }

        private void projectPathValidate(string value)
        {
            IsProjectPathCorrect = true;
            if (string.IsNullOrEmpty(value))
            {
                IsProjectPathCorrect = false;
                ProjectPathValidatedWrongTip = "位置不能为空";
            }
            else
            {
                if (!Directory.Exists(value))
                {
                    IsProjectPathCorrect = false;
                    ProjectPathValidatedWrongTip = "指定的位置不存在";
                }
            }

            CreateProjectCommand.RaiseCanExecuteChanged();
        }



        /// <summary>
        /// The <see cref="ProjectPathValidatedWrongTip" /> property's name.
        /// </summary>
        public const string ProjectPathValidatedWrongTipPropertyName = "ProjectPathValidatedWrongTip";

        private string _projectPathValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the ProjectPathValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectPathValidatedWrongTip
        {
            get
            {
                return _projectPathValidatedWrongTipProperty;
            }

            set
            {
                if (_projectPathValidatedWrongTipProperty == value)
                {
                    return;
                }

                _projectPathValidatedWrongTipProperty = value;
                RaisePropertyChanged(ProjectPathValidatedWrongTipPropertyName);
            }
        }



        private RelayCommand _selectProjectPathCommand;

        /// <summary>
        /// Gets the SelectProjectPathCommand.
        /// </summary>
        public RelayCommand SelectProjectPathCommand
        {
            get
            {
                return _selectProjectPathCommand
                    ?? (_selectProjectPathCommand = new RelayCommand(
                    () =>
                    {
                        string dst_dir = "";
                        if (Common.ShowSelectDirDialog("请选择一个位置来新建项目", ref dst_dir))
                        {
                            ProjectPath = dst_dir;
                        }
                    }));
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


        private RelayCommand _createProjectCommand;

        /// <summary>
        /// Gets the CreateProjectCommand.
        /// </summary>
        public RelayCommand CreateProjectCommand
        {
            get
            {
                return _createProjectCommand
                    ?? (_createProjectCommand = new RelayCommand(
                    () =>
                    {
                        if (!ViewModelLocator.Instance.Main.DoCloseProject())
                        {
                            return;//用户取消了关闭，直接返回
                        }

                        //开始创建项目
                        //1.首先保存创建项目的位置到ProjectUserConfig.xml中去
                        SaveDefaultProjectPath(ProjectPath);

                        //2.创建项目目录
                        try
                        {
                            Directory.CreateDirectory(ProjectPath + @"\" + ProjectName);
                        }
                        catch (Exception e)
                        {
                            //创建项目失败
                            Logger.Error(e, logger);

                            MessageBox.Show(App.Current.MainWindow, "创建项目目录失败，请检查", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        //3.创建项目配置文件project.json及Main.xaml
                        initProjectJson();
                        initMainXaml();

                        //4.保存到最近项目列表中
                        addToRecentProjects();

                        //5.切换到项目DOCKER中，并自动打开Main.xaml
                        var msg = new MessengerObjects.ProjectOpen();
                        msg.ProjectJsonFile = ProjectPath + @"\" + ProjectName + @"\project.json";
                        Messenger.Default.Send(msg);

                        m_view.Close();
                    },
                    () => IsProjectNameCorrect && IsProjectPathCorrect));
            }
        }

        private void addToRecentProjects()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RecentProjects.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var projectNodes = rootNode.SelectNodes("Project");

            //最多记录100条，默认显示个数由XML的MaxShowCount限制
            if (projectNodes.Count > 100)
            {
                rootNode.RemoveChild(rootNode.LastChild);
            }

            XmlElement projectElement = doc.CreateElement("Project");
            projectElement.SetAttribute("FilePath", ProjectPath + @"\" + ProjectName + @"\project.json");
            projectElement.SetAttribute("Name", ProjectName);
            projectElement.SetAttribute("Description", ProjectDescription);

            rootNode.PrependChild(projectElement);

            doc.Save(path);

            //广播RecentProjects.xml改变的消息，以重刷最近项目列表
            Messenger.Default.Send(new MessengerObjects.RecentProjectsModify());
        }

        private void initMainXaml()
        {
            byte[] data = Properties.Resources.Main;
            FileStream fileStream = new FileStream(ProjectPath+@"\"+ProjectName + @"\Main.xaml", FileMode.CreateNew);
            fileStream.Write(data, 0, (int)(data.Length));
            fileStream.Close();
        }

        private void initProjectJson()
        {
            var config = new ProjectJsonConfig();
            config.Init();
            config.studioVersion = Common.GetProgramVersion();
            config.name = ProjectName;
            config.description = ProjectDescription;
            config.main = "Main.xaml";

            //Packages目录下的nupkg包遍历，然后保存到依赖包对象里

            var repo = PackageRepositoryFactory.Default.CreateRepository(System.Environment.CurrentDirectory + @"\Packages");
            var nupkgs = repo.GetPackages();
            foreach(var item in nupkgs)
            {
                if(Regex.IsMatch(item.Id, "RPA.*.Activities", RegexOptions.IgnoreCase))
                {
                    var ver = item.Version.ToString();
                    config.dependencies[item.Id] = $"[{ver}]";
                }
            }

            if (ProjectType == enProjectType.Process)
            {
                config.projectType = "Workflow";
            }

            string json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);

            using (FileStream fs = new FileStream(ProjectPath + @"\" + ProjectName + @"\project.json", FileMode.Create))
            {
                //写入 
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                }
            }

        }

        private void SaveDefaultProjectPath(string projectPath)
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\ProjectUserConfig.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            rootNode.SetAttribute("DefaultCreatePath",ProjectPath);

            doc.Save(path);
        }

        private void initInfoByProjectType(enProjectType value)
        {
            if (value == enProjectType.Process)
            {
                initProjectUserConfig();
                initProjectConfig();
            }
        }

        private void initProjectConfig()
        {
            XmlDocument doc = new XmlDocument();
            var resourceXML = RPAStudio.Properties.ResourceLocalizer.GetLocalizedResource("ProjectConfig");
            using (var ms = new MemoryStream(resourceXML))
            {
                ms.Flush();
                ms.Position = 0;
                doc.Load(ms);
                ms.Close();
            }

            var rootNode = doc.DocumentElement;
            var processElement = rootNode.SelectSingleNode("Process") as XmlElement;
            Title = processElement.GetAttribute("Title");
            Description = processElement.GetAttribute("Description");
            var projectName = processElement.GetAttribute("ProjectName");

            ProjectName = Common.GetValidDirectoryName(ProjectPath, projectName, "{0}", 1);

            ProjectDescription = processElement.GetAttribute("ProjectDescription");
        }

        private void initProjectUserConfig()
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\ProjectUserConfig.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var defaultCreatePath = rootNode.GetAttribute("DefaultCreatePath");
            if (string.IsNullOrEmpty(defaultCreatePath) || !Directory.Exists(defaultCreatePath))
            {
                defaultCreatePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RPAStudio\Projects";

                if (!Directory.Exists(defaultCreatePath))
                {
                    Directory.CreateDirectory(defaultCreatePath);
                }
            }

            ProjectPath = defaultCreatePath;

        }


    }






    public class ProjectJsonConfig
    {
        private static readonly string initial_schema_version = "2.0.0";//新建项目时的的project.json文件版本
        private static readonly string initial_project_version = "2.0.0";//新建项目时的的项目版本，发布项目时会显示出来

        [JsonProperty(Order = 1)]
        public string schemaVersion { get; set; }

        [JsonProperty(Required = Required.Always,Order =2)]
        public string studioVersion { get; set; } 

        [JsonProperty(Required = Required.Always, Order = 3)]
        public string projectType { get; set; }

        [JsonProperty(Order = 4)]
        public string projectVersion { get; set; }

        [JsonProperty(Required = Required.Always, Order = 5)]
        public string name { get; set; }

        [JsonProperty(Order = 6)]
        public string description { get; set; }

        [JsonProperty(Required = Required.Always, Order = 7)]
        public string main { get; set; }

        [JsonProperty(Order = 8)]
        public JObject dependencies = new JObject();
        
        public bool Upgrade()
        {
            var schemaVersionTmp = schemaVersion;

            Upgrade(ref schemaVersionTmp, initial_schema_version);

            if(schemaVersion == schemaVersionTmp)
            {
                return false;
            }
            else
            {
                schemaVersion = schemaVersionTmp;
                return true;
            }
        }

        private void Upgrade(ref string schemaVersion, string newSchemaVersion)
        {
            if (schemaVersion == newSchemaVersion)
            {
                return;
            }

            Version currentVersion = new Version(schemaVersion);
            Version latestVersion = new Version(newSchemaVersion);

            if (currentVersion < new Version("2.0.0.0"))
            {
                //提示用户不再支持老版本项目，后期考虑再自动升级项目并备份老项目
                //TODO WJF 旧项目如何升级
                var err = "当前程序不再支持V2.0版本以下的旧版本项目！";
                MessageBox.Show(err);
                throw new Exception(err);
            }

            Upgrade(ref schemaVersion, initial_schema_version);
        }

        public void Init()
        {
            schemaVersion = initial_schema_version;
            projectVersion = initial_project_version;
        }
    }







}
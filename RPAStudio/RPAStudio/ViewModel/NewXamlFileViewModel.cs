using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Xml;
using RPAStudio.Librarys;
using System.IO;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NewXamlFileViewModel : ViewModelBase
    {
        private Window m_view;

        public string ProjectPath { get; internal set; }

        public enum enFileType
        {
            Null = 0,
            Sequence,
            Flowchart,
            StateMachine,
            GlobalHandler,
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
                    }));
            }
        }


        private RelayCommand<RoutedEventArgs> _fileNameLoadedCommand;

        /// <summary>
        /// Gets the FileNameLoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> FileNameLoadedCommand
        {
            get
            {
                return _fileNameLoadedCommand
                    ?? (_fileNameLoadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        var textBox = (TextBox)p.Source;
                        textBox.Focus();
                        textBox.SelectAll();
                    }));
            }
        }



        /// <summary>
        /// The <see cref="FileType" /> property's name.
        /// </summary>
        public const string FileTypePropertyName = "FileType";

        private enFileType _fileTypeProperty = enFileType.Null;

        /// <summary>
        /// Sets and gets the FileType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public enFileType FileType
        {
            get
            {
                return _fileTypeProperty;
            }

            set
            {
                if (_fileTypeProperty == value)
                {
                    return;
                }

                _fileTypeProperty = value;
                RaisePropertyChanged(FileTypePropertyName);

                initInfoByFileType(value);
            }
        }

        private void initInfoByFileType(enFileType value)
        {
            //从FileTypeConfig.xml中初始化信息
            XmlDocument doc = new XmlDocument();

            using (var ms = new MemoryStream(RPAStudio.Properties.Resources.FileTypeConfig))
            {
                ms.Flush();
                ms.Position = 0;
                doc.Load(ms);
                ms.Close();
            }

            var rootNode = doc.DocumentElement;

            var nodeTypeStr = value.ToString();
            var processElement = rootNode.SelectSingleNode(nodeTypeStr) as XmlElement;
            Title = processElement.GetAttribute("Title");
            Description = processElement.GetAttribute("Description");
            var fileName = processElement.GetAttribute("FileName");

            var newfileNameWithExt = Common.GetValidFileName(FilePath, fileName + @".xaml", "", "{0}", 1);
            FileName = Path.GetFileNameWithoutExtension(newfileNameWithExt);
        }


        /// <summary>
        /// Initializes a new instance of the NewXamlFileViewModel class.
        /// </summary>
        public NewXamlFileViewModel()
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
        /// The <see cref="IsFileNameCorrect" /> property's name.
        /// </summary>
        public const string IsFileNameCorrectPropertyName = "IsFileNameCorrect";

        private bool _isFileNameCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsFileNameCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsFileNameCorrect
        {
            get
            {
                return _isFileNameCorrectProperty;
            }

            set
            {
                if (_isFileNameCorrectProperty == value)
                {
                    return;
                }

                _isFileNameCorrectProperty = value;
                RaisePropertyChanged(IsFileNameCorrectPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="FileNameValidatedWrongTip" /> property's name.
        /// </summary>
        public const string FileNameValidatedWrongTipPropertyName = "FileNameValidatedWrongTip";

        private string _fileNameValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the FileNameValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FileNameValidatedWrongTip
        {
            get
            {
                return _fileNameValidatedWrongTipProperty;
            }

            set
            {
                if (_fileNameValidatedWrongTipProperty == value)
                {
                    return;
                }

                _fileNameValidatedWrongTipProperty = value;
                RaisePropertyChanged(FileNameValidatedWrongTipPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="FileName" /> property's name.
        /// </summary>
        public const string FileNamePropertyName = "FileName";

        private string _fileNameProperty = "";

        /// <summary>
        /// Sets and gets the FileName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileNameProperty;
            }

            set
            {
                if (_fileNameProperty == value)
                {
                    return;
                }

                _fileNameProperty = value;
                RaisePropertyChanged(FileNamePropertyName);

                fileNameValidate(value);
            }
        }

        private void fileNameValidate(string value)
        {
            IsFileNameCorrect = true;

            if (string.IsNullOrEmpty(value))
            {
                IsFileNameCorrect = false;
                FileNameValidatedWrongTip = "名称不能为空";
            }
            else
            {
                if (value.Contains(@"\") || value.Contains(@"/"))
                {
                    IsFileNameCorrect = false;
                    FileNameValidatedWrongTip = "名称不能有非法字符";
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
                        IsFileNameCorrect = false;
                        FileNameValidatedWrongTip = "名称不能有非法字符";
                    }
                    else
                    {
                        // file name is valid... May check for existence by calling fi.Exists.
                    }
                }
            }

            if(File.Exists(FilePath+@"\"+FileName+@".xaml"))
            {
                IsFileNameCorrect = false;
                FileNameValidatedWrongTip = "指定的文件已存在";
            }

            CreateFileCommand.RaiseCanExecuteChanged();
        }




        /// <summary>
        /// The <see cref="IsFilePathCorrect" /> property's name.
        /// </summary>
        public const string IsFilePathCorrectPropertyName = "IsFilePathCorrect";

        private bool _isFilePathCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsFilePathCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsFilePathCorrect
        {
            get
            {
                return _isFilePathCorrectProperty;
            }

            set
            {
                if (_isFilePathCorrectProperty == value)
                {
                    return;
                }

                _isFilePathCorrectProperty = value;
                RaisePropertyChanged(IsFilePathCorrectPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="FilePathValidatedWrongTip" /> property's name.
        /// </summary>
        public const string FilePathValidatedWrongTipPropertyName = "FilePathValidatedWrongTip";

        private string _filePathValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the FilePathValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FilePathValidatedWrongTip
        {
            get
            {
                return _filePathValidatedWrongTipProperty;
            }

            set
            {
                if (_filePathValidatedWrongTipProperty == value)
                {
                    return;
                }

                _filePathValidatedWrongTipProperty = value;
                RaisePropertyChanged(FilePathValidatedWrongTipPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="FilePath" /> property's name.
        /// </summary>
        public const string FilePathPropertyName = "FilePath";

        private string _filePathProperty = "";

        /// <summary>
        /// Sets and gets the FilePath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FilePath
        {
            get
            {
                return _filePathProperty;
            }

            set
            {
                if (_filePathProperty == value)
                {
                    return;
                }

                _filePathProperty = value;
                RaisePropertyChanged(FilePathPropertyName);

                filePathValidate(value);
                fileNameValidate(FileName);
            }
        }

        private void filePathValidate(string value)
        {
            IsFilePathCorrect = true;
            if (string.IsNullOrEmpty(value))
            {
                IsFilePathCorrect = false;
                FilePathValidatedWrongTip = "位置不能为空";
            }
            else
            {
                if (!Directory.Exists(value))
                {
                    IsFilePathCorrect = false;
                    FilePathValidatedWrongTip = "指定的位置不存在";
                }
            }

            //判断是否是在项目目录中的子目录里
            if(!value.IsSubPathOf(ProjectPath))
            {
                IsFilePathCorrect = false;
                FilePathValidatedWrongTip = "指定的位置必须是项目所在目录或其子目录中";
            }

            CreateFileCommand.RaiseCanExecuteChanged();
        }

        private RelayCommand _selectFilePathCommand;

        /// <summary>
        /// Gets the SelectFilePathCommand.
        /// </summary>
        public RelayCommand SelectFilePathCommand
        {
            get
            {
                return _selectFilePathCommand
                    ?? (_selectFilePathCommand = new RelayCommand(
                    () =>
                    {
                        string dst_dir = "";
                        if (Common.ShowSelectDirDialog(Title, ref dst_dir))
                        {
                            FilePath = dst_dir;
                        }
                    }));
            }
        }



        private RelayCommand _createFileCommand;

        /// <summary>
        /// Gets the CreateFileCommand.
        /// </summary>
        public RelayCommand CreateFileCommand
        {
            get
            {
                return _createFileCommand
                    ?? (_createFileCommand = new RelayCommand(
                    () =>
                    {
                        var xamlFilePath = newXamlFile();
                        ViewModelLocator.Instance.Project.RefreshCommand.Execute(null);
                        if(!string.IsNullOrEmpty(xamlFilePath))
                        {
                            var item = ViewModelLocator.Instance.Project.GetProjectTreeItemByFullPath(xamlFilePath);
                            if(item != null)
                            {
                                item.OpenXamlCommand.Execute(null);
                            }
                        }

                        m_view.Close();
                    },
                    () => IsFileNameCorrect && IsFilePathCorrect));
            }
        }







        private string newXamlFile()
        {
            var retPath = "";
            byte[] data = null;

            switch(FileType)
            {
                case enFileType.Sequence:
                    data = Properties.Resources.Sequence;
                    break;
                case enFileType.Flowchart:
                    data = Properties.Resources.Flowchart;
                    break;
                case enFileType.StateMachine:
                    data = Properties.Resources.StateMachine;
                    break;
                case enFileType.GlobalHandler:
                    data = Properties.Resources.GlobalHandler;
                    break;
            }

            if (data != null)
            {
                string str = System.Text.Encoding.UTF8.GetString(data);
                str = str.Replace("{{title}}", FileName);
                data = System.Text.Encoding.UTF8.GetBytes(str);
            }

            if (data != null)
            {
                retPath = FilePath + @"\" + FileName + @".xaml";
                FileStream fileStream = new FileStream(retPath, FileMode.CreateNew);
                fileStream.Write(data, 0, (int)(data.Length));
                fileStream.Close();
            }

            return retPath;
            
        }






    }
}
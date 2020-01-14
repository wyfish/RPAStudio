using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Localization;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RenameViewModel : ViewModelBase
    {
        private Window m_view;

        public string Path { get; set; }
        public string NewPath { get; set; }

        public bool IsDirectory { get; internal set; }
        public string Dir { get; internal set; }
        public bool IsMain { get; internal set; }

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



        private RelayCommand<RoutedEventArgs> _dstNameLoadedCommand;

        /// <summary>
        /// Gets the DstNameLoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> DstNameLoadedCommand
        {
            get
            {
                return _dstNameLoadedCommand
                    ?? (_dstNameLoadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        var textBox = (TextBox)p.Source;
                        textBox.Focus();
                        textBox.SelectAll();
                    }));
            }
        }



        /// <summary>
        /// Initializes a new instance of the RenameViewModel class.
        /// </summary>
        public RenameViewModel()
        {
            
        }



        /// <summary>
        /// The <see cref="SrcName" /> property's name.
        /// </summary>
        public const string SrcNamePropertyName = "SrcName";

        private string _srcNameProperty = "";

        /// <summary>
        /// Sets and gets the SrcName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SrcName
        {
            get
            {
                return _srcNameProperty;
            }

            set
            {
                if (_srcNameProperty == value)
                {
                    return;
                }

                _srcNameProperty = value;
                RaisePropertyChanged(SrcNamePropertyName);
            }
        }




        /// <summary>
        /// The <see cref="IsDstNameCorrect" /> property's name.
        /// </summary>
        public const string IsDstNameCorrectPropertyName = "IsDstNameCorrect";

        private bool _isDstNameCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsDstNameCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDstNameCorrect
        {
            get
            {
                return _isDstNameCorrectProperty;
            }

            set
            {
                if (_isDstNameCorrectProperty == value)
                {
                    return;
                }

                _isDstNameCorrectProperty = value;
                RaisePropertyChanged(IsDstNameCorrectPropertyName);
            }
        }

        


        /// <summary>
        /// The <see cref="DstNameValidatedWrongTip" /> property's name.
        /// </summary>
        public const string DstNameValidatedWrongTipPropertyName = "DstNameValidatedWrongTip";

        private string _dstNameValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the DstNameValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DstNameValidatedWrongTip
        {
            get
            {
                return _dstNameValidatedWrongTipProperty;
            }

            set
            {
                if (_dstNameValidatedWrongTipProperty == value)
                {
                    return;
                }

                _dstNameValidatedWrongTipProperty = value;
                RaisePropertyChanged(DstNameValidatedWrongTipPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="DstName" /> property's name.
        /// </summary>
        public const string DstNamePropertyName = "DstName";

        private string _dstNameProperty = "";

        /// <summary>
        /// Sets and gets the DstName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DstName
        {
            get
            {
                return _dstNameProperty;
            }

            set
            {
                if (_dstNameProperty == value)
                {
                    return;
                }

                _dstNameProperty = value;
                RaisePropertyChanged(DstNamePropertyName);

                dstNameValidate(value);
            }
        }

        private void dstNameValidate(string value)
        {
            IsDstNameCorrect = true;

            if (string.IsNullOrEmpty(value))
            {
                IsDstNameCorrect = false;
                // 名称不能为空
                DstNameValidatedWrongTip = ResxIF.GetString("NameIsRequired");
            }
            else
            {
                if (value.Contains(@"\") || value.Contains(@"/"))
                {
                    IsDstNameCorrect = false;
                    // 名称不能有非法字符
                    DstNameValidatedWrongTip = ResxIF.GetString("NameHasIlligalCharacter");
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
                        IsDstNameCorrect = false;
                        DstNameValidatedWrongTip = ResxIF.GetString("NameHasIlligalCharacter");
                    }
                    else
                    {
                        // file name is valid... May check for existence by calling fi.Exists.
                    }
                }
            }

            var dstFullPath = Dir + @"\" + DstName;
            if (Directory.Exists(dstFullPath))
            {
                IsDstNameCorrect = false;
                // 相同名字的目录已存在
                DstNameValidatedWrongTip = ResxIF.GetString("SameNameDirectoryAlreadyExists");
            }
            else if (File.Exists(dstFullPath))
            {
                IsDstNameCorrect = false;
                // 相同名字的文件已存在
                DstNameValidatedWrongTip = ResxIF.GetString("SameNameFileAlreadyExists");
            }


            OkCommand.RaiseCanExecuteChanged();
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
                        if(DstName != SrcName)
                        {
                            if(IsDirectory)
                            {
                                NewPath = Dir + @"\" + DstName;
                                Directory.Move(Dir+@"\"+SrcName, NewPath);
                            }
                            else
                            {
                                NewPath = Dir + @"\" + DstName;
                                File.Move(Dir + @"\" + SrcName, NewPath);
                            }

                            if (IsDirectory)
                            {
                                if(ProjectTreeItem.IsExpandedDict.ContainsKey(Path))
                                {
                                    var isExpanded = ProjectTreeItem.IsExpandedDict[Path];
                                    ProjectTreeItem.IsExpandedDict.Remove(Path);
                                    ProjectTreeItem.IsExpandedDict[NewPath] = isExpanded;
                                }
                            }

                            Messenger.Default.Send(this, "Rename");
                        }

                        m_view.Close();
                    },
                    () => IsDstNameCorrect));
            }
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
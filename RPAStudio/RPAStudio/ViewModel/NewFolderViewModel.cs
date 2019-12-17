using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using System.Windows;
using System;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NewFolderViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Window m_view;

        public string Path { get; internal set; }

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


        private RelayCommand<RoutedEventArgs> _folderNameLoadedCommand;

        /// <summary>
        /// Gets the FolderNameLoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> FolderNameLoadedCommand
        {
            get
            {
                return _folderNameLoadedCommand
                    ?? (_folderNameLoadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        var textBox = (TextBox)p.Source;
                        textBox.Focus();
                        textBox.SelectAll();
                    }));
            }
        }



        /// <summary>
        /// The <see cref="IsFolderNameCorrect" /> property's name.
        /// </summary>
        public const string IsFolderNameCorrectPropertyName = "IsFolderNameCorrect";

        private bool _isFolderNameCorrectProperty = false;

        /// <summary>
        /// Sets and gets the IsFolderNameCorrect property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsFolderNameCorrect
        {
            get
            {
                return _isFolderNameCorrectProperty;
            }

            set
            {
                if (_isFolderNameCorrectProperty == value)
                {
                    return;
                }

                _isFolderNameCorrectProperty = value;
                RaisePropertyChanged(IsFolderNameCorrectPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="FolderNameValidatedWrongTip" /> property's name.
        /// </summary>
        public const string FolderNameValidatedWrongTipPropertyName = "FolderNameValidatedWrongTip";

        private string _folderNameValidatedWrongTipProperty = "";

        /// <summary>
        /// Sets and gets the FolderNameValidatedWrongTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FolderNameValidatedWrongTip
        {
            get
            {
                return _folderNameValidatedWrongTipProperty;
            }

            set
            {
                if (_folderNameValidatedWrongTipProperty == value)
                {
                    return;
                }

                _folderNameValidatedWrongTipProperty = value;
                RaisePropertyChanged(FolderNameValidatedWrongTipPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="FolderName" /> property's name.
        /// </summary>
        public const string FolderNamePropertyName = "FolderName";

        private string _folderNameProperty = "";

        /// <summary>
        /// Sets and gets the FolderName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FolderName
        {
            get
            {
                return _folderNameProperty;
            }

            set
            {
                if (_folderNameProperty == value)
                {
                    return;
                }

                _folderNameProperty = value;
                RaisePropertyChanged(FolderNamePropertyName);

                folderNameValidate(value);
            }
        }

        private void folderNameValidate(string value)
        {
            IsFolderNameCorrect = true;

            if (string.IsNullOrEmpty(value))
            {
                IsFolderNameCorrect = false;
                FolderNameValidatedWrongTip = "名称不能为空";
            }
            else
            {
                if (value.Contains(@"\") || value.Contains(@"/"))
                {
                    IsFolderNameCorrect = false;
                    FolderNameValidatedWrongTip = "名称不能有非法字符";
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
                        IsFolderNameCorrect = false;
                        FolderNameValidatedWrongTip = "名称不能有非法字符";
                    }
                    else
                    {
                        // file name is valid... May check for existence by calling fi.Exists.
                    }
                }
            }

            if (Directory.Exists(Path + @"\" + FolderName))
            {
                IsFolderNameCorrect = false;
                FolderNameValidatedWrongTip = "已经存在同名称的目录";
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
                        //新建文件夹，然后刷新项目文件显示
                        Directory.CreateDirectory(Path+@"\"+FolderName);
                        Messenger.Default.Send(this, "AddNewFolder");

                        m_view.Close();
                    },
                    () => IsFolderNameCorrect));
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
                    },
                    () => true));
            }
        }

        



        /// <summary>
        /// Initializes a new instance of the NewFolderViewModel class.
        /// </summary>
        public NewFolderViewModel()
        {
        }












    }
}
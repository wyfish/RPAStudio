using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using RPAStudio.Librarys;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation.Metadata;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Resources;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Windows;
using System.Windows.Threading;
using Plugins.Shared.Library;
using RPAStudio.Executor;
using System.Activities.XamlIntegration;
using System.Activities.Validation;
using System.Diagnostics;
using System.ComponentModel;
using System.Activities.Presentation.View;
using System.Windows.Input;
using System.Xml;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Window m_view;

        public DebuggerManager m_debuggerManager { get; set; }

        public RunManager m_runManager { get; set; }

        public string ProjectPath { get; private set; }

        Stopwatch workflowExecutorStopwatch = new Stopwatch();

        public static Dictionary<string, BitmapImage> SystemIconDic = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            (new DesignerMetadata()).Register();
            LoadToolboxIconsForBuiltInActivities();

            UpdateSharedObject();

            Messenger.Default.Register<DebuggerManager>(this, "BeginRun", BeginRun);
            Messenger.Default.Register<RunManager>(this, "BeginRun", BeginRun);

            Messenger.Default.Register<DebuggerManager>(this, "EndRun", EndRun);
            Messenger.Default.Register<RunManager>(this, "EndRun", EndRun);

            Messenger.Default.Register<DockViewModel>(this, "DocumentsCountChanged", DocumentsCountChanged);
            Messenger.Default.Register<MessengerObjects.ProjectStateChanged>(this, OnProjectStateChangedMessage);
        }


        /// <summary>
        /// 窗体加载完成后调用
        /// </summary>
        private void Init()
        {
            //检查更新
            ViewModelLocator.Instance.CheckUpgrade.CheckUpgradeCommand.Execute(null);

            m_view.Activate();//激活，以便安装包安装完成后打开时置于最前
        }



        public DesignerView CurrentDesignerView
        {
            get
            {
                if(ViewModelLocator.Instance.Dock.ActiveDocument == null)
                {
                    return null;
                }

                return ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance.Context.Services.GetService<DesignerView>();
            }
        }

        public ICommand CopyCommand
        {
            get { return DesignerView.CopyCommand; }
        }

        public ICommand CutCommand
        {
            get { return DesignerView.CutCommand; }
        }

        public ICommand PasteCommand
        {
            get { return DesignerView.PasteCommand; }
        }

        /// <summary>
        /// 是否需要升级
        /// </summary>
        public const string IsNeedUpgradePropertyName = "IsNeedUpgrade";

        private bool _isNeedUpgradeProperty = false;

        /// <summary>
        /// Sets and gets the IsNeedUpgrade property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsNeedUpgrade
        {
            get
            {
                return _isNeedUpgradeProperty;
            }

            set
            {
                if (_isNeedUpgradeProperty == value)
                {
                    return;
                }

                _isNeedUpgradeProperty = value;
                RaisePropertyChanged(IsNeedUpgradePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsOpenStartScreen" /> property's name.
        /// </summary>
        public const string IsOpenStartScreenPropertyName = "IsOpenStartScreen";

        private bool _isOpenStartScreenProperty = true;

        /// <summary>
        /// Sets and gets the IsOpenStartScreen property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsOpenStartScreen
        {
            get
            {
                return _isOpenStartScreenProperty;
            }

            set
            {
                if (_isOpenStartScreenProperty == value)
                {
                    return;
                }

                _isOpenStartScreenProperty = value;
                RaisePropertyChanged(IsOpenStartScreenPropertyName);
            }
        }


        /// <summary>
        /// 是否起始页正忙（正忙则显示遮罩层）
        /// </summary>
        public const string IsStartContentBusyPropertyName = "IsStartContentBusy";

        private bool _isStartContentBusyProperty = false;

        /// <summary>
        /// Sets and gets the IsStartContentBusy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsStartContentBusy
        {
            get
            {
                return _isStartContentBusyProperty;
            }

            set
            {
                if (_isStartContentBusyProperty == value)
                {
                    return;
                }

                _isStartContentBusyProperty = value;
                RaisePropertyChanged(IsStartContentBusyPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsBackButtonVisible" /> property's name.
        /// </summary>
        public const string IsBackButtonVisiblePropertyName = "IsBackButtonVisible";

        private bool _isBackButtonVisibleProperty = false;

        /// <summary>
        /// Sets and gets the IsBackButtonVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBackButtonVisible
        {
            get
            {
                return _isBackButtonVisibleProperty;
            }

            set
            {
                if (_isBackButtonVisibleProperty == value)
                {
                    return;
                }

                _isBackButtonVisibleProperty = value;
                RaisePropertyChanged(IsBackButtonVisiblePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsProjectOpened" /> property's name.
        /// </summary>
        public const string IsProjectOpenedPropertyName = "IsProjectOpened";

        private bool _isProjectOpenedProperty = false;

        /// <summary>
        /// Sets and gets the IsProjectOpened property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsProjectOpened
        {
            get
            {
                return _isProjectOpenedProperty;
            }

            set
            {
                if (_isProjectOpenedProperty == value)
                {
                    return;
                }

                _isProjectOpenedProperty = value;
                RaisePropertyChanged(IsProjectOpenedPropertyName);

                CloseProjectCommand.RaiseCanExecuteChanged();
                NewSequenceDocumentCommand.RaiseCanExecuteChanged();
            }
        }





        /// <summary>
        /// The <see cref="IsDocumentExist" /> property's name.
        /// </summary>
        public const string IsDocumentExistPropertyName = "IsDocumentExist";

        private bool _isDocumentExistProperty = false;

        /// <summary>
        /// Sets and gets the IsDocumentExist property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDocumentExist
        {
            get
            {
                return _isDocumentExistProperty;
            }

            set
            {
                if (_isDocumentExistProperty == value)
                {
                    return;
                }

                _isDocumentExistProperty = value;
                RaisePropertyChanged(IsDocumentExistPropertyName);

                //ribbon控件强刷CommandManager.InvalidateRequerySuggested()失败，原因未知
                RunWorkflowCommand.RaiseCanExecuteChanged();
                StopWorkflowCommand.RaiseCanExecuteChanged();
                DebugOrContinueWorkflowCommand.RaiseCanExecuteChanged();

                StepIntoCommand.RaiseCanExecuteChanged();
                StepOverCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// The <see cref="IsWorkflowRunningOrDebugging" /> property's name.
        /// </summary>
        public const string IsWorkflowRunningOrDebuggingPropertyName = "IsWorkflowRunningOrDebugging";

        private bool _isWorkflowRunningOrDebuggingProperty = false;

        /// <summary>
        /// Sets and gets the IsWorkflowRunningOrDebugging property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWorkflowRunningOrDebugging
        {
            get
            {
                return _isWorkflowRunningOrDebuggingProperty;
            }

            set
            {
                if (_isWorkflowRunningOrDebuggingProperty == value)
                {
                    return;
                }

                _isWorkflowRunningOrDebuggingProperty = value;
                RaisePropertyChanged(IsWorkflowRunningOrDebuggingPropertyName);


                RunWorkflowCommand.RaiseCanExecuteChanged();
                StopWorkflowCommand.RaiseCanExecuteChanged();
                DebugOrContinueWorkflowCommand.RaiseCanExecuteChanged();

                StepIntoCommand.RaiseCanExecuteChanged();
                StepOverCommand.RaiseCanExecuteChanged();


                if(value)
                {
                    DebugOrContinueWorkflowButtonHeader = "继续";
                }
                else
                {
                    DebugOrContinueWorkflowButtonHeader = "调试";
                }
            }
        }




        /// <summary>
        /// The <see cref="IsWorkflowRunning" /> property's name.
        /// </summary>
        public const string IsWorkflowRunningPropertyName = "IsWorkflowRunning";

        private bool _isWorkflowRunningProperty = false;

        /// <summary>
        /// 当前是否正在运行状态
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWorkflowRunning
        {
            get
            {
                return _isWorkflowRunningProperty;
            }

            set
            {
                if (_isWorkflowRunningProperty == value)
                {
                    return;
                }

                _isWorkflowRunningProperty = value;
                RaisePropertyChanged(IsWorkflowRunningPropertyName);

                DebugOrContinueWorkflowCommand.RaiseCanExecuteChanged();
                StepIntoCommand.RaiseCanExecuteChanged();
                StepOverCommand.RaiseCanExecuteChanged();
            }
        }





        

        /// <summary>
        /// The <see cref="IsWorkflowDebugging" /> property's name.
        /// </summary>
        public const string IsWorkflowDebuggingPropertyName = "IsWorkflowDebugging";

        private bool _isWorkflowDebuggingProperty = false;

        /// <summary>
        /// 当前是否正在调试状态
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWorkflowDebugging
        {
            get
            {
                return _isWorkflowDebuggingProperty;
            }

            set
            {
                if (_isWorkflowDebuggingProperty == value)
                {
                    return;
                }

                _isWorkflowDebuggingProperty = value;
                RaisePropertyChanged(IsWorkflowDebuggingPropertyName);

                BreakCommand.RaiseCanExecuteChanged();
                DebugOrContinueWorkflowCommand.RaiseCanExecuteChanged();
                StepIntoCommand.RaiseCanExecuteChanged();
                StepOverCommand.RaiseCanExecuteChanged();
            }
        }



        /// <summary>
        /// The <see cref="IsWorkflowDebuggingPaused" /> property's name.
        /// </summary>
        public const string IsWorkflowDebuggingPausedPropertyName = "IsWorkflowDebuggingPaused";

        private bool _isWorkflowDebuggingPausedProperty = false;

        /// <summary>
        /// 当前是否正在调试中断状态
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWorkflowDebuggingPaused
        {
            get
            {
                return _isWorkflowDebuggingPausedProperty;
            }

            set
            {
                if (_isWorkflowDebuggingPausedProperty == value)
                {
                    return;
                }

                _isWorkflowDebuggingPausedProperty = value;
                RaisePropertyChanged(IsWorkflowDebuggingPausedPropertyName);

                BreakCommand.RaiseCanExecuteChanged();
                DebugOrContinueWorkflowCommand.RaiseCanExecuteChanged();
                StepIntoCommand.RaiseCanExecuteChanged();
                StepOverCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// The <see cref="SlowStepSpeed" /> property's name.
        /// </summary>
        public const string SlowStepSpeedPropertyName = "SlowStepSpeed";

        private DebuggerManager.enSpeed _slowStepSpeedProperty = DebuggerManager.enSpeed.Off;

        /// <summary>
        /// Sets and gets the SlowStepSpeed property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DebuggerManager.enSpeed SlowStepSpeed
        {
            get
            {
                return _slowStepSpeedProperty;
            }

            set
            {
                if (_slowStepSpeedProperty == value)
                {
                    return;
                }

                _slowStepSpeedProperty = value;
                RaisePropertyChanged(SlowStepSpeedPropertyName);

                if(value == DebuggerManager.enSpeed.Off)
                {
                    SlowStepBackground = "Transparent";
                }
                else
                {
                    SlowStepBackground = "#c2c2c2";
                }

                switch(value)
                {
                    case DebuggerManager.enSpeed.Off:
                        SlowStepIcon = "pack://application:,,,/Resources/Image/Ribbon/slow-step-off.png";
                        break;
                    case DebuggerManager.enSpeed.One:
                        SlowStepIcon = "pack://application:,,,/Resources/Image/Ribbon/slow-step-1x.png";
                        break;
                    case DebuggerManager.enSpeed.Two:
                        SlowStepIcon = "pack://application:,,,/Resources/Image/Ribbon/slow-step-2x.png";
                        break;
                    case DebuggerManager.enSpeed.Three:
                        SlowStepIcon = "pack://application:,,,/Resources/Image/Ribbon/slow-step-3x.png";
                        break;
                    case DebuggerManager.enSpeed.Four:
                        SlowStepIcon = "pack://application:,,,/Resources/Image/Ribbon/slow-step-4x.png";
                        break;
                }
            }
        }


        /// <summary>
        /// The <see cref="SlowStepIcon" /> property's name.
        /// </summary>
        public const string SlowStepIconPropertyName = "SlowStepIcon";

        private string _slowStepIconProperty = "pack://application:,,,/Resources/Image/Ribbon/slow-step-off.png";

        /// <summary>
        /// Sets and gets the SlowStepIcon property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SlowStepIcon
        {
            get
            {
                return _slowStepIconProperty;
            }

            set
            {
                if (_slowStepIconProperty == value)
                {
                    return;
                }

                _slowStepIconProperty = value;
                RaisePropertyChanged(SlowStepIconPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="SlowStepBackground" /> property's name.
        /// </summary>
        public const string SlowStepBackgroundPropertyName = "SlowStepBackground";

        private string _slowStepBackgroundProperty = "Transparent";

        /// <summary>
        /// Sets and gets the SlowStepBackground property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SlowStepBackground
        {
            get
            {
                return _slowStepBackgroundProperty;
            }

            set
            {
                if (_slowStepBackgroundProperty == value)
                {
                    return;
                }

                _slowStepBackgroundProperty = value;
                RaisePropertyChanged(SlowStepBackgroundPropertyName);
            }
        }



        private RelayCommand _slowStepCommand;

        /// <summary>
        /// Gets the SlowStepCommand.
        /// </summary>
        public RelayCommand SlowStepCommand
        {
            get
            {
                return _slowStepCommand
                    ?? (_slowStepCommand = new RelayCommand(
                    () =>
                    {
                        switch(SlowStepSpeed)
                        {
                            case DebuggerManager.enSpeed.Off:
                                SlowStepSpeed = DebuggerManager.enSpeed.One;
                                break;
                            case DebuggerManager.enSpeed.One:
                                SlowStepSpeed = DebuggerManager.enSpeed.Two;
                                break;
                            case DebuggerManager.enSpeed.Two:
                                SlowStepSpeed = DebuggerManager.enSpeed.Three;
                                break;
                            case DebuggerManager.enSpeed.Three:
                                SlowStepSpeed = DebuggerManager.enSpeed.Four;
                                break;
                            case DebuggerManager.enSpeed.Four:
                                SlowStepSpeed = DebuggerManager.enSpeed.Off;
                                break;
                            default:
                                break;
                        }
                    }));
            }
        }



        private void OnProjectStateChangedMessage(MessengerObjects.ProjectStateChanged obj)
        {
            Common.RunInUI(()=> {
                ProjectPath = obj.ProjectPath;
                Title = obj.ProjectName;

                IsProjectOpened = obj.IsOpen;

                UpdateSharedObject();
            });
        }

        private void UpdateSharedObject()
        {
            SharedObject.Instance.ProjectPath = ProjectPath;
            SharedObject.Instance.isHighlightElements = IsHighlightElements;
            SharedObject.Instance.SetOutputFun(LogToOutputWindow);
        }

        private void LogToOutputWindow(SharedObject.enOutputType type, string msg, string msgDetails)
        {
            Logger.Info(string.Format("LogToOutputWindow：type={0},msg={1},msgDetails={2}", type.ToString(), msg, msgDetails), logger);

            Common.RunInUI(() => {
                ViewModelLocator.Instance.Output.Log(type, msg, msgDetails);
            });
        }

        private void DocumentsCountChanged(DockViewModel vm)
        {
            IsDocumentExist = ViewModelLocator.Instance.Dock.Documents.Count > 0;
        }

        private void BeginRun(Object obj)
        {
            ViewModelLocator.Instance.Output.ClearAllCommand.Execute(null);

            workflowExecutorStopwatch.Restart();
            SharedObject.Instance.Output(SharedObject.enOutputType.Information, string.Format("{0} 开始运行", ViewModelLocator.Instance.Project.ProjectName));

            IsWorkflowRunningOrDebugging = true;

            if (obj is DebuggerManager)
            {
                IsWorkflowDebugging = true;
            }

            if(obj is RunManager)
            {
                IsWorkflowRunning = true;
            }

            foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
            {
                doc.IsReadOnly = true;
            }

            if (obj is DebuggerManager)
            {
                //调试状态时不用最小化

                ViewModelLocator.Instance.Dock.ActiveDocument.IsDebugging = true;
            }
            else
            {
                m_view.WindowState = WindowState.Minimized;
            }

        }

        private void EndRun(Object obj)
        {
            Common.RunInUI(()=> {

                workflowExecutorStopwatch.Stop();

                string elapsedTime = "";
                var elapsedTimeSpan = workflowExecutorStopwatch.Elapsed.Duration();
                if (workflowExecutorStopwatch.Elapsed.Days > 0)
                {
                    elapsedTime = elapsedTimeSpan.ToString("%d") + " day(s) " + elapsedTimeSpan.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    elapsedTime = elapsedTimeSpan.ToString(@"hh\:mm\:ss");
                }

                SharedObject.Instance.Output(SharedObject.enOutputType.Information, string.Format("{0} 结束运行，耗时：{1}", ViewModelLocator.Instance.Project.ProjectName, elapsedTime));

                foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
                {
                    if(!doc.IsAlwaysReadOnly)
                    {
                        doc.IsReadOnly = false;
                    }
                }

                if (obj is DebuggerManager)
                {
                    //调试状态时不用还原窗口状态

                    foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
                    {
                        doc.IsDebugging = false;
                    }
                    
                }
                else
                {
                    if (m_view.WindowState == WindowState.Minimized)
                    {
                        m_view.WindowState = WindowState.Normal;
                    }
                }
                

                IsWorkflowRunningOrDebugging = false;

                if (obj is DebuggerManager)
                {
                    IsWorkflowDebugging = false;
                }

                if (obj is RunManager)
                {
                    IsWorkflowRunning = false;
                }

            });
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
                        Logger.Debug("+++++主窗口启动+++++", logger);
                        m_view = (Window)p.Source;

                        Init();
                    }));
            }
        }

        private RelayCommand _unloadedCommand;

        /// <summary>
        /// Gets the UnloadedCommand.
        /// </summary>
        public RelayCommand UnloadedCommand
        {
            get
            {
                return _unloadedCommand
                    ?? (_unloadedCommand = new RelayCommand(
                    () =>
                    {
                        Logger.Debug("-----主窗口结束-----", logger);
                    }));
            }
        }



        private RelayCommand<CancelEventArgs> _closingCommand;

        /// <summary>
        /// Gets the ClosingCommand.
        /// </summary>
        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand
                    ?? (_closingCommand = new RelayCommand<CancelEventArgs>(
                    p =>
                    {
                        //关闭主窗口前确认
                        bool bContinueClose = DoCloseProject();

                        if (!bContinueClose)
                        {
                            p.Cancel = true;
                        }

                    }));
            }
        }


      














        private static Bitmap ExtractBitmapResource(ResourceReader resourceReader, string bitmapName)
        {
            var dictEnum = resourceReader.GetEnumerator();

            Bitmap bitmap = null;

            while (dictEnum.MoveNext())
            {
                if (Equals(dictEnum.Key, bitmapName))
                {
                    bitmap = dictEnum.Value as Bitmap;

                    if (bitmap != null)
                    {
                        bitmap.MakeTransparent(bitmap.GetPixel(bitmap.Width - 1, 0));

                        //WriteLine等部分透明色的处理
                        bitmap.MakeTransparent(bitmap.GetPixel(bitmap.Width - 1, bitmap.Height - 1));
                        bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                        //bitmap.Save("d:\\temp\\" + bitmapName+".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    break;
                }
            }

            return bitmap;
        }


        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private static void CreateToolboxBitmapAttributeForActivity(
            AttributeTableBuilder builder, ResourceReader resourceReader, Type builtInActivityType)
        {
            var bitmapName = builtInActivityType.IsGenericType ? builtInActivityType.Name.Split('`')[0] : builtInActivityType.Name;
            var bitmap = ExtractBitmapResource(
                resourceReader,
                bitmapName);

            if (bitmap == null)
            {
                return;
            }

            SystemIconDic[bitmapName] = BitmapToImageSource(bitmap);
            SystemIconDic[builtInActivityType.Name] = BitmapToImageSource(bitmap);

            var tbaType = typeof(ToolboxBitmapAttribute);

            var imageType = typeof(Image);

            var constructor = tbaType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { imageType, imageType }, null);

            var tba = constructor.Invoke(new object[] { bitmap, bitmap }) as ToolboxBitmapAttribute;

            builder.AddCustomAttributes(builtInActivityType, tba);
        }

        private static void LoadToolboxIconsForBuiltInActivities()
        {
            try
            {
                var sourceAssembly = Assembly.LoadFrom(@"Microsoft.VisualStudio.Activities.dll");

                var builder = new AttributeTableBuilder();

                if (sourceAssembly != null)
                {
                    var stream =
                        sourceAssembly.GetManifestResourceStream(
                            "Microsoft.VisualStudio.Activities.Resources.resources");
                    if (stream != null)
                    {
                        var resourceReader = new ResourceReader(stream);

                        foreach (var type in
                            typeof(Activity).Assembly.GetTypes().Where(
                                t => t.Namespace == "System.Activities.Statements"))
                        {
                            CreateToolboxBitmapAttributeForActivity(builder, resourceReader, type);
                        }


                        //FinalState的图标记录
                        foreach (var type in
                            typeof(FinalState).Assembly.GetTypes().Where(
                                t => t.Namespace == "System.Activities.Core.Presentation"))
                        {
                            CreateToolboxBitmapAttributeForActivity(builder, resourceReader, type);
                        }

                    }
                }

                MetadataStore.AddAttributeTable(builder.CreateTable());
            }
            catch (FileNotFoundException)
            {
                // Ignore - will use default icons
            }
        }

        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private readonly string prefixTitle = "RPA机器人流程自动化平台";
        private string _titleProperty = "";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get
            {
                if(_titleProperty == "")
                {
                    return prefixTitle;
                }
                else
                {
                    return prefixTitle+@" - "+_titleProperty;
                }
               
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



        private RelayCommand _selectProjectCommand;

        /// <summary>
        /// Gets the SelectProjectCommand.
        /// </summary>
        public RelayCommand SelectProjectCommand
        {
            get
            {
                return _selectProjectCommand
                    ?? (_selectProjectCommand = new RelayCommand(
                    () =>
                    {
                        ViewModelLocator.Instance.Start.m_view.IsEnabled = false;
                        var fileFullPath = Common.ShowSelectSingleFileDialog("工作流项目文件|project.json|工作流文件|*.xaml");

                        //延迟调用，避免双击选择文件时误触发后面的消息
                        Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                        {
                            ViewModelLocator.Instance.Start.m_view.IsEnabled = true;
                        }), DispatcherPriority.ContextIdle);

                        if (!string.IsNullOrEmpty(fileFullPath))
                        {
                            var fileName = Path.GetFileName(fileFullPath);
                            var fileExt = Path.GetExtension(fileFullPath);
                            var fileDir = Path.GetDirectoryName(fileFullPath);

                            bool hasError = false;
                            string errorMsg = "";

                            if (fileName.EqualsIgnoreCase("project.json"))
                            {
                                //判断project.json文件是否合法
                                if(isProjectJsonFileValid(fileFullPath))
                                {
                                    if (DoCloseProject())
                                    {
                                        addToRecentProjects(fileFullPath);
                                        var msg = new MessengerObjects.ProjectOpen();
                                        msg.ProjectJsonFile = fileFullPath;
                                        Messenger.Default.Send(msg);
                                    }else
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    hasError = true;
                                    errorMsg = "项目配置文件project.json的内容格式不正确";
                                }
                               
                            }
                            else if(fileExt.EqualsIgnoreCase(".xaml"))
                            {
                                //判断当前目录下是否有个project.json
                                var projectJsonFile = fileDir + @"\project.json";
                                if (File.Exists(projectJsonFile))
                                {
                                    //判断project.json文件是否合法
                                    if (isProjectJsonFileValid(projectJsonFile))
                                    {
                                        if (DoCloseProject())
                                        {
                                            addToRecentProjects(projectJsonFile);
                                            var msg = new MessengerObjects.ProjectOpen();
                                            msg.ProjectJsonFile = projectJsonFile;
                                            msg.DefaultOpenXamlFile = fileFullPath;
                                            Messenger.Default.Send(msg);
                                        }
                                        else
                                        {
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        hasError = true;
                                        errorMsg = "项目配置文件project.json的内容格式不正确";
                                    }
                                }
                                else
                                {
                                    hasError = true;
                                    errorMsg = "当前目录未找到项目配置文件project.json";
                                }
                            }
                            else
                            {
                                hasError = true;
                                errorMsg = "非法文件";
                            }

                            if(hasError)
                            {
                                MessageBox.Show(App.Current.MainWindow, errorMsg, "错误", MessageBoxButton.OK,MessageBoxImage.Warning);
                            }
                            else
                            {
                                IsOpenStartScreen = false;
                                IsBackButtonVisible = true;
                            }
                        }


                    }));
            }
        }

        private void addToRecentProjects(string projectJsonFile)
        {
            XmlDocument doc = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RecentProjects.xml";
            doc.Load(path);
            var rootNode = doc.DocumentElement;

            var projectNodes = rootNode.SelectNodes("Project");

            //可能已经存在，则去除旧的列表
            foreach (XmlElement node in projectNodes)
            {
                if(node.Attributes["FilePath"].Value.ToLower() == projectJsonFile.ToLower())
                {
                    rootNode.RemoveChild(node);
                    break;
                }
            }

            //最多记录100条，默认显示个数由XML的MaxShowCount限制
            if (projectNodes.Count > 100)
            {
                rootNode.RemoveChild(rootNode.LastChild);
            }

            var json_cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectJsonConfig>(File.ReadAllText(projectJsonFile));

            XmlElement projectElement = doc.CreateElement("Project");
            projectElement.SetAttribute("FilePath", projectJsonFile);
            projectElement.SetAttribute("Name", json_cfg.name);
            projectElement.SetAttribute("Description", json_cfg.description);

            rootNode.PrependChild(projectElement);

            doc.Save(path);

            //广播RecentProjects.xml改变的消息，以重刷最近项目列表
            Messenger.Default.Send(new MessengerObjects.RecentProjectsModify());
        }

        private bool isProjectJsonFileValid(string fileFullPath)
        {
            try
            {
                var json_cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectJsonConfig>(File.ReadAllText(fileFullPath));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool DoCloseProject()
        {
            //首先遍历所有文档，依次调用关闭TAB页文档功能（TAB页关闭命令有修改的提示用户是否要保存文档）
            var docList = ViewModelLocator.Instance.Dock.Documents.ToList();//先转成list再遍历，不然关闭时会修改Documents集合导致异常
            foreach (var doc in docList)
            {
                if (!doc.DoCloseDocument())
                {
                    return false;//只要点取消了就不往下走了
                }
            }

            if(!string.IsNullOrEmpty(ViewModelLocator.Instance.Main.ProjectPath))
            {
                var msg_close = new MessengerObjects.ProjectClose();
                Messenger.Default.Send(msg_close);
            }
            

            //项目树ViewModel变量重置（树节点清空，搜索框清空，过滤条件清空等）
            ViewModelLocator.Instance.Project.ResetAll();


            //发送ProjectStateChangedMessage关闭窗体消息，以还原主窗口标题
            var state_changed_msg = new MessengerObjects.ProjectStateChanged();
            state_changed_msg.IsOpen = false;
            state_changed_msg.ProjectPath = "";
            state_changed_msg.ProjectName = "";
            Messenger.Default.Send(state_changed_msg);

            //让起始页变成初始未打开工程的状态
            IsBackButtonVisible = false;
            IsProjectOpened = false;

            return true;
        }

        private RelayCommand _closeProjectCommand;

        /// <summary>
        /// Gets the CloseProjectCommand.
        /// </summary>
        public RelayCommand CloseProjectCommand
        {
            get
            {
                return _closeProjectCommand
                    ?? (_closeProjectCommand = new RelayCommand(
                    () =>
                    {
                        //关闭项目
                        if(DoCloseProject())
                        {
                        }
                    },
                    () => IsProjectOpened));
            }
        }




        private RelayCommand _newSequenceDocumentCommand;

        /// <summary>
        /// Gets the NewSequenceDocumentCommand.
        /// </summary>
        public RelayCommand NewSequenceDocumentCommand
        {
            get
            {
                return _newSequenceDocumentCommand
                    ?? (_newSequenceDocumentCommand = new RelayCommand(
                    () =>
                    {
                        //弹出新建序列图
                        var window = new NewXamlFileWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as NewXamlFileViewModel;
                        vm.ProjectPath = ProjectPath;
                        vm.FilePath = ProjectPath;
                        vm.FileType = NewXamlFileViewModel.enFileType.Sequence;
                        window.ShowDialog();
                    },
                    () => IsProjectOpened));
            }
        }






        private RelayCommand _newFlowchartDocumentCommand;

        /// <summary>
        /// Gets the NewFlowchartDocumentCommand.
        /// </summary>
        public RelayCommand NewFlowchartDocumentCommand
        {
            get
            {
                return _newFlowchartDocumentCommand
                    ?? (_newFlowchartDocumentCommand = new RelayCommand(
                    () =>
                    {
                        //弹出新建流程图
                        var window = new NewXamlFileWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as NewXamlFileViewModel;
                        vm.ProjectPath = ProjectPath;
                        vm.FilePath = ProjectPath;
                        vm.FileType = NewXamlFileViewModel.enFileType.Flowchart;
                        window.ShowDialog();
                    }));
            }
        }



        private RelayCommand _newStateMachineDocumentCommand;

        /// <summary>
        /// Gets the NewStateMachineDocumentCommand.
        /// </summary>
        public RelayCommand NewStateMachineDocumentCommand
        {
            get
            {
                return _newStateMachineDocumentCommand
                    ?? (_newStateMachineDocumentCommand = new RelayCommand(
                    () =>
                    {
                        //弹出新建状态机
                        var window = new NewXamlFileWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as NewXamlFileViewModel;
                        vm.ProjectPath = ProjectPath;
                        vm.FilePath = ProjectPath;
                        vm.FileType = NewXamlFileViewModel.enFileType.StateMachine;
                        window.ShowDialog();
                    }));
            }
        }


        private RelayCommand _newGlobalHandlerDocumentCommand;

        /// <summary>
        /// Gets the NewGlobalHandlerDocumentCommand.
        /// </summary>
        public RelayCommand NewGlobalHandlerDocumentCommand
        {
            get
            {
                return _newGlobalHandlerDocumentCommand
                    ?? (_newGlobalHandlerDocumentCommand = new RelayCommand(
                    () =>
                    {
                        //弹出新建全局处理器
                        var window = new NewXamlFileWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as NewXamlFileViewModel;
                        vm.ProjectPath = ProjectPath;
                        vm.FilePath = ProjectPath;
                        vm.FileType = NewXamlFileViewModel.enFileType.GlobalHandler;
                        window.ShowDialog();
                    }));
            }
        }







        private RelayCommand _runWorkflowCommand;

        /// <summary>
        /// Gets the RunWorkflowCommand.
        /// </summary>
        public RelayCommand RunWorkflowCommand
        {
            get
            {
                return _runWorkflowCommand
                    ?? (_runWorkflowCommand = new RelayCommand(
                    () =>
                    {
                        //当前活动窗口执行工作流
                        //TODO WJF 后期最小化、禁用主界面，执行完后恢复等功能实现，可能需要新开一个进程来执行
                        SaveAllCommand.Execute(null);//先全部保存

                        System.GC.Collect();//提醒系统回收内存，避免内存占用过高

                        m_runManager = new RunManager(this, ViewModelLocator.Instance.Dock.ActiveDocument);
                        m_runManager.Run();
                    },
                    () => IsDocumentExist && !IsWorkflowRunningOrDebugging));
            }
        }


        private RelayCommand _stopWorkflowCommand;

        /// <summary>
        /// Gets the StopWorkflowCommand.
        /// </summary>
        public RelayCommand StopWorkflowCommand
        {
            get
            {
                return _stopWorkflowCommand
                    ?? (_stopWorkflowCommand = new RelayCommand(
                    () =>
                    {
                        //停止调试
                        if(IsWorkflowRunning)
                        {
                            m_runManager.Stop();
                        }

                        if(IsWorkflowDebugging)
                        {
                            m_debuggerManager.Stop();
                        }

                    },
                    () => IsWorkflowRunningOrDebugging));
            }
        }



        


        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(
                    () =>
                    {
                        //保存(只保存当前的)
                        var doc = ViewModelLocator.Instance.Dock.ActiveDocument;
                        if(doc != null)
                        {
                            doc.SaveDocument();
                        }
                    },
                    () => true));
            }
        }


        private RelayCommand _saveAsCommand;

        /// <summary>
        /// Gets the SaveAsCommand.
        /// </summary>
        public RelayCommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand
                    ?? (_saveAsCommand = new RelayCommand(
                    () =>
                    {
                        var doc = ViewModelLocator.Instance.Dock.ActiveDocument;

                        string userSelPath;
                        bool ret = Common.ShowSaveAsFileDialog(out userSelPath, doc.Title, ".xaml", "Workflow Files");

                        if (ret == true)
                        {
                            //保存xaml到文件中
                            doc.WorkflowDesignerInstance.Flush();
                            var xamlText = doc.WorkflowDesignerInstance.Text;
                            File.WriteAllText(userSelPath, xamlText);

                            doc.IsDirty = false;
                            doc.XamlPath = userSelPath;
                            doc.Title = Path.GetFileNameWithoutExtension(userSelPath);
                            doc.UpdateCompositeTitle();

                            //如果另存为的路径在当前项目路径下，则需要刷新项目树视图
                            if (userSelPath.IsSubPathOf(ViewModelLocator.Instance.Project.ProjectPath))
                            {
                                ViewModelLocator.Instance.Project.RefreshCommand.Execute(null);
                            }
                        }

                    }));
            }
        }



        private RelayCommand _saveAllCommand;

        /// <summary>
        /// Gets the SaveAllCommand.
        /// </summary>
        public RelayCommand SaveAllCommand
        {
            get
            {
                return _saveAllCommand
                    ?? (_saveAllCommand = new RelayCommand(
                    () =>
                    {
                        foreach(var doc in ViewModelLocator.Instance.Dock.Documents)
                        {
                            doc.SaveDocument();
                        }
                    }));
            }
        }



        /// <summary>
        /// The <see cref="DebugOrContinueWorkflowButtonToolTip" /> property's name.
        /// </summary>
        public const string DebugOrContinueWorkflowButtonToolTipPropertyName = "DebugOrContinueWorkflowButtonToolTip";

        private string _debugOrContinueWorkflowButtonToolTipProperty = "调试（F7）";

        /// <summary>
        /// Sets and gets the DebugOrContinueWorkflowButtonToolTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DebugOrContinueWorkflowButtonToolTip
        {
            get
            {
                return _debugOrContinueWorkflowButtonToolTipProperty;
            }

            set
            {
                if (_debugOrContinueWorkflowButtonToolTipProperty == value)
                {
                    return;
                }

                _debugOrContinueWorkflowButtonToolTipProperty = value;
                RaisePropertyChanged(DebugOrContinueWorkflowButtonToolTipPropertyName);
            }
        }








        /// <summary>
        /// The <see cref="DebugOrContinueWorkflowButtonHeader" /> property's name.
        /// </summary>
        public const string DebugOrContinueWorkflowButtonHeaderPropertyName = "DebugOrContinueWorkflowButtonHeader";

        private string _debugOrContinueWorkflowButtonHeaderProperty = "调试";

        /// <summary>
        /// Sets and gets the DebugOrContinueWorkflowButtonHeader property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DebugOrContinueWorkflowButtonHeader
        {
            get
            {
                return _debugOrContinueWorkflowButtonHeaderProperty;
            }

            set
            {
                if (_debugOrContinueWorkflowButtonHeaderProperty == value)
                {
                    return;
                }

                _debugOrContinueWorkflowButtonHeaderProperty = value;
                RaisePropertyChanged(DebugOrContinueWorkflowButtonHeaderPropertyName);

                DebugOrContinueWorkflowButtonToolTip = string.Format("{0}（F7）", value);
            }
        }






        private RelayCommand _debugOrContinueWorkflowCommand;

        /// <summary>
        /// Gets the DebugOrContinueWorkflowCommand.
        /// </summary>
        public RelayCommand DebugOrContinueWorkflowCommand
        {
            get
            {
                return _debugOrContinueWorkflowCommand
                    ?? (_debugOrContinueWorkflowCommand = new RelayCommand(
                    () =>
                    {
                        if (IsWorkflowDebugging)
                        {
                            m_debuggerManager.Continue();
                        }
                        else
                        {
                            SaveAllCommand.Execute(null);//先全部保存

                            System.GC.Collect();//提醒系统回收内存，避免内存占用过高

                            m_debuggerManager = new DebuggerManager(this, ViewModelLocator.Instance.Dock.ActiveDocument);
                            m_debuggerManager.Debug();
                        }

                    },
                    () =>
                    {

                        if (!IsDocumentExist)
                        {
                            return false;
                        }

                        if (IsWorkflowRunning)
                        {
                            return false;
                        }
                        else if (IsWorkflowDebugging)
                        {
                            if (!IsWorkflowDebuggingPaused)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }


                    }
                    ));
            }
        }
        





        



        private RelayCommand _breakCommand;

        /// <summary>
        /// Gets the BreakCommand.
        /// </summary>
        public RelayCommand BreakCommand
        {
            get
            {
                return _breakCommand
                    ?? (_breakCommand = new RelayCommand(
                    () =>
                    {
                        if(m_debuggerManager != null)
                        {
                            m_debuggerManager.Break();
                        }
                    },
                    () => IsWorkflowDebugging && !IsWorkflowDebuggingPaused));
            }
        }






        private RelayCommand _stepIntoCommand;

        /// <summary>
        /// Gets the StepIntoCommand.
        /// </summary>
        public RelayCommand StepIntoCommand
        {
            get
            {
                return _stepIntoCommand
                    ?? (_stepIntoCommand = new RelayCommand(
                    () =>
                    {
                        if (!IsWorkflowRunningOrDebugging)
                        {
                            SaveAllCommand.Execute(null);//先全部保存

                            m_debuggerManager = new DebuggerManager(this, ViewModelLocator.Instance.Dock.ActiveDocument);
                            m_debuggerManager.SetNextOperate(DebuggerManager.enOperate.StepInto);
                            m_debuggerManager.Debug();
                        }
                        else
                        {
                            m_debuggerManager.Continue(DebuggerManager.enOperate.StepInto);
                        }
                    },
                    () => {

                        if(!IsDocumentExist)
                        {
                            return false;
                        }

                        if (IsWorkflowRunning)
                        {
                            return false;
                        }
                        else if(IsWorkflowDebugging)
                        {
                            if (!IsWorkflowDebuggingPaused)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }

                        
                    }
                    ));
            }
        }



        private RelayCommand _stepOverCommand;

        /// <summary>
        /// Gets the StepOverCommand.
        /// </summary>
        public RelayCommand StepOverCommand
        {
            get
            {
                return _stepOverCommand
                    ?? (_stepOverCommand = new RelayCommand(
                    () =>
                    {
                        if (!IsWorkflowRunningOrDebugging)
                        {
                            SaveAllCommand.Execute(null);//先全部保存

                            m_debuggerManager = new DebuggerManager(this, ViewModelLocator.Instance.Dock.ActiveDocument);
                            m_debuggerManager.SetNextOperate(DebuggerManager.enOperate.StepOver);
                            m_debuggerManager.Debug();
                        }
                        else
                        {
                            m_debuggerManager.Continue(DebuggerManager.enOperate.StepOver);
                        }
                    },
                    () =>
                    {
                        if (!IsDocumentExist)
                        {
                            return false;
                        }

                        if (IsWorkflowRunning)
                        {
                            return false;
                        }
                        else if (IsWorkflowDebugging)
                        {
                            if (!IsWorkflowDebuggingPaused)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }


                    }));
            }
        }


        private RelayCommand _validateWorkflowCommand;

        /// <summary>
        /// Gets the ValidateWorkflowCommand.
        /// </summary>
        public RelayCommand ValidateWorkflowCommand
        {
            get
            {
                return _validateWorkflowCommand
                    ?? (_validateWorkflowCommand = new RelayCommand(
                    () =>
                    {
                        ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance.Flush();
                        Activity workflow = ActivityXamlServices.Load(new StringReader(ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance.Text));
                        workflow.DisplayName = ViewModelLocator.Instance.Project.ProjectName;//让报错信息能报出项目名

                        var result = ActivityValidationServices.Validate(workflow);
                        if (result.Errors.Count == 0)
                        {
                            MessageBox.Show(App.Current.MainWindow, "工作流校验正确", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            foreach(var err in result.Errors)
                            {
                                SharedObject.Instance.Output(SharedObject.enOutputType.Error, err.Message);
                            }
                            
                            MessageBox.Show(App.Current.MainWindow, "工作流校验错误，请检查参数配置", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }));
            }
        }


        private RelayCommand _toggleBreakpointCommand;

        /// <summary>
        /// Gets the ToggleBreakpointCommand.
        /// </summary>
        public RelayCommand ToggleBreakpointCommand
        {
            get
            {
                return _toggleBreakpointCommand
                    ?? (_toggleBreakpointCommand = new RelayCommand(
                    () =>
                    {
                        BreakpointsManager.ToggleBreakpoint(ViewModelLocator.Instance.Dock.ActiveDocument);
                    }));
            }
        }



        private RelayCommand _removeAllBreakpointsCommand;

        /// <summary>
        /// Gets the RemoveAllBreakpointsCommand.
        /// </summary>
        public RelayCommand RemoveAllBreakpointsCommand
        {
            get
            {
                return _removeAllBreakpointsCommand
                    ?? (_removeAllBreakpointsCommand = new RelayCommand(
                    () =>
                    {
                        BreakpointsManager.RemoveAllBreakpoints(ViewModelLocator.Instance.Dock.ActiveDocument);
                    }));
            }
        }














        private RelayCommand _openLogsCommand;

        /// <summary>
        /// Gets the OpenLogsCommand.
        /// </summary>
        public RelayCommand OpenLogsCommand
        {
            get
            {
                return _openLogsCommand
                    ?? (_openLogsCommand = new RelayCommand(
                    () =>
                    {
                        //打开日志所在的目录
                        Common.LocateDirInExplorer(App.LocalRPAStudioDir + @"\Logs");
                    }));
            }
        }




        /// <summary>
        /// The <see cref="IsHighlightElements" /> property's name.
        /// </summary>
        public const string IsHighlightElementsPropertyName = "IsHighlightElements";

        private bool _isHighlightElementsProperty = false;

        /// <summary>
        /// Sets and gets the IsHighlightElements property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsHighlightElements
        {
            get
            {
                return _isHighlightElementsProperty;
            }

            set
            {
                if (_isHighlightElementsProperty == value)
                {
                    return;
                }

                _isHighlightElementsProperty = value;
                RaisePropertyChanged(IsHighlightElementsPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsLogActivities" /> property's name.
        /// </summary>
        public const string IsLogActivitiesPropertyName = "IsLogActivities";

        private bool _isLogActivitiesProperty = true;

        /// <summary>
        /// Sets and gets the IsLogActivities property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsLogActivities
        {
            get
            {
                return _isLogActivitiesProperty;
            }

            set
            {
                if (_isLogActivitiesProperty == value)
                {
                    return;
                }

                _isLogActivitiesProperty = value;
                RaisePropertyChanged(IsLogActivitiesPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsBreakOnExceptions" /> property's name.
        /// </summary>
        public const string IsBreakOnExceptionsPropertyName = "IsBreakOnExceptions";

        private bool _isBreakOnExceptionsProperty = true;

        /// <summary>
        /// Sets and gets the IsBreakOnExceptions property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBreakOnExceptions
        {
            get
            {
                return _isBreakOnExceptionsProperty;
            }

            set
            {
                if (_isBreakOnExceptionsProperty == value)
                {
                    return;
                }

                _isBreakOnExceptionsProperty = value;
                RaisePropertyChanged(IsBreakOnExceptionsPropertyName);
            }
        }



        private RelayCommand _launchUIExplorerCommand;

        /// <summary>
        /// Gets the LaunchUIExplorerCommand.
        /// </summary>
        public RelayCommand LaunchUIExplorerCommand
        {
            get
            {
                return _launchUIExplorerCommand
                    ?? (_launchUIExplorerCommand = new RelayCommand(
                    () =>
                    {
                        //运行UI浏览器
                    }));
            }
        }


        private RelayCommand _launchPackageManagerCommand;

        /// <summary>
        /// Gets the LaunchPackageManagerCommand.
        /// </summary>
        public RelayCommand LaunchPackageManagerCommand
        {
            get
            {
                return _launchPackageManagerCommand
                    ?? (_launchPackageManagerCommand = new RelayCommand(
                    () =>
                    {
                        //运行包管理器（每次都重新打开，不隐藏）
                        var window = new PackageManagerWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        //var vm = window.DataContext as PackageManagerViewModel;
                        
                        window.ShowDialog();
                    }));
            }
        }



        private RelayCommand _publishCommand;

        /// <summary>
        /// Gets the PublishCommand.
        /// </summary>
        public RelayCommand PublishCommand
        {
            get
            {
                return _publishCommand
                    ?? (_publishCommand = new RelayCommand(
                    () =>
                    {
                        //打包发布
                        //弹窗，让用户选择打包输出到的路径(记忆之前的路径列表)
                        SaveAllCommand.Execute(null);//先全部保存

                        var window = new PublishProjectWindow();
                        window.Owner = Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        var vm = window.DataContext as PublishProjectViewModel;
                        window.ShowDialog();

                    }));
            }
        }







        
    }

    
}
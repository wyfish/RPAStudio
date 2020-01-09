using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using Newtonsoft.Json.Linq;
using Plugins.Shared.Library;
using RPAStudio.DataManager;
using RPAStudio.Executor;
using RPAStudio.ExpressionEditor;
using RPAStudio.Librarys;
using RPAStudio.Localization;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DocumentViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WorkflowDesigner WorkflowDesignerInstance { get; set; }

        public bool IsAlwaysReadOnly { get; set; }//记录是否一直保持只读状态，比如代码片断文件

        public string ActivityBuilderDisplayName = "";

        /// <summary>
        /// The <see cref="WorkflowDesignerView" /> property's name.
        /// </summary>
        public const string WorkflowDesignerViewPropertyName = "WorkflowDesignerView";

        private object _workflowDesignerViewProperty = null;

        /// <summary>
        /// Sets and gets the WorkflowDesignerView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public object WorkflowDesignerView
        {
            get
            {
                return _workflowDesignerViewProperty;
            }

            set
            {
                if (_workflowDesignerViewProperty == value)
                {
                    return;
                }

                _workflowDesignerViewProperty = value;
                RaisePropertyChanged(WorkflowDesignerViewPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsDebugging" /> property's name.
        /// </summary>
        public const string IsDebuggingPropertyName = "IsDebugging";

        private bool _isDebuggingProperty = false;

        /// <summary>
        /// Sets and gets the IsDebugging property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDebugging
        {
            get
            {
                return _isDebuggingProperty;
            }

            set
            {
                if (_isDebuggingProperty == value)
                {
                    return;
                }

                _isDebuggingProperty = value;
                RaisePropertyChanged(IsDebuggingPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsReadOnly" /> property's name.
        /// </summary>
        public const string IsReadOnlyPropertyName = "IsReadOnly";

        private bool _isReadOnlyProperty = false;

        /// <summary>
        /// Sets and gets the IsReadOnly property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnlyProperty;
            }

            set
            {
                if (IsAlwaysReadOnly && !value)
                {
                    return;
                }


                _isReadOnlyProperty = value;
                RaisePropertyChanged(IsReadOnlyPropertyName);

                WorkflowDesignerInstance.Context.Items.GetValue<ReadOnlyState>().IsReadOnly = value;
                WorkflowDesignerInstance.Context.Services.GetService<DesignerView>().IsReadOnly = value;

                if (value)
                {
                    CompositeTitle = Title + " (只读)";
                }
                else
                {
                    CompositeTitle = Title;
                }
            }
        }

        /// <summary>
        /// The <see cref="IsDirty" /> property's name.
        /// </summary>
        public const string IsDirtyPropertyName = "IsDirty";

        private bool _isDirtyProperty = false;

        /// <summary>
        /// Sets and gets the IsDirty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirtyProperty;
            }

            set
            {
                _isDirtyProperty = value;
                RaisePropertyChanged(IsDirtyPropertyName);

                if(value)
                {
                    CompositeTitle = Title + " *";
                }
                else
                {
                    CompositeTitle = Title;
                }
            }
        }


        public void UpdateCompositeTitle()
        {
            IsReadOnly = IsReadOnly;
            IsDirty = IsDirty;
        }


        /// <summary>
        /// The <see cref="CompositeTitle" /> property's name.
        /// </summary>
        public const string CompositeTitlePropertyName = "CompositeTitle";

        private string _compositeTitleProperty = "";

        /// <summary>
        /// Sets and gets the CompositeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CompositeTitle
        {
            get
            {
                return _compositeTitleProperty;
            }

            set
            {
                if (_compositeTitleProperty == value)
                {
                    return;
                }

                _compositeTitleProperty = value;
                RaisePropertyChanged(CompositeTitlePropertyName);
            }
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
        /// The <see cref="XamlPath" /> property's name.
        /// </summary>
        public const string XamlPathPropertyName = "XamlPath";

        private string _xamlPathProperty = "";

        /// <summary>
        /// Sets and gets the XamlPath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string XamlPath
        {
            get
            {
                return _xamlPathProperty;
            }

            set
            {
                if (_xamlPathProperty == value)
                {
                    return;
                }

                _xamlPathProperty = value;
                RaisePropertyChanged(XamlPathPropertyName);

                //设置RelativeXamlPath
                RelativeXamlPath = Common.MakeRelativePath(SharedObject.Instance.ProjectPath, value);
            }
        }


        /// <summary>
        /// 文件相对于项目的相对路径
        /// </summary>
        public const string RelativeXamlPathPropertyName = "RelativeXamlPath";

        private string _relativeXamlPathProperty = "";

        /// <summary>
        /// Sets and gets the RelativeXamlPath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string RelativeXamlPath
        {
            get
            {
                return _relativeXamlPathProperty;
            }

            set
            {
                if (_relativeXamlPathProperty == value)
                {
                    return;
                }

                _relativeXamlPathProperty = value;
                RaisePropertyChanged(RelativeXamlPathPropertyName);
            }
        }



        public bool DoCloseDocument()
        {
            if (IsDebugging)
            {
                var ret = MessageBox.Show(App.Current.MainWindow, string.Format("当前文档正在被调试，确定终止调试并关闭\"{0}\"吗？", XamlPath), ResxIF.GetString("ConfirmText"), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (ret == MessageBoxResult.Yes)
                {
                    ViewModelLocator.Instance.Main.StopWorkflowCommand.Execute(null);
                }
                else if (ret == MessageBoxResult.No)
                {
                    return false;
                }
            }

            //当前文档窗口关闭
            bool isClose = true;
            if (IsDirty)
            {
                //var ret = MessageBox.Show(App.Current.MainWindow, string.Format("文件有修改，需要保存文件\"{0}\"吗？", XamlPath), "询问", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                var ret = MessageBox.Show(App.Current.MainWindow, string.Format(ResxIF.GetString("Message_SaveConfirm"), XamlPath), ResxIF.GetString("ConfirmText"), MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

                if (ret == MessageBoxResult.Yes)
                {
                    SaveDocument();
                }
                else if (ret == MessageBoxResult.No)
                {

                }
                else
                {
                    isClose = false;
                }
            }

            if (isClose)
            {
                Messenger.Default.Send(this, "Close");
                Messenger.Default.Unregister(this);//取消注册
            }

            return isClose;
        }



        private RelayCommand _closeCommand;

        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(
                    () =>
                    {
                        DoCloseDocument();
                    },
                    () => true));
            }
        }


        /// <summary>
        /// The <see cref="ContentId" /> property's name.
        /// </summary>
        public const string ContentIdPropertyName = "ContentId";

        private string _contentIdProperty = System.Guid.NewGuid().ToString();

        /// <summary>
        /// Sets and gets the ContentId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ContentId
        {
            get
            {
                return _contentIdProperty;
            }

            set
            {
                if (_contentIdProperty == value)
                {
                    return;
                }

                _contentIdProperty = value;
                RaisePropertyChanged(ContentIdPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelectedProperty = false;

        /// <summary>
        /// 当前文档是否是用户正在操作的
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelectedProperty;
            }

            set
            {
                if (_isSelectedProperty == value)
                {
                    return;
                }

                _isSelectedProperty = value;
                RaisePropertyChanged(IsSelectedPropertyName);

                if (value)
                {
                    //当前文档窗口激活
                    Messenger.Default.Send(this, "IsSelected");
                }
            }
        }

       

        public void SaveDocument()
        {
            //保存xaml到文件中
            if(!IsReadOnly)
            {
                WorkflowDesignerInstance.Flush();
                var xamlText = WorkflowDesignerInstance.Text;
                File.WriteAllText(XamlPath, xamlText);
                IsDirty = false;
            }
        }

        //private void ParseCopyDataProc(string copyData)
        //{
        //    var modelService = this.WorkflowDesignerInstance.Context.Services.GetService<ModelService>();
        //    var modelItem = modelService.Find(modelService.Root, typeof(Activity));

        //    JsonData jsonData = JsonMapper.ToObject(copyData);
        //    if (Equals(jsonData["cmd"].ToString(), "grab"))
        //    {
        //        foreach (ModelItem item in modelItem)
        //        {
        //            if (Equals(item.ItemType.Name, jsonData["className"].ToString()))
        //            {
        //                List<ModelProperty> PropertyList = item.Properties.ToList();
        //                ModelProperty propertyInfo = PropertyList.Find((ModelProperty property) => property.Name.Equals("guid"));
        //                if (Equals(propertyInfo.ComputedValue, jsonData["classID"].ToString()))
        //                {
        //                    ModelProperty imgProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("SourceImgPath"));

        //                    var imgFilePath = jsonData["savePath"].ToString();
        //                    if (imgFilePath.StartsWith(SharedObject.Instance.ProjectPath+@"\.screenshots\", System.StringComparison.CurrentCultureIgnoreCase))
        //                    {
        //                        //如果在项目目录下，则使用相对路径保存
        //                        imgFilePath = Common.MakeRelativePath(SharedObject.Instance.ProjectPath + @"\.screenshots\", imgFilePath);
        //                    }

        //                    if (imgProperty != null) imgProperty.SetValue(imgFilePath);


        //                    ModelProperty visiProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("visibility"));
        //                    if (visiProperty != null) visiProperty.SetValue(System.Windows.Visibility.Visible);
        //                    ModelProperty offsetXProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("offsetX"));
        //                    if (offsetXProperty != null)
        //                    {
        //                        InArgument<Int32> offsetX = Convert.ToInt32(jsonData["offsetX"].ToString());
        //                        offsetXProperty.SetValue(offsetX);
        //                    }
        //                    ModelProperty offsetYProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("offsetY"));
        //                    if (offsetYProperty != null)
        //                    {
        //                        InArgument<Int32> offsetY = Convert.ToInt32(jsonData["offsetY"].ToString());
        //                        offsetYProperty.SetValue(offsetY);
        //                    }
                            
        //                    //暂定组织的SELECTOR字段
        //                    if (jsonData["className"].ToString().Equals("WindowClose") || jsonData["className"].ToString().Equals("WindowAttach"))
        //                    {
        //                        Int32 offsetX = Convert.ToInt32(jsonData["offsetX"].ToString());
        //                        Int32 offsetY = Convert.ToInt32(jsonData["offsetY"].ToString());

        //                        int hwnd = WindowFromPoint(offsetX, offsetY);
        //                        StringBuilder windowText = new StringBuilder(256);
        //                        GetWindowText(hwnd, windowText, 256);
        //                        StringBuilder className = new StringBuilder(256);
        //                        GetClassName(hwnd, className, 256);

        //                        System.Diagnostics.Debug.WriteLine("formHandle 窗口句柄 : " + hwnd);
        //                        System.Diagnostics.Debug.WriteLine("windowText 窗口的标题: " + windowText);
        //                        System.Diagnostics.Debug.WriteLine("className: " + className);

        //                        ModelProperty SelectorProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("Selector"));
        //                        InArgument<string> Selector = "<" + "Handle:" + hwnd
        //                            + " WindowText:" + windowText + " ClassName:" + className + ">";
        //                        SelectorProperty.SetValue(Selector);
        //                        ModelProperty hwndProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("hwnd"));
        //                        hwndProperty.SetValue(hwnd);
        //                    }

        //                    ModelProperty targetHwndProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("targetHwnd"));
        //                    if (targetHwndProperty != null)
        //                    {
        //                        int targetHwnd = Convert.ToInt32(jsonData["targetHwnd"].ToString());
        //                        targetHwndProperty.SetValue(targetHwnd);
        //                    }
        //                    ModelProperty displayProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("DisplayName"));
        //                    if (displayProperty != null)
        //                    {
        //                        displayProperty.SetValue(jsonData["DisplayName"].ToString());
        //                    }
        //                    ModelProperty processPathProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("ProcessPath"));
        //                    if (processPathProperty != null)
        //                    {
        //                        int targetHwnd = Convert.ToInt32(jsonData["targetHwnd"].ToString());
        //                        uint psID;
        //                        char[] buf = new char[65535];
        //                        Win32Api.GetWindowThreadProcessId((IntPtr)targetHwnd, out psID);
        //                        UInt32 len = 65535;
        //                        int calcProcess;
        //                        calcProcess = Win32Api.OpenProcess(Win32Api.PROCESS_QUERY_INFORMATION, false,(int) psID);
        //                        Win32Api.QueryFullProcessImageName((IntPtr)calcProcess, 0, buf, ref len);
        //                        string exeName = new string(buf, 0, (int)len);
        //                        InArgument<string> _value = exeName;
        //                        processPathProperty.SetValue(_value);
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    else if(Equals(jsonData["cmd"].ToString(), "grabErro"))
        //    {
        //        Application.Current.MainWindow.WindowState = WindowState.Normal;
        //       // SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "UI Element Erro!");
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the DocumentViewModel class.
        /// </summary>
        public DocumentViewModel(string title,string xamlPath,bool isReadOnly = false,bool isAlwaysReadOnly = false)
        {
            ActivityBuilderDisplayName = title;
            Title = title;
            XamlPath = xamlPath;
            
            initWorkflowDesigner();

            IsReadOnly = isReadOnly;

            IsAlwaysReadOnly = isAlwaysReadOnly;

            Messenger.Default.Register<MessengerObjects.CopyData>(this, OnCopyData);

            Messenger.Default.Register<RenameViewModel>(this, "Rename", Rename);

            Messenger.Default.Register<ProjectTreeItem>(this, "Delete", Delete);
        }

        private void OnCopyData(MessengerObjects.CopyData obj)
        {
            //Console.WriteLine(obj.data);
            //string json = Base64.DecodeBase64("utf-8", obj.data);

            //Common.RunInUI(()=> {
            //    ParseCopyDataProc(json);
            //});
        }

        private void Delete(ProjectTreeItem obj)
        {
            //有文件被删除，检查下当前文档对应的xamlPath是否还存在，不存在的话强制关闭即可
            if(!File.Exists(XamlPath))
            {
                Messenger.Default.Send(this, "Close");
                Messenger.Default.Unregister(this);//取消注册
            }
        }

        private void Rename(RenameViewModel obj)
        {
            if (obj.IsDirectory)
            {
                if(XamlPath.ContainsIgnoreCase(obj.Path+@"\"))
                {
                    XamlPath = XamlPath.Replace(obj.Path + @"\", obj.NewPath + @"\");
                    UpdateCompositeTitle();
                }
            }
            else
            {
                if (obj.Path.EqualsIgnoreCase(XamlPath))
                {
                    Title = Path.GetFileNameWithoutExtension(obj.NewPath);
                    XamlPath = obj.NewPath;

                    UpdateCompositeTitle();
                }
            }
        }

        private void initWorkflowDesigner()
        {
            if (_workflowDesignerViewProperty == null)
            {
                WorkflowDesignerInstance = new WorkflowDesigner();

                initExpressionEditor();
               
                var designerConfigurationService = WorkflowDesignerInstance.Context.Services.GetService<DesignerConfigurationService>();

                designerConfigurationService.TargetFrameworkName = new System.Runtime.Versioning.FrameworkName(".NETFramework", new Version(4, 5));

                //enable AutoSurroundWithSequence
                designerConfigurationService.AutoSurroundWithSequenceEnabled = true;

                // enable annotations 
                designerConfigurationService.AnnotationEnabled = true;

                WorkflowDesignerInstance.Load(XamlPath);

                ((FrameworkElement)WorkflowDesignerInstance.View).Loaded += (s, e) =>
                {
                    var designerView = WorkflowDesignerInstance.Context.Services.GetService<DesignerView>();

                    designerView.WorkflowShellBarItemVisibility =
                        ShellBarItemVisibility.All;

                    var modelService = WorkflowDesignerInstance.Context.Services.GetService<ModelService>();
                    modelService.ModelChanged += ModelService_ModelChanged;

                    initBreakpointsInfo();
                };


                _workflowDesignerViewProperty = WorkflowDesignerInstance.View;
            }
        }

       
        private void initBreakpointsInfo()
        {
            //TODO WJF 根据保存的断点位置信息自动设置断点,无效的断点需要删除
            var breakpointsDict = ProjectSettingsDataManager.Instance.m_projectBreakpointsDataManager.m_breakpointsDict;
            if(breakpointsDict.Count>0)
            {
                if(breakpointsDict.ContainsKey(RelativeXamlPath))
                {
                    JArray jarr = (JArray)breakpointsDict[RelativeXamlPath].DeepClone();

                    foreach (JToken ji in jarr)
                    {
                        var activityId= ((JObject)ji)["ActivityId"].ToString();
                        var IsEnabled = (bool)(((JObject)ji)["IsEnabled"]);

                        BreakpointsManager.SetBreakpoint(this, activityId, IsEnabled);
                    }
                }
            }
            
        }

        private void initExpressionEditor()
        {
            WorkflowDesignerInstance.Context.Services.Publish<IExpressionEditorService>(new RoslynExpressionEditorService());
        }


        /// <summary>
        /// TODO WJF遍历所有的Activities，然后执行相应的操作，目前暂未使用
        /// </summary>
        /// <param name="rootItem"></param>
        /// <param name="action"></param>
        private void ProcessActivities(ModelItem rootItem, Action<ModelItem> action)
        {
            if (rootItem == null)
            {
                return;
            }

            action(rootItem);

            foreach (var modelProperty in rootItem.Properties)
            {
                if (typeof(Activity).IsAssignableFrom(modelProperty.PropertyType) ||
                  typeof(FlowNode).IsAssignableFrom(modelProperty.PropertyType))
                {
                    ProcessActivities(modelProperty.Value, action);
                }
                else if (modelProperty.PropertyType.IsGenericType &&
                  modelProperty.PropertyType.GetGenericTypeDefinition() == typeof(Collection<>) &&
                  modelProperty.Collection != null)
                {
                    foreach (var activityModel in modelProperty.Collection)
                    {
                        ProcessActivities(activityModel, action);
                    }
                }
            }
        }


        private void ModelService_ModelChanged(object sender, ModelChangedEventArgs e)
        {
            IsDirty = true;
            try
            {
                if (e.ModelChangeInfo!= null  && e.ModelChangeInfo.Value != null && e.ModelChangeInfo.OldValue == null && e.ModelChangeInfo.ModelChangeType != ModelChangeType.CollectionItemRemoved)
                {
                    //新增activity
                    var currentItem = e.ModelChangeInfo.Value;
                    if(currentItem.ItemType.Name == "FlowStep")
                    {
                        //flowchart设计器拖动时要特殊处理
                        currentItem = e.ModelChangeInfo.Value.Content.Value;
                    }
                    else
                    {
                       
                    }

                    //var assemblyQualifiedName = currentItem.ItemType.AssemblyQualifiedName;//Switch<T>的类型需要特殊判断
                    var assemblyQualifiedName = currentItem.ItemType.Namespace + "." + currentItem.ItemType.Name + "," + currentItem.ItemType.Assembly;
                    assemblyQualifiedName = assemblyQualifiedName.Replace(" ", "");

                    if (!ViewModelLocator.Instance.Activities.ActivityTreeItemAssemblyQualifiedNameDic.ContainsKey(assemblyQualifiedName))
                    {
                        assemblyQualifiedName = currentItem.ItemType.AssemblyQualifiedName;
                        assemblyQualifiedName = assemblyQualifiedName.Replace(" ", "");
                    }

                    if (ViewModelLocator.Instance.Activities.ActivityTreeItemAssemblyQualifiedNameDic.ContainsKey(assemblyQualifiedName))
                    {
                        var item = ViewModelLocator.Instance.Activities.ActivityTreeItemAssemblyQualifiedNameDic[assemblyQualifiedName];
                        Messenger.Default.Send(item, "AddToRecent");

                        if(currentItem.View == null && !item.IsSystem)
                        {
                            currentItem.Properties["DisplayName"].SetValue(item.Name);
                        }
                        
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Warn(err, logger);
            }

            System.GC.Collect();//提醒系统回收内存，避免内存占用过高
        }
    }
}
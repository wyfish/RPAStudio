using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Activities.Presentation;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System;
using RPAStudio.Executor;
using RPAStudio.UserControls;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
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
    public class DockViewModel : ViewModelBase
    {
        private DockContent m_view { get; set; }

        private LayoutAnchorable m_layoutAnchorable { get; set; }

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
                        m_view = (DockContent)p.Source;
                    }));
            }
        }


        /// <summary>
        /// Initializes a new instance of the DockViewModel class.
        /// </summary>
        public DockViewModel()
        {
            Messenger.Default.Register<DocumentViewModel>(this, "IsSelected", (doc) => {
                WorkflowPropertyView = doc.WorkflowDesignerInstance.PropertyInspectorView;
                WorkflowOutlineView = doc.WorkflowDesignerInstance.OutlineView;

                ViewModelLocator.Instance.SourceCode.Connect(doc);
            });


            Messenger.Default.Register<DocumentViewModel>(this, "Close", (doc) =>
            {
                Documents.Remove(doc);

                if(Documents.Count == 0)
                {
                    //文档全关闭时，设置大纲视图为空，活动文档为空
                    WorkflowOutlineView = null;
                    ActiveDocument = null;

                    ViewModelLocator.Instance.SourceCode.Connect(null);
                }

                Messenger.Default.Send(this, "DocumentsCountChanged");
            });

            Messenger.Default.Register<DebuggerManager>(this, "BeginRun", BeginRun);
            Messenger.Default.Register<DebuggerManager>(this, "EndRun", EndRun);
        }

        
        private void BeginRun(DebuggerManager obj)
        {
            m_layoutAnchorable = new LayoutAnchorable();
            m_layoutAnchorable.CanClose = false;
            m_layoutAnchorable.CanHide = false;
            m_layoutAnchorable.Title = ResxIF.GetString("VariableTracking"); // 变量跟踪
            m_layoutAnchorable.IsActive = true;
            m_layoutAnchorable.Content = new LocalsContent();
            m_view.m_leftLayoutAnchorablePane.Children.Add(m_layoutAnchorable);
            
        }

        private void EndRun(DebuggerManager obj)
        {
            Common.RunInUI(()=> {
                m_view.m_leftLayoutAnchorablePane.Children.Remove(m_layoutAnchorable);
                m_layoutAnchorable = null;
            });
        }


        /// <summary>
        /// The <see cref="Documents" /> property's name.
        /// </summary>
        public const string DocumentsPropertyName = "Documents";

        private ObservableCollection<DocumentViewModel> _documentsProperty = new ObservableCollection<DocumentViewModel>();

        /// <summary>
        /// Sets and gets the Documents property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<DocumentViewModel> Documents
        {
            get
            {
                return _documentsProperty;
            }

            set
            {
                if (_documentsProperty == value)
                {
                    return;
                }

                _documentsProperty = value;
                RaisePropertyChanged(DocumentsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="ActiveDocument" /> property's name.
        /// </summary>
        public const string ActiveDocumentPropertyName = "ActiveDocument";

        private DocumentViewModel _activeDocumentProperty = null;

        /// <summary>
        /// Sets and gets the ActiveDocument property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DocumentViewModel ActiveDocument
        {
            get
            {
                return _activeDocumentProperty;
            }

            set
            {
                if (_activeDocumentProperty == value)
                {
                    return;
                }

                _activeDocumentProperty = value;
                RaisePropertyChanged(ActiveDocumentPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="WorkflowPropertyView" /> property's name.
        /// </summary>
        public const string WorkflowPropertyViewPropertyName = "WorkflowPropertyView";

        private object _workflowPropertyViewProperty = null;

        /// <summary>
        /// Sets and gets the WorkflowPropertyView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public object WorkflowPropertyView
        {
            get
            {
                return _workflowPropertyViewProperty;
            }

            set
            {
                if (_workflowPropertyViewProperty == value)
                {
                    return;
                }

                _workflowPropertyViewProperty = value;
                RaisePropertyChanged(WorkflowPropertyViewPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="WorkflowOutlineView" /> property's name.
        /// </summary>
        public const string WorkflowOutlineViewPropertyName = "WorkflowOutlineView";

        private object _workflowOutlineViewProperty = null;

        /// <summary>
        /// Sets and gets the WorkflowOutlineView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public object WorkflowOutlineView
        {
            get
            {
                return _workflowOutlineViewProperty;
            }

            set
            {
                if (_workflowOutlineViewProperty == value)
                {
                    return;
                }

                _workflowOutlineViewProperty = value;
                RaisePropertyChanged(WorkflowOutlineViewPropertyName);
            }
        }

        /// <summary>
        /// 创建新的Document
        /// </summary>
        /// <param name="title">文档标题</param>
        /// <param name="xamlPath">文档xaml路径</param>
        /// <param name="isReadOnly">是否只读（程序运行完毕后会自动恢复为可编辑）</param>
        /// <param name="isAlwaysReadOnly">是否永远只读（代码片断文件打开时须设置该属性）</param>
        public void NewProcessDocument(string title= "未命名文档",string xamlPath="",bool isReadOnly = false, bool isAlwaysReadOnly = false)
        {
            var doc = new DocumentViewModel(title, xamlPath,isReadOnly,isAlwaysReadOnly);
            ViewModelLocator.Instance.Dock.Documents.Add(doc);
            ViewModelLocator.Instance.Dock.ActiveDocument = doc;
            Messenger.Default.Send(this, "DocumentsCountChanged");
        }
    }
}
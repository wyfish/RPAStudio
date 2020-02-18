using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace RPAStudio.ViewModel
{
    /// <summary>
    /// 源代码编辑器窗口视图模型
    /// </summary>
    public class SourceCodeViewModel : ViewModelBase
    {
        /// <summary>
        /// 对应的文档视图模型
        /// </summary>
        private DocumentViewModel _doc;

        /// <summary>
        /// 文本编辑器控件
        /// </summary>
        public TextEditor m_editor { get; set; }

        private RelayCommand<RoutedEventArgs> _loadedCommand;

        /// <summary>
        /// 窗体加载完成
        /// </summary>
        public RelayCommand<RoutedEventArgs> LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        m_editor = (TextEditor)p.Source;
                    }));
            }
        }

        /// <summary>
        /// Initializes a new instance of the SourceCodeViewModel class.
        /// </summary>
        public SourceCodeViewModel()
        {
        }

        /// <summary>
        /// 连接指定的文档对象模型
        /// </summary>
        /// <param name="doc">文档对象模型</param>
        public void Connect(DocumentViewModel doc)
        {
            _doc = doc;

            if(_doc != null)
            {
                _doc.WorkflowDesignerInstance.Flush();
                var xamlText = _doc.WorkflowDesignerInstance.Text;

                m_editor.Text = xamlText;
            }
            else
            {
                m_editor.Text = "";
            }
            
        }


        private RelayCommand _revertCommand;

        /// <summary>
        /// 丢弃
        /// </summary>
        public RelayCommand RevertCommand
        {
            get
            {
                return _revertCommand
                    ?? (_revertCommand = new RelayCommand(
                    () =>
                    {
                        Connect(_doc);
                    }));
            }
        }



        private RelayCommand _commitCommand;

        /// <summary>
        /// 提交
        /// </summary>
        public RelayCommand CommitCommand
        {
            get
            {
                return _commitCommand
                    ?? (_commitCommand = new RelayCommand(
                    () =>
                    {
                        if (_doc != null)
                        {
                            _doc.CommitWorkflowDesigner(m_editor.Text);
                        }
                    }));
            }
        }



        



    }
}
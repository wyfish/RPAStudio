using System.Activities;
using System.ComponentModel;
using System.IO;
using System;
using Word = Microsoft.Office.Interop.Word;
using Plugins.Shared.Library;
using System.Activities.Statements;
using System.Windows;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(WordCreateDesigner))]
    public sealed class WordCreate : NativeActivity
    {
        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }


        public WordCreate()
        {
            Body = new ActivityAction<object>
            {
                Argument = new DelegateInArgument<object>(GetWordAppTag),
                Handler = new Sequence()
                {
                    DisplayName = "Do"
                }
            };
        }

        [Browsable(false)]
        public static string GetWordAppTag { get { return "WordCreate"; } }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/create.png"; } }

        InArgument<string> _PathUrl;
        [Category("打开/新建文档选项")]
        [DisplayName("文件路径")]
        [Browsable(true)]
        public InArgument<string> PathUrl
        {
            get
            {
                return _PathUrl;
            }
            set
            {
                _PathUrl = value;
            }
        }


        [Category("打开/新建文档选项")]
        [DisplayName("是否创建新文档")]
        [Browsable(true)]
        public bool NewDoc
        {
            get;set;
        }

        [Category("打开/新建文档选项")]
        [DisplayName("流程是否可见")]
        [Browsable(true)]
        public bool IsVisible
        {
            get;set;
        }



        InArgument<string> _SavePathUrl;
        [Category("保存选项")]
        [DisplayName("文件路径")]
        [Browsable(true)]
        public InArgument<string> SavePathUrl
        {
            get
            {
                return _SavePathUrl;
            }
            set
            {
                _SavePathUrl = value;
            }
        }


        bool _Save = true;
        [Category("保存选项")]
        [DisplayName("保存")]
        [Browsable(true)]
        public bool Save
        {
            get
            {
                return _Save;
            }
            set
            {
                _Save = value;
            }
        }

        bool _IsExit = true;
        [Category("保存选项")]
        [DisplayName("程序是否退出")]
        [Browsable(true)]
        public bool IsExit
        {
            get
            {
                return _IsExit;
            }
            set
            {
                _IsExit = value;
            }
        }

        bool _SaveAs;
        [Category("保存选项")]
        [DisplayName("另存为")]
        [Browsable(true)]
        public bool SaveAs
        {
            get
            {
                return _SaveAs;
            }
            set
            {
                _SaveAs = value;
            }
        }

        [Browsable(false)]
        private bool isPathAvailable(string path)
        {
            if (path == null)
                return false;
            string[] sArray = path.Split('\\');
            string dict = sArray[0] + '\\';
            System.Diagnostics.Debug.WriteLine("dict : " + dict);
            if (!Directory.Exists(dict))
                return false;
            else
                return true;
        }


        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (this.NewDoc == false && this.PathUrl == null)
            {
                metadata.AddValidationError("非创建新文档需要添加有效路径");
            }
        }


        private Word::Application app;
        protected override void Execute(NativeActivityContext context)
        {
            app = new Word::Application();
            try
            {
                string filePath = PathUrl.Get(context);

                Word::Documents docs = app.Documents;
                app.Visible = IsVisible;
                app.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;

                if (NewDoc)
                {
                    docs.Add();
                }
                else
                {
                    if (!File.Exists(PathUrl.Get(context)))
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "文件不存在，请检查路径有效性!");
                        CommonVariable.realaseProcessExit(app);
                    }
                    else
                    {
                        docs.Open(filePath);
                    }
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(app);
            }
            if(Body != null)
                context.ScheduleAction(Body, app, OnCompleted, OnFaulted);
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }
        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            try
            {
                string filePath = SavePathUrl.Get(context);
                if (_Save)
                {
                    if ((!isPathAvailable(filePath)) && (NewDoc))
                    {
                        string messageBoxText = "此文档为新建文件,请输入正确保存路径!";
                        string caption = "提示";
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBox.Show(messageBoxText, caption, button, icon);
                        CommonVariable.realaseProcessExit(app);
                        return;
                    }
                    else if (NewDoc)
                    {
                        app.Documents.Save(true, Word.WdOriginalFormat.wdOriginalDocumentFormat);
                    }
                    else
                    {
                        app.ActiveDocument.SaveAs2(filePath);
                    }
                }
                if (_SaveAs)
                {
                    if (!isPathAvailable(filePath))
                    {
                        string messageBoxText = "另存为应输入正确保存路径!";
                        string caption = "提示";
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBox.Show(messageBoxText, caption, button, icon);
                        CommonVariable.realaseProcessExit(app);
                        return;
                    }
                    else
                    {
                        app.ActiveDocument.SaveAs2(filePath);
                    }
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(app);
            }

            if (IsExit)
                CommonVariable.realaseProcessExit(app);
        }
    }
}

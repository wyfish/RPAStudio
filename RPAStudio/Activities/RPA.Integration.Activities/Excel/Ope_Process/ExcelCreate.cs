using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(ExcelCreateDesigner))]
    public sealed class ExcelCreate : NativeActivity
    {
        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }


        public ExcelCreate()
        {
            Body = new ActivityAction<object>
            {
                Argument = new DelegateInArgument<object>(GetExcelAppTag),
                Handler = new Sequence()
                {
                    DisplayName = "Do"
                }
            };
        }

        [Browsable(false)]
        public static string GetExcelAppTag { get { return "GetMail"; } }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/create.png"; } }


        InArgument<Int32> _DelayAfter = 3000;
        [Category("Common")]
        [Localize.LocalizedDescription("Description15")] //执行活动后的延迟时间(以毫秒为单位)。默认时间为3000毫秒。 //The delay in milliseconds after the activity is performed.  The default time is 3000 milliseconds. //アクティビティが実行された後のミリ秒単位の遅延。 デフォルトの時間は3000ミリ秒です。
        public InArgument<Int32> DelayAfter
        {
            get
            {
                return _DelayAfter;
            }
            set
            {
                _DelayAfter = value;
            }
        }

        InArgument<Int32> _DelayBefore = 0;
        [Category("Common")]
        [Localize.LocalizedDescription("Description16")] //延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为0毫秒。 //The time (in milliseconds) before the deferred activity begins any operation.  The default time is 0 milliseconds. //遅延アクティビティが操作を開始するまでの時間（ミリ秒）。 デフォルトの時間は0ミリ秒です。
        public InArgument<Int32> DelayBefore
        {
            get
            {
                return _DelayBefore;
            }
            set
            {
                _DelayBefore = value;
            }
        }

        InArgument<string> _PathUrl;
        [Localize.LocalizedCategory("Category6")] //新建/打开文档选项 //New/Open Document Options //新規/開くドキュメントオプション
        [Localize.LocalizedDisplayName("DisplayName12")] //文件路径 //File path //ファイルパス
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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


        bool _NewDoc;
        [Localize.LocalizedCategory("Category6")] //新建/打开文档选项 //New/Open Document Options //新規/開くドキュメントオプション
        [Localize.LocalizedDisplayName("DisplayName20")] //是否创建新文档 //Whether to create a new document //新しいドキュメントを作成するかどうか
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool NewDoc
        {
            get
            {
                return _NewDoc;
            }
            set
            {
                _NewDoc = value;
            }
        }

        bool _IsVisible = true;
        [Localize.LocalizedCategory("Category6")] //新建/打开文档选项 //New/Open Document Options //新規/開くドキュメントオプション
        [Localize.LocalizedDisplayName("DisplayName21")] //流程是否可见 //Whether the process is visible //プロセスが見えるかどうか
        [Browsable(true)]
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }


        InArgument<string> _SavePathUrl;
        [Localize.LocalizedCategory("Category7")] //保存选项 //Save option //保存オプション
        [Localize.LocalizedDisplayName("DisplayName12")] //文件路径 //File path //ファイルパス
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

        bool _IsExit = true;
        [Localize.LocalizedCategory("Category7")] //保存选项 //Save option //保存オプション
        [Localize.LocalizedDisplayName("DisplayName22")] //程序是否退出 //Whether the program exits //プログラムが終了するかどうか
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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


        bool _Save = true;
        [Localize.LocalizedCategory("Category7")] //保存选项 //Save option //保存オプション
        [Localize.LocalizedDisplayName("DisplayName23")] //保存 //save //保存する
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

        bool _SaveAs;
        [Localize.LocalizedCategory("Category7")] //保存选项 //Save option //保存オプション
        [Localize.LocalizedDisplayName("DisplayName24")] //另存为 //Save as //名前を付けて保存
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
            if (this.SaveAs == true && this.SavePathUrl == null)
            {
                metadata.AddValidationError("另存为需要添加有效路径");
            }
        }

        private Excel::Application excelApp;
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                Thread.Sleep(DelayBefore.Get(context));
                string filePath = PathUrl.Get(context);

                excelApp = new Excel::Application();
                excelApp.Visible = IsVisible;
                excelApp.DisplayAlerts = false;

                if (_NewDoc == true)
                {
                    excelApp.Workbooks.Add(true);
                }
                else
                {
                    if (!File.Exists(filePath))
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "文件不存在，请检查路径有效性");
                        new CommonVariable().realaseProcessExit(excelApp);
                        return;
                    }
                    else
                    {
                        //可用Open或Add函数打开文件，但对于执行VBA，Add无保存权限
                        excelApp.Workbooks.Open(filePath);
                    }
                }

                context.ScheduleAction(Body, excelApp, OnCompleted, OnFaulted);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL执行过程出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }
        }
        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }
        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Excel::_Workbook book = excelApp.ActiveWorkbook;
            string saveFilePath = SavePathUrl.Get(context);
            if (_Save)
            {
                if ((!isPathAvailable(saveFilePath)) && (_NewDoc))
                {
                    string messageBoxText = "此文档为新建文件,请输入正确保存路径!";
                    string caption = "提示";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }
                else if (!_NewDoc)
                {
                    book.Save();
                }
                else
                {
                    book.SaveAs(saveFilePath);
                }
            }
            if (_SaveAs)
            {
                if (!isPathAvailable(saveFilePath))
                {
                    string messageBoxText = "另存为应输入正确保存路径!";
                    string caption = "提示";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }
                else
                {
                    book.SaveAs(saveFilePath);
                }
            }
            if (IsExit)
                new CommonVariable().realaseProcessExit(excelApp);
            else
                new CommonVariable().realaseProcess(excelApp);

            Thread.Sleep(DelayAfter.Get(context));
        }
    }
}

using RPA.Core.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;


namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(OpenApplicationDesigner))]
    public sealed class OpenApplicationActivity : NativeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Open Application"; } }

        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Common")]
        [Localize.LocalizedDescription("Description7")] //指定在抛出错误之前等待活动运行的时间(以毫秒为单位)。默认值为3000毫秒(3秒)。 //Specifies the amount of time, in milliseconds, to wait for an activity to run before throwing an error.  The default is 3000 milliseconds (3 seconds). //エラーをスローする前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは3000ミリ秒（3秒）です。
        [DisplayNameAttribute("Timeout(milliseconds)")]
        public InArgument<Int32> Timeout { get; set; }

        [Category("Input")]
        [Localize.LocalizedDescription("Description8")] //可以在启动时传递给应用程序的参数。 //Parameters that can be passed to the application at startup. //起動時にアプリケーションに渡すことができるパラメーター。
        public InArgument<string> Arguments { get; set; }

        [Category("Input")]
        [Localize.LocalizedDescription("Description9")] //可以找到要打开的应用程序的可执行文件的完整文件路径。注意:所有字符串都必须放在引号之间。 //You can find the full file path of the executable for the application you want to open.  Note: All strings must be placed between quotes. //開きたいアプリケーションの実行可能ファイルの完全なファイルパスを見つけることができます。 注：すべての文字列は引用符で囲む必要があります。
        [DisplayNameAttribute("FileName")]
        public InArgument<string> ProcessPath { get; set; }

        [Category("Options")]
        [Localize.LocalizedDescription("Description10")] //当前工作目录的路径。这个字段只接受字符串变量。注意:所有字符串变量必须放在引号之间。 //The path to the current working directory.  This field only accepts string variables.  Note: All string variables must be placed between quotes. //現在の作業ディレクトリへのパス。 このフィールドは文字列変数のみを受け入れます。 注：すべての文字列変数は引用符で囲む必要があります。
        public InArgument<string> WorkingDirectory { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (Arguments == null && ProcessPath == null)
            {
                metadata.AddValidationError("FileName和Arguments都为空，至少需要一个不为空。");
            }
        }

        public OpenApplicationActivity()
        {
            Body = new ActivityAction<object>
            {
                Handler = new Sequence()
                {
                    DisplayName = Resources.Do
                }
            };
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/openApplication.png";
            }
        }

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Localize.LocalizedCategory("Category1")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName1")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description3")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility
        {
            get
            {
                return visi;
            }
            set
            {
                visi = value;
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                string _arguments = Arguments.Get(context);
                string _fileName = ProcessPath.Get(context);
                string _workingDirectory = WorkingDirectory.Get(context);
                Int32 _timeout = Timeout.Get(context);
                Process p = new System.Diagnostics.Process();
                if (_arguments != null)
                {
                    p.StartInfo.Arguments = _arguments;
                }
                if (_workingDirectory != null)
                {
                    p.StartInfo.WorkingDirectory = _workingDirectory;
                }
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = _fileName;
                p.Start();
                Thread.Sleep(_timeout);
                context.ScheduleAction(Body, "", OnCompleted, OnFaulted);
            }
            catch (Exception e)
            {
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw new NotImplementedException(e.Message);
                }
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
          //  faultContext.CancelChildren();
           // Cleanup();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //Cleanup();
        }
    }
}

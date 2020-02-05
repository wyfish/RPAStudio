using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Plugins.Shared.Library;

namespace RPA.OpenCV.Activities.Mouse
{
    [Designer(typeof(CVClickActivityDesigner))]
    public sealed class CVClickActivity : CodeActivity
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public CVClickActivity()
        {
            //File.Delete(FlaxCV._FlaxCV_exe);
            using (FileStream fs = new FileStream(FlaxCV._FlaxCV_exe, FileMode.Create))
            {
                fs.Write(Properties.Resources.Flax_CV, 0, Properties.Resources.Flax_CV.Length);
            }
        }

        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "CV Click"; } }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath {
            get {
                return @"pack://application:,,,/RPA.OpenCV.Activities;Component/Resources/Mouse/cvclick.png";
            }
        }

        [Category("Public")] //公共 //Public //一般公開
        [DisplayName("Error execution")] //错误执行 //Error execution //エラー実行
        [Description("Specifies whether automation should continue even if the activity raises an error")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Input")] //输入 //Input //入力
        [Browsable(true)]
        [DisplayName("Window Title")] //窗口标题 //Window title //ウィンドウタイトル
        [Description("Enter the Window Title")] //输入窗口标题 //Enter window title //Windowのタイトルを入力
        public InArgument<string> Title {
            get;
            set;
        }

        [Category("入力")]
        [Browsable(true)]
        [DisplayName("MatchingThreshold")]
        [Description("マッチング閾値 (デフォルト値 = 91. Must 80 <= x <= 100)")]
        public InArgument<int> MatchingThreshold { get; set; } = 91;

        [Category("入力")]
        [Browsable(true)]
        [DisplayName("MatchingInterval")]
        [Description("マッチング間隔[ms] (デフォルト値 = 2000[ms]. Must 600 <= x)")]
        public InArgument<int> MatchingInterval { get; set; } = 2000;

        [Category("入力")]
        [Browsable(true)]
        [DisplayName("Retry")]
        [Description("リトライ回数 (デフォルト値 = 600回)")]
        public InArgument<int> Retry { get; set; } = 600;

        [Category("出力")]
        [Browsable(true)]
        public OutArgument<bool> Result { get; set; }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility {
            get {
                return visi;
            }
            set {
                visi = value;
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            int x = 0, y = 0;
            // Disply Width
            int w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            // Disply Height
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            try
            {
                string title = Title.Get(context);
                if (title != null && title.Length > 0)
                {
                    IntPtr hWnd = IntPtr.Zero;
                    hWnd = Win32Api.FindWindow(null, title);
                    RECT rect;
                    bool flag = GetWindowRect(hWnd, out rect);
                    x = rect.left;
                    y = rect.top;
                    w = rect.right - rect.left;
                    h = rect.bottom - rect.top;
                }
                var cv = new FlaxCV();
                int matchingThreshold = MatchingThreshold.Get(context);
                int matchingInterval = MatchingInterval.Get(context);
                int retry = Retry.Get(context);
                var cvRet = cv.DoCVAction(FlaxCV.CvActionType.LClick, SourceImgPath, matchingThreshold, matchingInterval, retry, new System.Drawing.Rectangle(x, y, w, h));
                string resultIfo = string.Format("Capture Area : x={0}, y={1}, w={2}, h={3}\nMatched = {4}\nMatched Level = {5}", x, y, w, h, cvRet.IsMatched, cvRet.MatchedLevel);
                SharedObject.Instance.Output(SharedObject.enOutputType.Information, "Image Matching Result", resultIfo);
                Result.Set(context, cvRet.IsMatched);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Error on Executing CVClickActivity()", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

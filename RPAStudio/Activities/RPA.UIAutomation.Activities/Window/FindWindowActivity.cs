using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(FindWindowDesigner))]
    public sealed class FindWindowActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Window"; } }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/findwindow.png";
            }
        }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName60")] //窗口标题 //Window title //ウィンドウタイトル
        [Localize.LocalizedDescription("Description74")] //输入窗口标题 //Enter window title //Windowのタイトルを入力
        public InArgument<string> Title
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName61")] //窗口类名 //Window class name //Windowのクラス名
        [Localize.LocalizedDescription("Description75")] //输入窗口类名 //Enter window class name //Windowのクラス名を入力
        public InArgument<string> ClassName
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        public OutArgument<IntPtr> Result{ get; set; }

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

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _Title = Title.Get(context);
                string _ClassName = ClassName.Get(context);
                IntPtr _result = IntPtr.Zero;

                _result = Win32Api.FindWindow(null, _Title);
                Result.Set(context,_result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "关闭窗口错误产生", e.Message);
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

using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(CloseApplicationDesigner))]
    public sealed class CloseApplicationActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Close Application"; } }

        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/close.png";
            }
        }

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

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Category("Input")]
        [Localize.LocalizedDescription("Description2")] //进程名称 //Process name //プロセス名
        [DisplayNameAttribute("ProcessName")]
        public InArgument<string> ProcessName { get; set; }

        [Localize.LocalizedCategory("Category1")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName1")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description3")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _ProcessName = ProcessName.Get(context);
                if(_ProcessName.ToUpper().EndsWith(".EXE"))
                {
                    _ProcessName = _ProcessName.Substring(0,_ProcessName.Length-4);
                }

                Process[] ps = Process.GetProcessesByName(_ProcessName);
                foreach(Process item in ps)
                {
                    item.Kill();
                }
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
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

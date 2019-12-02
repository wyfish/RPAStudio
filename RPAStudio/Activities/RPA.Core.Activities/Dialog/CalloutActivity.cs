using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(CalloutActivityDesigner))]
    public sealed class CalloutActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Callout"; } }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/callout.png";
            }
        }
        [Category("Input")]
        public InArgument<Int32> offsetX { get; set; }
        [Category("Input")]
        public InArgument<Int32> offsetY { get; set; }
        [Category("Input")]
        [DisplayName("使用坐标点")]
        public bool usePoint { get; set; }
        [RequiredArgument]
        [Category("Input")]
        [Description("弹出窗口的文本。")]
        public InArgument<string> Content { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("弹出窗口的标题。")]
        public InArgument<string> Title { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

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


        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        [Category("Options")]
        [Description("持续时间，直到弹出窗口自动关闭。默认情况下，弹出窗口不会关闭。")]
        [DisplayNameAttribute("Timer")]
        public InArgument<Int32> Timers { get; set; }

        public static T GetValueOrDefault<T>(ActivityContext context, InArgument<T> source, T defaultValue)
        {
            T result = defaultValue;
            if (source != null && source.Expression != null)
            {
                result = source.Get(context);
            }
            return result;
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var selStr = Selector.Get(context);
                UiElement element = GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                Int32 pointX = 0;
                Int32 pointY = 0;
                if (usePoint)
                {
                    pointX = offsetX.Get(context);
                    pointY = offsetY.Get(context);
                }
                else
                {
                    if (element != null)
                    {
                        pointX = element.GetClickablePoint().X;
                        pointY = element.GetClickablePoint().Y;
                        element.SetForeground();
                    }
                    else
                    {

                    }
                }

                Int32 _timers = Timers.Get(context);
                string _content = Content.Get(context);
                string _title = Title.Get(context);
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    DialogActivity.Windows.CalloutWindow dlg = new DialogActivity.Windows.CalloutWindow(_timers);
                    dlg.setContent(_content);
                    dlg.setTitle(_title);
                    dlg.Title = _title;
                    double _height = dlg.getCanvasHeight();
                    double _width = dlg.getCanvasWidth()/2;
                    dlg.Top = pointY - _height;
                    dlg.Left = pointX - _width;
                    dlg.ShowDialog();
                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

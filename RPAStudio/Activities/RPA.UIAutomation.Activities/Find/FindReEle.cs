using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(FindReEleDesigner))]
    public sealed class FindReEle : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Relative Element"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续,取值为（True或False）")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("UI对象")]
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("UI对象")]
        [OverloadGroup("Element")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("要关闭的窗口。该字段仅接受Window变量")]
        public InArgument<UiElement> Element { get; set; }

        [Category("偏移选项")]
        [Browsable(true)]
        [DisplayName("水平偏移")]
        [Description("指定水平方向的光标偏移距离，负数代表反方向")]
        public InArgument<Int32> OffSetX { get; set; }

        [Category("偏移选项")]
        [Browsable(true)]
        [DisplayName("垂直偏移")]
        [Description("指定垂直方向的光标偏移距离，负数代表反方向")]
        public InArgument<Int32> OffSetY { get; set; }


        public enum CursorPosition
        {
            Center,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private CursorPosition _Position = CursorPosition.Center;
        [Category("偏移选项")]
        [Browsable(true)]
        [DisplayName("位置")]
        public CursorPosition Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }



        [Category("输出")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("相对元素")]
        [Description("您正在寻找的相对ui元素。仅支持UIElement类型变量")]
        public OutArgument<UiElement> FoundElement { get; set; }

        
        [Browsable(false)]
        public string SourceImgPath { get; set; }
       

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/FindReEle.png";
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
        public string ClassName { get { return "FindReEle"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            try
            {
                m_Delegate = new runDelegate(Run);
                var selStr = Selector.Get(context);
                UiElement element = null;
                element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                else
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[EleScope.GetEleScope];
                    element = property.GetValue(context.DataContext) as UiElement;
                }

                UiElement relativeEle = element.FindRelativeElement((int)Position, OffSetX.Get(context), OffSetY.Get(context));
                FoundElement.Set(context, relativeEle);
                //relativeEle.DrawHighlight(System.Drawing.Color.Red, TimeSpan.FromSeconds(2), true);

                return m_Delegate.BeginInvoke(callback, state);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取偏移相对元素失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                else
                {
                    throw e;
                }
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}

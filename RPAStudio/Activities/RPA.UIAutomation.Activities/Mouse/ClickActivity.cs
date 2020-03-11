using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System.Threading;

namespace RPA.UIAutomation.Activities.Mouse
{
    [Designer(typeof(MouseDesigner))]
    public sealed class ClickActivity : CodeActivity
    {
        [Browsable(false)]
        public string _DisplayName { get { return "Click"; } }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName60")] //窗口标题 //Window Title
        [Localize.LocalizedDescription("Description132")] //输入屏幕标题以通过AutomationId或Name识别元素。  *前后是通配符。
        public InArgument<string> WindowTitle { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("AutomationId")]
        [Localize.LocalizedDescription("Description133")] //元素的AutomationId属性
        public InArgument<string> AutomationId { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("Name")]
        [Localize.LocalizedDescription("Description134")] //元素的AutomationId属性
        public InArgument<string> Name { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description3")] //输入UIElement //Enter UIElement //UIElementを入力
        public InArgument<UiElement> Element { get; set; }

        [Localize.LocalizedCategory("Category11")] //UI元素矩阵 //UI element matrix //UI要素のマトリックス
        [Browsable(true)]
        public InArgument<Int32> Left { get; set; }
        [Localize.LocalizedCategory("Category11")] //UI元素矩阵 //UI element matrix //UI要素のマトリックス
        [Browsable(true)]
        public InArgument<Int32> Right { get; set; }
        [Localize.LocalizedCategory("Category11")] //UI元素矩阵 //UI element matrix //UI要素のマトリックス
        [Browsable(true)]
        public InArgument<Int32> Top { get; set; }
        [Localize.LocalizedCategory("Category11")] //UI元素矩阵 //UI element matrix //UI要素のマトリックス
        [Browsable(true)]
        public InArgument<Int32> Bottom { get; set; }

        [Category("Common")]
        [Localize.LocalizedDescription("Description55")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Common")]
        [Localize.LocalizedDescription("Description56")] //执行活动后的延迟时间(以毫秒为单位)。默认时间为300毫秒。 //The delay time, in milliseconds, after the activity is executed. The default time is 300 milliseconds. //アクティビティが実行された後のミリ秒単位の遅延。 デフォルトの時間は300ミリ秒です。
        public InArgument<Int32> DelayAfter { get; set;}

        [Category("Common")]
        [Localize.LocalizedDescription("Description57")] //延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为300毫秒。 //The delay time, in milliseconds, before the deferred the activity is executed. The default time is 300 milliseconds. //遅延アクティビティが操作を開始するまでの時間（ミリ秒）。 デフォルトの時間は300ミリ秒です。
        public InArgument<Int32> DelayBefore { get; set; }
   
        [Category("Input")]
        public Int32 ClickType { get; set; }

        [Category("Input")]
        public Int32 MouseButton { get; set; }

        [Category("Input")]
        public string KeyModifiers { get; set; }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName48")] //使用坐标点 //Use coordinate points //座標点を使用する
        public bool usePoint { get; set; }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName66")] // X Coordinate
        [Localize.LocalizedDescription("Description136")] //座標点を使用するがTrueの場合、マウス操作を行うX座標
        public InArgument<Int32> offsetX { get; set; }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName67")] // Y Coordinate
        [Localize.LocalizedDescription("Description137")] //座標点を使用するがTrueの場合、マウス操作を行うY座標
        public InArgument<Int32> offsetY { get; set; }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName87")] //输入前单击偏移 //Click offset before input //オフセット位置をクリック
        [Localize.LocalizedDescription("Description139")] //通过移动元素中心的偏移量来执行鼠标操作 //要素の中心から移動オフセット分ずらしてマウス操作を行います
        public bool clickOffsetPoint { get; set; } = false;

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName85")] //运动偏移量X //移動オフセット X // Move Offset X
        [Localize.LocalizedDescription("Description140")] //X轴上要从元素中心偏移的像素数 //要素の中心からずらすX軸のピクセル数
        public InArgument<Int32> moveOffsetX { get; set; } = 0;

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName86")] //运动偏移量Y //移動オフセット Y // Move Offset Y
        [Localize.LocalizedDescription("Description141")] //Y轴上要从元素中心偏移的像素数 //要素の中心からずらすY軸のピクセル数
        public InArgument<Int32> moveOffsetY { get; set; } = 0;


        [Browsable(false)]
        public string SourceImgPath{ get;set;}

        [Browsable(false)]
        public string icoPath {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Mouse/mouseclick.png";
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

       
        public void SetMouseLeftClick()
        {
            ClickType = (int)MouseClickType.CLICK_SINGLE;
            MouseButton = (int)MouseButtonType.BTN_LEFT;
        }

       
        public void SetMouseRightClick()
        {
            ClickType = (int)MouseClickType.CLICK_SINGLE;
            MouseButton = (int)MouseButtonType.BTN_RIGHT;
        }

       
        static ClickActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(ClickActivity), "ClickType", new EditorAttribute(typeof(MouseClickTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(ClickActivity), "MouseButton", new EditorAttribute(typeof(MouseButtonTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(ClickActivity), "KeyModifiers", new EditorAttribute(typeof(KeyModifiersEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void Execute(CodeActivityContext context)
        {
            Int32 _delayAfter = Common.GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = Common.GetValueOrDefault(context, this.DelayBefore, 300);
            Thread.Sleep(_delayBefore);
            try
            {
                // Prioritize to use the AutomationId or Name property to get faster.
                var nativeElement = UIAutomationCommon.GetNativeElement(context, WindowTitle, AutomationId, Name);
                if (nativeElement != null)
                {
                    int moveX = 0, moveY = 0;
                    if (clickOffsetPoint)
                    {
                        moveX = moveOffsetX.Get(context);
                        moveY = moveOffsetY.Get(context);
                    }
                    UIAutomationCommon.ClickNativeElement(nativeElement, moveX, moveY);
                    Thread.Sleep(_delayAfter);
                    return;
                }

                var selStr = Selector.Get(context);
                var element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }

                var point = UIAutomationCommon.GetPoint(context, usePoint, offsetX, offsetY, element);
                if (point.X == -1 && point.Y == -1)
                {
                    UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, Localize.LocalizedResources.GetString("msgNoElementFound"));
                    return;
                }

                if (KeyModifiers != null)
                {
                    string[] sArray = KeyModifiers.Split(',');
                    foreach (string i in sArray)
                    {
                        Common.DealKeyBordPress(i);
                    }
                }
                UiElement.MouseMoveTo(point);
                UiElement.MouseAction((Plugins.Shared.Library.UiAutomation.ClickType)ClickType, (Plugins.Shared.Library.UiAutomation.MouseButton)MouseButton);
                if (KeyModifiers != null)
                {
                    string[] sArray = KeyModifiers.Split(',');
                    foreach (string i in sArray)
                    {
                        Common.DealKeyBordRelease(i);
                    }
                }
                Thread.Sleep(_delayAfter);
            }
            catch (Exception e)
            {
                UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, e.Message);
            }
        }
    }
}

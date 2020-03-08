using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Activities.Presentation.PropertyEditing;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using RPA.UIAutomation.Activities.Mouse;
using System.Threading;

namespace RPA.UIAutomation.Activities.Keyboard
{
    [Designer(typeof(HotKeyDesigner))]
    public sealed class HotKeyActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Hot Key"; } }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Category("Common")]
        [Localize.LocalizedDescription("Description55")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Common")]
        [Localize.LocalizedDescription("Description56")] //执行活动后的延迟时间(以毫秒为单位)。默认时间为300毫秒。 //The delay time, in milliseconds, after the activity is executed. The default time is 300 milliseconds. //アクティビティが実行された後のミリ秒単位の遅延。 デフォルトの時間は300ミリ秒です。
        public InArgument<Int32> DelayAfter { get; set; }

        [Category("Common")]
        [Localize.LocalizedDescription("Description57")] //延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为300毫秒。 //The delay time, in milliseconds, before the deferred the activity is executed. The default time is 300 milliseconds. //遅延アクティビティが操作を開始するまでの時間（ミリ秒）。 デフォルトの時間は300ミリ秒です。
        public InArgument<Int32> DelayBefore { get; set; }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/KeyBoard/hotkey.png"; } }

        [Browsable(false)]
        public List<string> KeyTypes
        {
            get
            {
                KeyboardTypes key = new KeyboardTypes();
                return key.getKeyTypes;
            }
            set
            {

            }
        }

        [Localize.LocalizedCategory("Category9")] //按键选项 //Key options //ボタンオプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName47")] //其它按键 //Other buttons //その他のボタン
        public string SelectedKey { get; set; }

        [Localize.LocalizedCategory("Category9")] //按键选项 //Key options //ボタンオプション
        [Browsable(true)]
        [DisplayName("Alt")]
        public bool Alt { get; set; }

        [Localize.LocalizedCategory("Category9")] //按键选项 //Key options //ボタンオプション
        [Browsable(true)]
        [DisplayName("Ctrl")]
        public bool Ctrl { get; set; }

        [Localize.LocalizedCategory("Category9")] //按键选项 //Key options //ボタンオプション
        [Browsable(true)]
        [DisplayName("Shift")]
        public bool Shift { get; set; }

        [Localize.LocalizedCategory("Category9")] //按键选项 //Key options //ボタンオプション
        [Browsable(true)]
        [DisplayName("Win")]
        public bool Win { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public Int32 ClickType { get; set; }
        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public Int32 MouseButton { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public InArgument<Int32> offsetX { get; set; }
        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public InArgument<Int32> offsetY { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        [Localize.LocalizedDisplayName("DisplayName48")] //使用坐标点 //Use coordinate points //座標点を使用する
        public bool usePoint { get; set; }
        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public bool isRunClick { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description3")] //输入UIElement //Enter UIElement //UIElementを入力
        public InArgument<UiElement> Element { get; set; }

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

        static HotKeyActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(HotKeyActivity), "ClickType", new EditorAttribute(typeof(MouseClickTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(HotKeyActivity), "MouseButton", new EditorAttribute(typeof(MouseButtonTypeEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        private void DealBaseKeyBordPress()
        {
            if (Alt)
                Common.DealKeyBordPress("Alt");
            if(Ctrl)
                Common.DealKeyBordPress("Ctrl");
            if(Shift)
                Common.DealKeyBordPress("Shift");
            if(Win)
                Common.DealKeyBordPress("Win");
        }

        private void DealBaseKeyBordRelease()
        {
            if (Alt)
                Common.DealKeyBordRelease("Alt");
            if (Ctrl)
                Common.DealKeyBordRelease("Ctrl");
            if (Shift)
                Common.DealKeyBordRelease("Shift");
            if (Win)
                Common.DealKeyBordRelease("Win");
        }

        protected override void Execute(CodeActivityContext context)
        {
            Int32 _delayAfter = Common.GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = Common.GetValueOrDefault(context, this.DelayBefore, 300);
            try
            {
                Thread.Sleep(_delayBefore);
                var selStr = Selector.Get(context);
                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }

                //Int32 pointX = 0;
                //Int32 pointY = 0;
                //if (usePoint)
                //{
                //    pointX = offsetX.Get(context);
                //    pointY = offsetY.Get(context);
                //}
                //else
                //{
                //    if (element != null)
                //    {
                //        pointX = element.GetClickablePoint().X;
                //        pointY = element.GetClickablePoint().Y;
                //        element.SetForeground();
                //    }
                //}
                var point = UIAutomationCommon.GetPoint(context, usePoint, offsetX, offsetY, element);
                if (point.X == -1 && point.Y == -1)
                {
                    UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, "查找不到元素");
                    return;
                }
                if (isRunClick)
                {
                    UiElement.MouseMoveTo(point);
                    UiElement.MouseAction((Plugins.Shared.Library.UiAutomation.ClickType)ClickType, (Plugins.Shared.Library.UiAutomation.MouseButton)MouseButton);
                }
                DealBaseKeyBordPress();
                if (Common.DealVirtualKeyPress(SelectedKey.ToUpper()))
                {
                    Common.DealVirtualKeyRelease(SelectedKey.ToUpper());
                }
                DealBaseKeyBordRelease();
                Thread.Sleep(_delayAfter);
            }
            catch (Exception e)
            {
                UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, e.Message);
            }
        } 
    }
}

using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Activities.Presentation.PropertyEditing;
using System.Security;
using System.Windows;
using RPA.UIAutomation.Activities.Mouse;
using System.Runtime.InteropServices;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library;
using System.Threading;
using System.Windows.Forms;

namespace RPA.UIAutomation.Activities.Keyboard
{
    [Designer(typeof(SecureTextDesigner))]
    public sealed class SecureTextActivity : CodeActivity
    {
        static SecureTextActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(SecureTextActivity), "ClickType", new EditorAttribute(typeof(MouseClickTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(SecureTextActivity), "MouseButton", new EditorAttribute(typeof(MouseButtonTypeEditor), typeof(PropertyValueEditor)));
            //builder.AddCustomAttributes(typeof(SecureTextActivity), "KeyModifiers", new EditorAttribute(typeof(KeyModifiersEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Secure Text"; } }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description3")] //输入UIElement //Enter UIElement //UIElementを入力
        public InArgument<UiElement> Element { get; set; }

        //  [RequiredArgument]
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
        public string icoPath { get { return "pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/KeyBoard/text.png"; } }

        [Browsable(false)]
        public List<string> KeyTypes
        {
            get
            {
                KeyboardTypes key = new KeyboardTypes();
                return key.getKeyTypes;
            }
        }


        [RequiredArgument]
        [Localize.LocalizedCategory("Category13")] //输入项
        [Browsable(true)]
        [DisplayName("Secure Text")]
        public InArgument<SecureString> SecureText
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        [Localize.LocalizedDisplayName("DisplayName83")] //运行时的鼠标操作 //Mouse operation at runtime //実行時のマウス操作
        [Localize.LocalizedDescription("Description138")] //キーボード入力の前に、指定のマウス操作を行うかどうかを指定します
        public bool isRunClick { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public Int32 ClickType { get; set; }
        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        public Int32 MouseButton { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        [Localize.LocalizedDisplayName("DisplayName48")] //使用坐标点 //Use coordinate points //座標点を使用する
        [Localize.LocalizedDescription("Description135")] //実行時のマウス操作がTrueの場合、要素の座標ではなく入力された座標をクリック後に入力します。
        public bool usePoint { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        [Localize.LocalizedDisplayName("DisplayName66")] // X Coordinate
        [Localize.LocalizedDescription("Description136")] //座標点を使用するがTrueの場合、マウス操作を行うX座標
        public InArgument<Int32> offsetX { get; set; }

        [Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        [Localize.LocalizedDisplayName("DisplayName67")] // Y Coordinate
        [Localize.LocalizedDescription("Description137")] //座標点を使用するがTrueの場合、マウス操作を行うY座標
        public InArgument<Int32> offsetY { get; set; }


        //[Localize.LocalizedCategory("Category10")] //鼠标选项 //Mouse options //マウスオプション
        //public string KeyModifiers { get; set; }

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


        protected override void Execute(CodeActivityContext context)
        {
            Int32 _delayAfter = Common.GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = Common.GetValueOrDefault(context, this.DelayBefore, 300);
            try
            {
                Thread.Sleep(_delayBefore);
                SecureString secureText = SecureText.Get(context);
                IntPtr inP = Marshal.SecureStringToBSTR(secureText);//inP为secureStr的句柄
                string text = Marshal.PtrToStringBSTR(inP);
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
                //    else
                //    {
                //        UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, "查找不到元素");
                //        return;
                //    }
                //}
                var point = UIAutomationCommon.GetPoint(context, usePoint, offsetX, offsetY, element);
                if (point.X == -1 && point.Y == -1)
                {
                    UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, Localize.LocalizedResources.GetString("msgNoElementFound"));
                    return;
                }
                /*执行鼠标点击事件*/
                if (isRunClick)
                {
                    UiElement.MouseMoveTo(point);
                    UiElement.MouseAction((Plugins.Shared.Library.UiAutomation.ClickType)ClickType, (Plugins.Shared.Library.UiAutomation.MouseButton)MouseButton);
                }
                else if(true)
                {

                }
                else
                {
                    UIAutomationCommon.HandleContinueOnError(context, ContinueOnError, "找不到键值");
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "发送安全文本执行过程出错", e.Message);
            }
        }
    }
}

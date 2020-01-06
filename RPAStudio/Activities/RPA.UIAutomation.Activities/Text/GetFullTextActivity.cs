using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(GetFullTextDesigner))]
    public sealed class GetFullTextActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Get Full Text"; } }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }


        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description43")] //要关闭的窗口。该字段仅接受Window变量 //The window to close.  This field only accepts Window variables //閉じるウィンドウ。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<UIElement> ActiveWindow { get; set; }


        [RequiredArgument]
        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName34")] //文本 //Text //テキスト
        [Localize.LocalizedDescription("Description63")] //要单击的文本 //The text to click //クリックするテキスト
        public InArgument<String> Text { get; set; }


        //  [RequiredArgument]
        [Category("Common")]
        [Localize.LocalizedDescription("Description55")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName57")] //忽略隐藏 //Ignore hiding //非表示を無視
        [Localize.LocalizedDescription("Description71")] //如果选中此复选框，则不提取指定ui元素上的字符串信息，默认情况下不选中此复选框 //If this check box is selected, the string information on the specified ui element is not extracted, and this check box is not selected by default. //このチェックボックスがオンの場合、指定されたui要素の文字列情報は抽出されず、このチェックボックスはデフォルトでは選択されません。
        public bool IgnoreHidden { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }
        
        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Text/gettext.png";
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

        protected override void Execute(CodeActivityContext context)
        {
        }
    }
}

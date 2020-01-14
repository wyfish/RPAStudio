using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(GetVisibleTextDesigner))]
    public sealed class GetVisibleTextActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Get Visible Text"; } }

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


        //  [RequiredArgument]
        [Category("Common")]
        [Localize.LocalizedDescription("Description55")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName57")] //忽略隐藏 //Ignore hiding //非表示を無視
        [Localize.LocalizedDescription("Description71")] //如果选中此复选框，则不提取指定ui元素上的字符串信息，默认情况下不选中此复选框 //If this check box is selected, the string information on the specified ui element is not extracted, and this check box is not selected by default. //このチェックボックスがオンの場合、指定されたui要素の文字列情報は抽出されず、このチェックボックスはデフォルトでは選択されません。
        public bool IgnoreHidden { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName58")] //分隔符 //Separator //セパレーター
        [Localize.LocalizedDescription("Description72")] //指定用作字符串先锋的字符。如果字段为空，则使用所有已知的文本分隔符(空格、句号、逗号等) //Specifies the character to use as a string pioneer.  If the field is empty, all known text separators (spaces, periods, commas, etc.) are used //文字列の先駆者として使用する文字を指定します。 フィールドが空の場合、すべての既知のテキスト区切り文字（スペース、ピリオド、コンマなど）が使用されます
        public InArgument<string> Separator { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName59")] //单词信息 //Word information //単語情報
        [Localize.LocalizedDescription("Description73")] //在指定的ui元素中找到的每个单词的屏幕坐标 //Screen coordinates of each word found in the specified ui element //指定されたui要素で見つかった各単語の画面座標
        public InArgument<UIElement> WordsInfo { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName34")] //文本 //Text //テキスト
        [Localize.LocalizedDescription("Description63")] //要单击的文本 //The text to click //クリックするテキスト
        public InArgument<String> Text { get; set; }

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

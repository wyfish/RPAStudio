using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 依存句法分析
    /// </summary>
    [Designer(typeof(BaiDuNLPSyntacticAnalysisDesigner))]
    public sealed class BaiDuNLPSyntacticAnalysisActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "BaiDuNLP"; } }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description93")]//您的APIKey。//Your APIKey. //あなたのAPIKey。
        public InArgument<string> APIKey { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description94")]//您的SecretKey。//Your SecretKey.//あなたのSecretKey。
        public InArgument<string> SecretKey { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description99")]//待分析文本 //Text to be analyzed //分析対象テキスト
        public InArgument<string> Text { get; set; }

        public enum mode
        {
            yes=0,
            no=1
        }
        mode _mode = (int)mode.yes;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Browsable(true)]
        [Localize.LocalizedDescription("Description109")]//模型选择,默认值为yes(web模型)；no为query模型。//Model selection, the default value is yes (web model); no is query model. //モデルの選択は、デフォルト値はyes（webモデル）であり、noはqueryモデルである。
        public mode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description110")]//文本中的依存句法结构信息。//Dependency syntactic structure information in text.//テキストの依存構文情報。
        public OutArgument<string> Result { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/NLP/nlp.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string text = Text.Get(context);
            string apiKey = APIKey.Get(context);
            string seKey = SecretKey.Get(context);
            try
            {
                var client = new Baidu.Aip.Nlp.Nlp(apiKey, seKey);
                //修改超时时间
                client.Timeout = 60000;  
                //设置可选参数
                var options = new Dictionary<string, object>
                {
                    {"mode", Mode}
                };
                //带参数调用依存句法分析
                string result = client.DepParser(text, options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

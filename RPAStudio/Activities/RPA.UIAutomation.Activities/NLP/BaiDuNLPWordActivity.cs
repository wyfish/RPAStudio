using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 词向量表示
    /// </summary>
    [Designer(typeof(BaiDuNLPWordDesigner))]
    public sealed class BaiDuNLPWordActivity : CodeActivity
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
        [Localize.LocalizedDescription("Description95")]//文本内容 //Text content //テキストの内容
        public InArgument<string> Text { get; set; }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description116")]//词向量查询结果表示。//Word vector query result representation. //ワードベクトルの検索結果を表示します。
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
                //调用词向量表示
                string result = client.WordEmbedding(text).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

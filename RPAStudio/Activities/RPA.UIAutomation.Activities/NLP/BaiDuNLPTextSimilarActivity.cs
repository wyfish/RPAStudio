using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 短文本相似度
    /// </summary>
    [Designer(typeof(BaiDuNLPTextSimilarDesigner))]
    public sealed class BaiDuNLPTextSimilarActivity : CodeActivity
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
        [Localize.LocalizedDescription("Description113")]//待比较文本，最大512字节。//Text to be compared, 512 bytes maximum.//テキストの最大512バイトを比較します。
        public InArgument<string> Text1 { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDescription("Description113")]//待比较文本，最大512字节。//Text to be compared, 512 bytes maximum.//テキストの最大512バイトを比較します。
        public InArgument<string> Text2 { get; set; }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description114")]//两个文本的相似度结果。//The similarity results of two texts.//二つのテキストの類似度の結果。
        public OutArgument<string> Result { get; set; }

        public enum Model
        {
            BOW,   
            CNN,
            GRNN
        }
        Model _model = Model.BOW; 
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName74")]//模型 //Model //モデル
        [Localize.LocalizedDescription("Description115")]//默认为BOW。//The default is bow.//デフォルトはBOWです。
        [Browsable(true)]
        public Model model
        {
            get { return _model; }
            set { _model = value; }
        }

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
            string text1 = Text1.Get(context);
            string text2 = Text2.Get(context);
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
                    {"model", model}
                };
                //带参数调用短文本相似度
                string result = client.Simnet(text1, text2,options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

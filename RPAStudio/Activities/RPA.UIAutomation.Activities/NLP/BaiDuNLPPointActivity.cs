using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 评论观点抽取
    /// </summary>
    [Designer(typeof(BaiDuNLPPointDesigner))]
    public sealed class BaiDuNLPPointActivity : CodeActivity
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
        [Localize.LocalizedDescription("Description106")]//评论内容，最大10240字节。//Comment content, up to 10240 bytes. //コメントの内容は、最大10240バイトです。
        public InArgument<string> Text { get; set; }

        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Browsable(true)]
        [Localize.LocalizedDescription("Description107")]//两个文本的相似度结果。//The similarity results of two texts.//二つのテキストの類似度の結果。
        public OutArgument<string> Result { get; set; }

        InArgument<Int32> _type = 4;
        [Localize.LocalizedCategory("Category5")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName73")]//评论行业类型 //Comment industry type //業界のタイプを評論します
        [Localize.LocalizedDescription("Description108")]//1:酒店;2:KTV;3:丽人;4:美食餐饮;5:旅游;6:健康;7:教育;8:商业;9:房产;10:汽车;11:生活;12:购物;13:3C，默认为4（餐饮美食）。//1: Hotel; 2: KTV; 3: beauty; 4: Food and beverage; 5: tourism; 6: health; 7: education; 8: business; 9: real estate; 10: car; 11: life; 12: shopping; 13:3c, default is 4 (food and beverage).//1:ホテル、2:KTV、3:麗人、4:グルメレストラン、5:旅行、6:健康、7:教育、8:ビジネス、9:不動産、10:自動車、11:生活、12:ショッピング、13:3 C、デフォルトは4(飲食グルメ)です。
        [Browsable(true)]
        public InArgument<Int32> type
        {
            get { return _type; }
            set { _type = value; }
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
                    {"type",type}
                };
                //带参数调用评论观点抽取
                string result = client.CommentTag(text,options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}

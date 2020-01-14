using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.Core.Activities.DateTimeActivity
{
    [Designer(typeof(GetDateTimeDesigner))]
    public sealed class GetDateTimeActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Get Current DateTime";
            }
        }

        DateTimeType _dateTimeType;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName38")] //时间类型 //Time type //時間タイプ
        public DateTimeType DateType
        {
            get { return _dateTimeType; }
            set { _dateTimeType = value; }
        }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName37")] //时间 //Time //時間
        public OutArgument<string> Date { get; set; }
       

        protected override void Execute(CodeActivityContext context)
        {
            string strDate = "";
            switch(DateType)
            {
                case DateTimeType.日期时间:
                    strDate = DateTime.Now.ToString();
                    break;
                case DateTimeType.日期:
                    strDate = DateTime.Now.ToShortDateString().ToString();
                    break;
                case DateTimeType.时间:
                    strDate = DateTime.Now.ToLongTimeString().ToString();
                    break;
                case DateTimeType.年:
                    strDate = DateTime.Now.Year.ToString();
                    break;
                case DateTimeType.月:
                    strDate = DateTime.Now.Month.ToString();
                    break;
                case DateTimeType.日:
                    strDate = DateTime.Now.Day.ToString();
                    break;
                case DateTimeType.时:
                    strDate = DateTime.Now.Hour.ToString();
                    break;
                case DateTimeType.分:
                    strDate = DateTime.Now.Minute.ToString();
                    break;
                case DateTimeType.秒:
                    strDate = DateTime.Now.Second.ToString();
                    break;
                default:
                    break;
            }
            Date.Set(context,strDate);
        }
    }
}

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
        [Category("输入")]
        [DisplayName("时间类型")]
        public DateTimeType DateType
        {
            get { return _dateTimeType; }
            set { _dateTimeType = value; }
        }

        [Category("输出")]
        [DisplayName("时间")]
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

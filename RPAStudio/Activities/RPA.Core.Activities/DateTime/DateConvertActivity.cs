using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Globalization;

namespace RPA.Core.Activities.DateTimeActivity
{
    [Designer(typeof(GetDateTimeDesigner))]
    public sealed class DateConvertActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Date Convert";
            }
        }

        public enum ConvertType
        {
            日期时间,
            日期,
            时间,
        }


        [Category("输入")]
        [DisplayName("日期时间")]
        public InArgument<string> SDate { get; set;}

        [Category("输入")]
        [DisplayName("转换类型")]
        public ConvertType convertType { get; set; }

        [Category("输出")]
        [DisplayName("时间")]
        public OutArgument<DateTime> Date { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _SDate = SDate.Get(context);
                DateTime _dateTime = new DateTime();
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                switch (convertType)
                {
                    case ConvertType.日期时间:
                        dtFormat.ShortDatePattern = "yyyy/MM/dd HH:mm:ss";
                        break;
                    case ConvertType.日期:
                        dtFormat.ShortDatePattern = "yyyy/MM/dd";
                        break;
                    case ConvertType.时间:
                        dtFormat.ShortTimePattern = "HH:mm:ss";
                        break;
                    default:
                        break;
                }
                _dateTime = Convert.ToDateTime(_SDate, dtFormat);
                Date.Set(context,_dateTime);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
                throw;
            }
        }
    }
}

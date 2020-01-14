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


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName35")] //日期时间 //Date time //日時
        public InArgument<string> SDate { get; set;}

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName36")] //转换类型 //Conversion type //変換タイプ
        public ConvertType convertType { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName37")] //时间 //Time //時間
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

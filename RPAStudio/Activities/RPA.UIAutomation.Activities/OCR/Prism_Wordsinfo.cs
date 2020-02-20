using System.Collections.Generic;

namespace RPA.UIAutomation.Activities.OCR
{
    /// <summary>
    /// 阿里云OCR识别结果字段word，pos
    /// </summary>
    public class Prism_Wordsinfo
    {
        public string word { get; set; }
        public IList<Pos> pos { get; set; }
    }
}

using System.Collections.Generic;

namespace RPA.UIAutomation.Activities.OCR
{
    /// <summary>
    /// 阿里云OCR识别出结果的字段
    /// </summary>
    public class OcrResult
    {
        public string sid { get; set; }
        public string prism_version { get; set; }
        public int prism_wnum { get; set; }
        public List<Prism_Wordsinfo> prism_wordsInfo { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int orgHeight { get; set; }
        public int orgWidth { get; set; }
    }
}

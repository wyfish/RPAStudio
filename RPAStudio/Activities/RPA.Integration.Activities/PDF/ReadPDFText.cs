using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;

namespace RPA.Integration.Activities.PDF
{
    [Designer(typeof(ReadPDFTextDesigner))]
    public sealed class ReadPDFText : CodeActivity
    {

        public ReadPDFText()
        {
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/PDF/pdf.png";
            }
        }

        InArgument<string> _PathUrl;
        [Localize.LocalizedCategory("key319")] //文件选项
        [Localize.LocalizedDisplayName("key320")] //文件路径
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> PathUrl
        {
            get
            {
                return _PathUrl;
            }
            set
            {
                _PathUrl = value;
            }
        }

        [Localize.LocalizedCategory("key319")] //文件选项
        [Localize.LocalizedDisplayName("key321")] //文件密码
        [Browsable(true)]
        public InArgument<string> PassWord
        {
            get; set;
        }

        public InArgument<string> _Range = "全部";
        [Localize.LocalizedCategory("key322")] //选项
        [Localize.LocalizedDisplayName("key323")] //页面范围
        [Browsable(true)]
        [RequiredArgument]
        public InArgument<string> Range
        {
            get
            {
                return _Range;
            }
            set
            {
                _Range = value;
            }
        }
        

        [Localize.LocalizedCategory("key324")] //输出
        [Localize.LocalizedDisplayName("key325")] //PDF文本
        [Browsable(true)]
        public OutArgument<string> PDFText
        {
            get;set;
        }



        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                PdfReader pdfReader;
                string pathUrl = PathUrl.Get(context);
                string range = Range.Get(context);

                if (!File.Exists(pathUrl))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "PDF文件不存在，请检查路径有效性", pathUrl);
                    return;
                }
                if(PassWord.Expression != null)
                {
                    byte[] byteArray = System.Text.Encoding.Default.GetBytes(PassWord.Get(context));
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pathUrl, byteArray);
                }
                else
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pathUrl);
                }

                string pdfText = null;
                if(range.Contains(","))
                {
                    string[] rangeArray1 = range.Split(',');
                    foreach(string buff in rangeArray1)
                    {
                        string pdfPara = null;
                        if (buff.Contains("-"))
                        {
                            string[] rangeArray = buff.Split('-');
                            string range1 = rangeArray[0];
                            string range2 = rangeArray[1];
                            pdfPara = getContent(pdfReader, range1, range2);
                        }
                        else
                        {
                            pdfPara = getContent(pdfReader, buff);
                        }
                        pdfText += pdfPara;
                    }
                }
                else if (range.Contains("-"))
                {
                    string[] rangeArray = range.Split('-');
                    string range1 = rangeArray[0];
                    string range2 = rangeArray[1];
                    pdfText = getContent(pdfReader, range1, range2);
                }
                else
                {
                    pdfText = getContent(pdfReader, range);
                }
                context.SetValue(PDFText, pdfText);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "PDF读取文本失败", e);
                throw e;
            }
        }

        private string getContent(PdfReader pdfReader, string page)
        {
            string pdfText = null;
            int pageCount = pdfReader.NumberOfPages;
            if(page == "全部")
            {
                for (int pg = 1; pg <= pageCount; pg++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string value = PdfTextExtractor.GetTextFromPage(pdfReader, pg, strategy);
                    pdfText += value;
                }
            }
            else
            {
                for (int pg = 1; pg <= pageCount; pg++)
                {
                    if (Convert.ToInt32(page) == pg)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        string value = PdfTextExtractor.GetTextFromPage(pdfReader, pg, strategy);
                        pdfText = value;
                        break;
                    }
                }
            }
            return pdfText;
        }

        private string getContent(PdfReader pdfReader, string page1, string page2)
        {
            string pdfText = null;
            int pageCount = pdfReader.NumberOfPages;
            if(Convert.ToInt32(page2) > pageCount)
            {
                throw new Exception("PDF结尾页数不可超过最大页数 "+ pageCount);
            }
            int begin = 1, last = pageCount;
            if (page1 == "开始")
                begin = 1;
            else
                begin = Convert.ToInt32(page1);

            if (page2 == "结束")
                last = pageCount;
            else
                last = Convert.ToInt32(page2);
    
            for (int pg = begin; pg <= last; pg++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string value = PdfTextExtractor.GetTextFromPage(pdfReader, pg, strategy);
                pdfText += value;
            }
            return pdfText;
        }
    }
}

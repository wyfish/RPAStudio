using Word = Microsoft.Office.Interop.Word;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;

namespace RPA.Integration.Activities.WordPlugins
{
    public enum WdColorIndexEnum
    {
        自动配色 = 0,
        黑色 = 1,
        蓝色 = 2,
        青绿色 = 3,
        鲜绿色 = 4,
        粉红色 = 5,
        红色 = 6,
        黄色 = 7,
        白色 = 8,
        深蓝色 = 9,
        青色 = 10,
        绿色 = 11,
        紫色 = 12,
        深红色 = 13,
        深黄色 = 14,
        灰度50 = 15,
        灰度25 = 16
    }

    public enum CursorMoveType
    {
        Left,
        Right,
        Up,
        Down
    }

    public class CommonVariable
    {
        //public static Word::Application app;
        //public static Word::Documents docs;
        //public static Word::Document doc;
        //public static Word::Font font;
        //public static Word::Selection sel;
        //public static Word::Hyperlinks links;
        //public static Word::Hyperlink link;
        //public static Word::Bookmarks marks;
        //public static Word::Bookmark mark;
        //public static Word::Range range;

        public static string getPropertyValue(string propertyName, ModelItem currItem)
        {
            List<ModelProperty> PropertyList = currItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }

        public static void realaseProcessExit(Word::Application app)
        {
            app.Quit();
            //if (CommonVariable.range != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.range);
            //    CommonVariable.range = null;
            //}
            //if (CommonVariable.mark != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.mark);
            //    CommonVariable.mark = null;
            //}
            //if (CommonVariable.marks != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.marks);
            //    CommonVariable.marks = null;
            //}
            //if (CommonVariable.link != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.link);
            //    CommonVariable.link = null;
            //}
            //if (CommonVariable.links != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.links);
            //    CommonVariable.links = null;
            //}
            //if (CommonVariable.doc != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.doc);
            //    CommonVariable.doc = null;
            //}
            //if (CommonVariable.docs != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.docs);
            //    CommonVariable.docs = null;
            //}
            //CommonVariable.app.Quit();
            //if (CommonVariable.app != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.app);
            //    CommonVariable.app = null;
            //}
            //GC.Collect();
        }
    }
}

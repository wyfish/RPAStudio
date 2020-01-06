using System;
using System.Activities;
using System.ComponentModel;
using System.Reflection;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(MarkLinkDesigner))]
    public sealed class MarkLink : AsyncCodeActivity
    {
        public MarkLink()
        {
        }

        InArgument<string> _Pic;
        [RequiredArgument]
        [OverloadGroup("Picture")]
        [Localize.LocalizedCategory("Category26")] //图片 //Image //写真
        [Localize.LocalizedDisplayName("DisplayName131")] //图片链接 //Image link //画像リンク
        [Browsable(true)]
        public InArgument<string> Pic
        {
            get { return _Pic; }
            set
            {
                _Pic = value;
            }
        }

        InArgument<string> _BookMark;
        [RequiredArgument]
        [OverloadGroup("BookMark")]
        [Localize.LocalizedCategory("Category27")] //书签 //Bookmark //しおり
        [Localize.LocalizedDisplayName("DisplayName132")] //书签名称 //Bookmark name //ブックマーク名
        [Browsable(true)]
        public InArgument<string> BookMark
        {
            get{    return _BookMark;    }
            set{    _BookMark = value;

            }
        }

        InArgument<string> _LinkName;
        [RequiredArgument]
        [OverloadGroup("Link")]
        [Localize.LocalizedCategory("Category28")] //超链接 //Hyperlinks //ハイパーリンク
        [Localize.LocalizedDisplayName("DisplayName133")] //超链接名称 //Hyperlink name //ハイパーリンク名
        [Browsable(true)]
        public InArgument<string> LinkName
        {
            get {   return _LinkName;   }
            set { _LinkName = value;  }
        }

        InArgument<string> _LinkMark;
        [RequiredArgument]
        [OverloadGroup("Link")]
        [Localize.LocalizedCategory("Category28")] //超链接 //Hyperlinks //ハイパーリンク   
        [Localize.LocalizedDisplayName("DisplayName134")] //超链接标签 //Hyperlink label //ハイパーリンクラベル
        [Browsable(true)]
        public InArgument<string> LinkMark
        {
            get { return _LinkMark; }
            set { _LinkMark = value; }
        }

        InArgument<string> _LinkAddr;
        [RequiredArgument]
        [OverloadGroup("Link")]
        [Localize.LocalizedCategory("Category28")] //超链接 //Hyperlinks //ハイパーリンク
        [Localize.LocalizedDisplayName("DisplayName135")] //超链接地址 //Hyperlink address //ハイパーリンクアドレス
        [Browsable(true)]
        public InArgument<string> LinkAddr
        {
            get { return _LinkAddr; }
            set { _LinkAddr = value; }
        }


        //设置属性可见/不可见
        private void SetPropertyVisibility(object obj, string propertyName, bool visible)
        {
            try
            {
                Type type = typeof(BrowsableAttribute);
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
                AttributeCollection attrs = props[propertyName].Attributes;
                FieldInfo fld = type.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
                fld.SetValue(attrs[type], visible);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex);
            }
        }

        //设置属性只读
        //private void SetPropertyReadOnly(object obj, string propertyName, bool readOnly)
        //{
        //    try
        //    {
        //        Type type = typeof(ReadOnlyAttribute);
        //        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
        //        AttributeCollection attrs = props[propertyName].Attributes;
        //        FieldInfo fld = type.GetField("ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
        //        fld.SetValue(attrs[type], readOnly);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Exception: " + ex);
        //    }
        //}

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/mark.png"; } }

        [Browsable(false)]
        public string ClassName { get { return "MarkLink"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[WordCreate.GetWordAppTag];
            Application wordApp = property.GetValue(context.DataContext) as Application;

            try
            {
                string linkName = LinkName.Get(context);
                string linkMark = LinkMark.Get(context);
                string linkAddr = LinkAddr.Get(context);
                string bookMark = BookMark.Get(context);
                string pic = Pic.Get(context);

                if (linkName != null)
                {
                    Hyperlinks links = wordApp.ActiveDocument.Hyperlinks;
                    links.Add(wordApp.Selection, linkAddr, linkMark, "", linkName, linkMark);
                }
                if (bookMark != null)
                {
                    wordApp.ActiveDocument.Bookmarks.Add(bookMark);
                }
                if (pic != null)
                {
                    InlineShapes lineshapes = wordApp.Selection.InlineShapes;
                    InlineShape lineshape = lineshapes.AddPicture(pic);
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(wordApp);
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}

using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(BuildDataTableDesigner))]
    public sealed class BuildDataTable : AsyncCodeActivity
    {
        public BuildDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "TableName";
            dataTable.Columns.Add(new DataColumn("Column1", typeof(string))
            {
                MaxLength = 100
            });
            dataTable.Columns.Add(new DataColumn("Column2", typeof(int)));
            DataRow dataRow = dataTable.NewRow();
            dataRow["Column1"] = "text";
            dataRow["Column2"] = 1;
            dataTable.Rows.Add(dataRow);
            StringWriter stringWriter = new StringWriter();
            dataTable.WriteXml(stringWriter, XmlWriteMode.WriteSchema);
            this.TableInfo = stringWriter.ToString();
        }

        public new string DisplayName
        {
            get
            {
                return "BuildDataTable";
            }
        }

        [RequiredArgument, Browsable(false)]
        public string TableInfo
        {
            get;
            set;
        }


        [Category("输出")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("根据行列信息生成的DataTable表")]
        public OutArgument<DataTable> DataTable { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }


        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return DisplayName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            DataTable dataTable = new DataTable();
            BuildDataTable.ReadDataTableFromXML(this.TableInfo, dataTable);
            this.DataTable.Set(context, dataTable);
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }


        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }


        public static void ReadDataTableFromXML(string text, DataTable dt)
        {
            try
            {
                using (StringReader stringReader = new StringReader(text))
                {
                    dt.ReadXml(stringReader);
                }
            }
            catch (ArgumentException)
            {
                using (XmlReader xmlReader = BuildDataTable.ReplaceAssemblyName(text))
                {
                    dt.ReadXml(xmlReader);
                }
            }
        }

        private static XmlReader ReplaceAssemblyName(string info)
        {
            List<Tuple<string, string>> expr_05 = new List<Tuple<string, string>>();
            expr_05.Add(new Tuple<string, string>("GenericValue", "System"));
            expr_05.Add(new Tuple<string, string>("Image", "UiAutomation"));
            XElement xElement = XElement.Parse(info);
            using (List<Tuple<string, string>>.Enumerator enumerator = expr_05.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Tuple<string, string> tuple = enumerator.Current;
                    using (IEnumerator<XElement> enumerator2 = (from el in xElement.Descendants(BuildDataTable._xsNamespace + "element")
                                                                where BuildDataTable.CheckReplaceCondition(el, tuple.Item1)
                                                                select el).GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            BuildDataTable.Replace(enumerator2.Current, tuple.Item2);
                        }
                    }
                }
            }
            return xElement.CreateReader();
        }

        private static bool CheckReplaceCondition(XElement node, string component)
        {
            if (node == null)
            {
                return false;
            }
            XAttribute expr_1A = node.Attribute(BuildDataTable._msdataNamespace + "DataType");
            string text = (expr_1A != null) ? expr_1A.ToString() : null;
            return !string.IsNullOrWhiteSpace(text) && (text.Contains(component) && text.Contains("Culture") && text.Contains("PublicKeyToken") && text.Contains("Version")) && text.Contains("");
        }
        private static readonly XNamespace _xsNamespace = "http://www.w3.org/2001/XMLSchema";
        private static readonly XNamespace _msdataNamespace = "urn:schemas-microsoft-com:xml-msdata";
        private static void Replace(XElement node, string package)
        {
        }
    }
}
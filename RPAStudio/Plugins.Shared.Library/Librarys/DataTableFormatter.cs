using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;


namespace Plugins.Shared.Library.Librarys
{
    public static class DataTableFormatter
    {
        private const char delimitator = ',';

        public static string FormatTable(DataTable dataTable, int maxLength = 0)
        {
            if (dataTable == null)
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(DataTableFormatter.FormatColumnNames(dataTable));
            if (maxLength != 0 && stringBuilder.Length >= maxLength)
            {
                return stringBuilder.ToString().Substring(0, maxLength);
            }
            int arg_3D_0 = dataTable.Columns.Count;
            foreach (DataRow row in dataTable.Rows)
            {
                stringBuilder.AppendLine(DataTableFormatter.FormatRow(row, 0));
                if (maxLength != 0 && stringBuilder.Length >= maxLength)
                {
                    return stringBuilder.ToString().Substring(0, maxLength);
                }
            }
            return stringBuilder.ToString();
        }

        public static string FormatRow(DataRow Row, int maxLenght = 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            object[] itemArray = Row.ItemArray;
            for (int i = 0; i < itemArray.Length; i++)
            {
                object obj = itemArray[i];
                string text = (obj == null) ? string.Empty : obj.ToString();
                if (!flag)
                {
                    stringBuilder.Append(',');
                }
                if (text.IndexOfAny(new char[]
                {
                    '"',
                    ','
                }) != -1)
                {
                    text = string.Format("\"{0}\"", text.Replace("\"", "\"\""));
                }
                stringBuilder.Append(text);
                flag = false;
            }
            if (maxLenght != 0 && stringBuilder.Length >= maxLenght)
            {
                return stringBuilder.ToString().Substring(0, maxLenght);
            }
            return stringBuilder.ToString();
        }

        private static string FormatColumnNames(DataTable dataTable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            IEnumerator enumerator = dataTable.Columns.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string text = ((DataColumn)enumerator.Current).ColumnName.ToString();
                    if (!flag)
                    {
                        stringBuilder.Append(",");
                    }
                    if (text.IndexOfAny(new char[]
                    {
                        '"',
                        ','
                    }) != -1)
                    {
                        text = string.Format("\"{0}\"", text.Replace("\"", "\"\""));
                    }
                    stringBuilder.Append(text);
                    flag = false;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return stringBuilder.ToString();
        }
    }
}

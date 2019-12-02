using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace Plugins.Shared.Library.Librarys
{
    internal interface ITableFormatter
    {
        DataTable Format(string text, IFormatOptions formatOptions, ITableOptions tableOptions);
        DataTable Format(IEnumerable<KeyValuePair<Rectangle, string>> textChunks, ITableOptions tableOptions);
    }
    //public class TableFormatter : ITableFormatter
    //{
    //    private static readonly Type s_DefaultType = typeof(string);

       // private static readonly ITypeDetector[] s_TypeDetectors = new ITypeDetector[]
         //   }
    //    {
    //        new DateTimeDetector(),
    //        new BoolDetector(),
    //        new IntDetector(),
    //        new DecimalDetector(),
    //        new DoubleDetector()
    //    };

    //    public DataTable Format(string text, IFormatOptions formatOptions, ITableOptions tableOptions)
    //    {
    //        if (string.IsNullOrWhiteSpace(text))
    //        {
    //            return new DataTable();
    //        }
    //        ITableDetector detector = this.ConstructDetector(text, formatOptions);
    //        return this.Process(detector, tableOptions ?? TableOptions.Default);
    //    }

    //    private ITableDetector ConstructDetector(string text, IFormatOptions formatOptions)
    //    {
    //        if (formatOptions.CSVParsing)
    //        {
    //            return new CSVDetector(text, formatOptions);
    //        }
    //        if (formatOptions != null && formatOptions.ColumnSizes != null && formatOptions.ColumnSizes.Count<int>() > 0)
    //        {
    //            return new FixedColumnsDetector(text, formatOptions ?? FormatOptions.Default);
    //        }
    //        return new SeparatorBasedDetector(text, formatOptions ?? FormatOptions.Default);
    //    }

    //    public DataTable Format(IEnumerable<KeyValuePair<Rectangle, string>> textChunks, ITableOptions tableOptions)
    //    {
    //        if (textChunks == null || textChunks.Count<KeyValuePair<Rectangle, string>>() <= 0)
    //        {
    //            return new DataTable();
    //        }
    //        return this.Process(new ChunkBasedDetector(textChunks), tableOptions ?? TableOptions.Default);
    //    }

    //    private DataTable Process(ITableDetector detector, ITableOptions tableOptions)
    //    {
    //        string[,] array = detector.Detect();
    //        int length = array.GetLength(0);
    //        int length2 = array.GetLength(1);
    //        DataTable dataTable = new DataTable();
    //        if (length > 0 && length2 > 0)
    //        {
    //            object[][] array2 = new object[length2][];
    //            for (int i = tableOptions.UseRowHeader ? 1 : 0; i < length2; i++)
    //            {
    //                string[] array3 = new string[length];
    //                for (int j = 0; j < length; j++)
    //                {
    //                    array3[j] = array[j, i];
    //                }
    //                Type dataType;
    //                array2[i] = this.AutoDetectType(array3, tableOptions, out dataType);
    //                DataColumn dataColumn = new DataColumn(tableOptions.UseColumnHeader ? array[0, i] : string.Empty, dataType);
    //                dataTable.Columns.Add(dataColumn);
    //                dataColumn.Caption = string.Empty;
    //            }
    //            for (int k = tableOptions.UseColumnHeader ? 1 : 0; k < length; k++)
    //            {
    //                DataRow dataRow = dataTable.NewRow();
    //                dataTable.Rows.Add(dataRow);
    //                int num = tableOptions.UseRowHeader ? 1 : 0;
    //                int num2 = 0;
    //                while (num2 + num < length2)
    //                {
    //                    dataRow[num2] = array2[num2 + num][k];
    //                    num2++;
    //                }
    //            }
    //        }
    //        return dataTable;
    //    }

    //    private object[] AutoDetectType(string[] data, ITableOptions tableOptions, out Type type)
    //    {
    //        type = TableFormatter.s_DefaultType;
    //        if (data == null || data.Length == 0 || tableOptions == null || !tableOptions.AutoDetectTypes)
    //        {
    //            return data;
    //        }
    //        object[] array = new object[data.Length];
    //        ITypeDetector[] array2 = TableFormatter.s_TypeDetectors;
    //        for (int i = 0; i < array2.Length; i++)
    //        {
    //            ITypeDetector typeDetector = array2[i];
    //            bool flag = true;
    //            int num = tableOptions.UseColumnHeader ? 1 : 0;
    //            while (num < data.Length & flag)
    //            {
    //                if (string.IsNullOrEmpty(data[num]))
    //                {
    //                    array[num] = DBNull.Value;
    //                }
    //                else if (!typeDetector.CanConvert(data[num].ToString(), out array[num]))
    //                {
    //                    flag = false;
    //                }
    //                num++;
    //            }
    //            if (flag)
    //            {
    //                type = typeDetector.Type;
    //                return array;
    //            }
    //        }
    //        return data;
    //    }
    //}
}

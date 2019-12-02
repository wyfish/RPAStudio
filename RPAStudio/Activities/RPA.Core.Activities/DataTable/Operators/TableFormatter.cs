using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Plugins.Shared.Library.Librarys;

namespace RPA.Core.Activities.DataTableActivity.Operators
{
    internal interface ITableFormatter
    {
        DataTable Format(string text, IFormatOptions formatOptions, ITableOptions tableOptions);
        DataTable Format(IEnumerable<KeyValuePair<Rectangle, string>> textChunks, ITableOptions tableOptions);
    }
    internal interface ITypeDetector
    {
        Type Type
        {
            get;
        }

        bool CanConvert(string value, out object result);
    }
    internal interface ITableDetector
    {
        string[,] Detect();
    }
    internal abstract class BaseTypeDetector<T> : ITypeDetector
    {
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        public bool CanConvert(string value, out object objValue)
        {
            T t;
            bool arg_11_0 = this.CanConvert(value, out t);
            objValue = t;
            return arg_11_0;
        }
        public abstract bool CanConvert(string value, out T result);
    }

    internal class DateTimeDetector : BaseTypeDetector<DateTime>
    {
        public override bool CanConvert(string value, out DateTime result)
        {
            return DateTime.TryParse(value, null, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault, out result);
        }
    }
    internal class BoolDetector : BaseTypeDetector<bool>
    {
        public override bool CanConvert(string value, out bool result)
        {
            return bool.TryParse(value, out result);
        }
    }
    internal class IntDetector : BaseTypeDetector<int>
    {
        public override bool CanConvert(string value, out int result)
        {
            return int.TryParse(value, NumberStyles.Number, null, out result);
        }
    }
    internal class DecimalDetector : BaseTypeDetector<decimal>
    {
        public override bool CanConvert(string value, out decimal result)
        {
            return decimal.TryParse(value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol, null, out result);
        }
    }
    internal class DoubleDetector : BaseTypeDetector<double>
    {
        public override bool CanConvert(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, null, out result);
        }
    }

    public class CSVDetector : ITableDetector
    {
        // Fields
        private IFormatOptions _format;
        private string _input;

        // Methods
        public CSVDetector(string input, IFormatOptions format)
        {
            this._input = input ?? string.Empty;
            this._format = format ?? FormatOptions.Default;
        }

        public string[,] Detect()
        {
            string str = string.IsNullOrEmpty(this._format.ColumnSeparators) ? "," : this._format.ColumnSeparators;
            CsvConfiguration configuration1 = new CsvConfiguration();
            //configuration1.set_Delimiter(str);
            //configuration1.set_ThrowOnBadData(false);
            //configuration1.set_BadDataCallback(delegate (string readerContext) {});
            configuration1.Delimiter = str;
            configuration1.ThrowOnBadData = false;
            configuration1.BadDataCallback = delegate (string readerContext) { };

            CsvConfiguration configuration = configuration1;
            List<string[]> source = new List<string[]>();
            int num = 0;
            using (CsvParser parser = new CsvParser(new StringReader(this._input), configuration))
            {
                string[] item = null;
                while ((item = parser.Read()) != null)
                {
                    num = Math.Max(item.Length, num);
                    source.Add(item);
                }
            }
            string[,] strArray = new string[source.Count, num];
            foreach (var type in source.SelectMany((r, i) => r.Select((c, j) => new
            {
                Value = c,
                Ridx = i,
                Cidx = j
            })))
            {
                strArray[type.Ridx, type.Cidx] = type.Value;
            }
            return strArray;
        }
    }
    internal class FixedColumnsDetector : ITableDetector
    {
        // Fields
        private string _text;
        private IFormatOptions _formatOptions;
        private const string ColumnDelimiters = ",";

        // Methods
        internal FixedColumnsDetector(string text, IFormatOptions formatOptions)
        {
            this._text = text;
            this._formatOptions = formatOptions;
        }

        public string[,] Detect()
        {
            int[] numArray = this.ValidateColumnLengths();
            int length = numArray.Length;
            string str = string.IsNullOrEmpty(this._formatOptions.NewLineSeparator) ? Environment.NewLine : this._formatOptions.NewLineSeparator;
            string[] separator = new string[] { str };
            string[] strArray = this._text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            int num2 = strArray.Length;
            string[,] strArray2 = new string[num2, length];
            for (int i = 0; i < num2; i++)
            {
                string text = strArray[i];
                int index = 0;
                int start = 0;
                while (index < length)
                {
                    strArray2[i, index] = SafeSubstring(text, start, numArray[index]);
                    start += numArray[index];
                    index++;
                }
            }
            return strArray2;
        }

        private static string SafeSubstring(string text, int start, int length)
        {
            if (((length <= 0) || (start < 0)) || ((start >= text.Length) || string.IsNullOrEmpty(text)))
            {
                return string.Empty;
            }
            if (((start + length) - 1) >= text.Length)
            {
                return text.Substring(start);
            }
            return text.Substring(start, length);
        }

        private int[] ValidateColumnLengths()
        {
            if (((this._formatOptions == null) || (this._formatOptions.ColumnSizes == null)) || (this._formatOptions.ColumnSizes.Count<int>() == 0))
            {
                throw new ArgumentException("Invalid column length specifier");
            }
            IEnumerable<int> source = from size in this._formatOptions.ColumnSizes
                                      where size <= 0
                                      select size;
            if ((source != null) && (source.Count<int>() > 0))
            {
                throw new ArgumentException("Column length must be a positive value");
            }
            return this._formatOptions.ColumnSizes.ToArray<int>();
        }
    }


    internal class TextRange
    {
        // Fields
        internal static readonly TextRange Empty;

        // Methods
        static TextRange()
        {
            TextRange range1 = new TextRange
            {
                Start = 0,
                End = -1
            };
            Empty = range1;
        }

        internal bool Overlaps(int value) =>
            ((this.Start <= value) && (this.End >= value));

        internal bool Overlaps(TextRange range)
        {
            if (this.Equals(Empty) || range.Equals(Empty))
            {
                return false;
            }
            return ((this.Start <= range.End) && (this.End >= range.Start));
        }

        // Properties
        internal int Start { get; set; }

        internal int End { get; set; }

        internal string Text { get; set; }
    }
    internal class Line
    {
        internal List<TextRange> Chunks
        {
            get;set;
        }
        internal int Count
        {
            get
            {
                return this.Chunks.Count;
            }
        }
        public Line()
        {
            Chunks = new List<TextRange>();
        }
    }


    internal class PositionalTextComparer : IComparer<KeyValuePair<Rectangle, string>>
    {
        public int Compare(KeyValuePair<Rectangle, string> chunk1, KeyValuePair<Rectangle, string> chunk2)
        {
            if (chunk1.Key.Top == chunk2.Key.Top)
            {
                return chunk1.Key.Left - chunk2.Key.Left;
            }
            return chunk1.Key.Top - chunk2.Key.Top;
        }
    }
    internal class ChunkBasedDetector : PositionalDetector
    {
        // Fields
        private IEnumerable<KeyValuePair<Rectangle, string>> _textChunks;

        // Methods
        internal ChunkBasedDetector(IEnumerable<KeyValuePair<Rectangle, string>> textChunks)
        {
            this._textChunks = textChunks;
        }

        protected override Line[] DetectLines()
        {
            if ((this._textChunks == null) || (this._textChunks.Count<KeyValuePair<Rectangle, string>>() <= 0))
            {
                return null;
            }
            this._textChunks = this._textChunks.OrderBy<KeyValuePair<Rectangle, string>, KeyValuePair<Rectangle, string>>(chunk => chunk, new PositionalTextComparer());
            List<Line> source = new List<Line>();
            TextRange range = new TextRange
            {
                Start = 0x7fffffff,
                End = -2147483648
            };
            foreach (KeyValuePair<Rectangle, string> pair in this._textChunks)
            {
                Line item = source.LastOrDefault<Line>();
                if ((item == null) || (!range.Overlaps(pair.Key.Top) && !range.Overlaps(pair.Key.Bottom)))
                {
                    item = new Line();
                    source.Add(item);
                }
                TextRange range2 = new TextRange
                {
                    Start = pair.Key.Left,
                    End = pair.Key.Right,
                    Text = pair.Value
                };
                item.Chunks.Add(range2);
                range.Start = Math.Min(range.Start, pair.Key.Top);
                range.End = Math.Max(range.End, pair.Key.Bottom);
            }
            return source.ToArray();
        }
    }

    internal abstract class PositionalDetector : ITableDetector
    {
        // Fields
        private static readonly string[,] Empty = new string[0, 0];
        protected const char Space = ' ';
        public static Func<Line, bool> __5_0;
        public static Func<Line, List<TextRange>> __5_1;
        public static Func<IEnumerable<TextRange>, bool> __5_3;
        internal bool b__5_0(Line line)
        {
            return line.Count > 0;
        }
        internal List<TextRange> b__5_1(Line line)
        {
            return line.Chunks;
        }
        internal bool b__5_3(IEnumerable<TextRange> line)
        {
            return line.Count<TextRange>() > 0;
        }
        // Methods
        protected PositionalDetector()
        {
        }

        public string[,] Detect()
        {
            Line[] lines = this.DetectLines();
            if ((lines == null) || (lines.Length == 0))
            {
                return Empty;
            }
            TextRange[] rangeArray = this.DetectColumns(lines);
            string[,] strArray = new string[lines.Length, rangeArray.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < rangeArray.Length; j++)
                {
                    strArray[i, j] = this.GetCell(lines, i, rangeArray[j]);
                }
            }
            return strArray;
        }

        protected TextRange[] DetectColumns(Line[] lines)
        {
            List<TextRange> list = new List<TextRange>();
            Func<Line, bool> arg_26_1;
            if ((arg_26_1 = PositionalDetector.__5_0) == null)
			{
                arg_26_1 = (PositionalDetector.__5_0 = new Func<Line, bool>(b__5_0));
            }
            IEnumerable<Line> arg_4A_0 = lines.Where(arg_26_1);
            Func<Line, List<TextRange>> arg_4A_1;
            if ((arg_4A_1 = PositionalDetector.__5_1) == null)
			{
                arg_4A_1 = (PositionalDetector.__5_1 = new Func<Line, List<TextRange>>(b__5_1));
            }
            IEnumerable<IEnumerable<TextRange>> enumerable = arg_4A_0.Select(arg_4A_1);
            while (enumerable.Count<IEnumerable<TextRange>>() > 0)
            {
                TextRange textRange = new TextRange
                {
                    Start = 2147483647,
                    End = -2147483648
                };
                using (IEnumerator<IEnumerable<TextRange>> enumerator = enumerable.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        TextRange textRange2 = enumerator.Current.First<TextRange>();
                        if (textRange2.Start < textRange.Start)
                        {
                            textRange = textRange2;
                        }
                    }
                }
                int end = textRange.End;
                int columnEnd = end;
                do
                {
                    columnEnd = end;
                    using (IEnumerator<IEnumerable<TextRange>> enumerator = enumerable.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            foreach (TextRange current in enumerator.Current)
                            {
                                if (current.Start > end)
                                {
                                    break;
                                }
                                if (current.Overlaps(end))
                                {
                                    end = current.End;
                                }
                            }
                        }
                    }
                }
                while (end > columnEnd);
                list.Add(new TextRange
                {
                    Start = textRange.Start,
                    End = columnEnd
                });
                IEnumerable<IEnumerable<TextRange>> arg_1A3_0 = enumerable.Select(delegate (IEnumerable<TextRange> line)
                {
                    Func<TextRange, bool> arg_20_1;
                    arg_20_1 = ((TextRange chunk) => chunk.Start > columnEnd);
                    return line.Where(arg_20_1);
                });
            Func<IEnumerable<TextRange>, bool> arg_1A3_1;
            if ((arg_1A3_1 = PositionalDetector.__5_3) == null)
				{
                arg_1A3_1 = (PositionalDetector.__5_3 = new Func<IEnumerable<TextRange>, bool>(b__5_3));
            }
            enumerable = arg_1A3_0.Where(arg_1A3_1);
            }
			return list.ToArray();
        }

        protected abstract Line[] DetectLines();
        protected virtual string GetCell(Line[] lines, int line, TextRange range)
        {
            StringBuilder builder = new StringBuilder();
            foreach (TextRange range2 in from chunk in lines[line].Chunks
                                         where chunk.Overlaps(range)
                                         select chunk)
            {
                if (!string.IsNullOrEmpty(range2.Text))
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(' ');
                    }
                    builder.Append(range2.Text);
                }
            }
            return builder.ToString();
        }

}

    internal class SeparatorBasedDetector : PositionalDetector
    {
        private string _text;

        private IFormatOptions _formatOptions;

        private string[] _lines;

        private char[] _trimmingChars;

        internal SeparatorBasedDetector(string text, IFormatOptions formatOptions)
        {
            this._text = text;
            this._formatOptions = SeparatorBasedDetector.ValidateFormatOptions(formatOptions);
            this._trimmingChars = this._formatOptions.ColumnSeparators.ToCharArray();
        }

        protected override string GetCell(Line[] lines, int line, TextRange range)
        {
            int num = (range.Start < this._lines[line].Length) ? range.Start : -1;
            int num2 = (range.End < this._lines[line].Length) ? range.End : -1;
            string text = string.Empty;
            if (num >= 0)
            {
                text = ((num2 >= 0) ? this._lines[line].Substring(num, num2 - num + 1) : this._lines[line].Substring(num));
                text = text.Trim(this._trimmingChars);
            }
            return text;
        }

        protected override Line[] DetectLines()
        {
            this._lines = this._text.Split(new string[]
            {
                this._formatOptions.NewLineSeparator
            }, StringSplitOptions.RemoveEmptyEntries);
            Regex regex = new Regex(string.Format("[^{0}]+", Regex.Escape(this._formatOptions.ColumnSeparators)));
            List<Line> list = new List<Line>();
            for (int i = 0; i < this._lines.Length; i++)
            {
                Line line = new Line();
                foreach (Match match in regex.Matches(this._lines[i]))
                {
                    line.Chunks.Add(new TextRange
                    {
                        Start = match.Index,
                        End = match.Index + match.Length - 1
                    });
                }
                list.Add(line);
            }
            return list.ToArray();
        }

        private static IFormatOptions ValidateFormatOptions(IFormatOptions formatOptions)
        {
            IFormatOptions result;
            if (formatOptions != null)
            {
                result = new FormatOptions
                {
                    ColumnSeparators = string.IsNullOrEmpty(formatOptions.ColumnSeparators) ? FormatOptions.Default.ColumnSeparators : formatOptions.ColumnSeparators,
                    NewLineSeparator = string.IsNullOrEmpty(formatOptions.NewLineSeparator) ? FormatOptions.Default.NewLineSeparator : formatOptions.NewLineSeparator,
                    ColumnSizes = formatOptions.ColumnSizes,
                    PreserveNewLines = formatOptions.PreserveNewLines,
                    PreserveStrings = formatOptions.PreserveStrings
                };
            }
            else
            {
                result = FormatOptions.Default;
            }
            return result;
        }
    }


    public class TableFormatter : ITableFormatter
    {
        private static readonly Type s_DefaultType = typeof(string);

        private static readonly ITypeDetector[] s_TypeDetectors = new ITypeDetector[]
        {
            new DateTimeDetector(),
            new BoolDetector(),
            new IntDetector(),
            new DecimalDetector(),
            new DoubleDetector()
        };

        public DataTable Format(string text, IFormatOptions formatOptions, ITableOptions tableOptions)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new DataTable();
            }
            ITableDetector detector = this.ConstructDetector(text, formatOptions);
            return this.Process(detector, tableOptions ?? TableOptions.Default);
        }

        public DataTable Format(IEnumerable<KeyValuePair<Rectangle, string>> textChunks, ITableOptions tableOptions)
        {
            if (textChunks == null || textChunks.Count<KeyValuePair<Rectangle, string>>() <= 0)
            {
                return new DataTable();
            }
            return this.Process(new ChunkBasedDetector(textChunks), tableOptions ?? TableOptions.Default);
        }


        private ITableDetector ConstructDetector(string text, IFormatOptions formatOptions)
        {
            if (formatOptions.CSVParsing)
            {
                return new CSVDetector(text, formatOptions);
            }
            if (formatOptions != null && formatOptions.ColumnSizes != null && formatOptions.ColumnSizes.Count<int>() > 0)
            {
                return new FixedColumnsDetector(text, formatOptions ?? FormatOptions.Default);
            }
            return new SeparatorBasedDetector(text, formatOptions ?? FormatOptions.Default);
        }

        private DataTable Process(ITableDetector detector, ITableOptions tableOptions)
        {
            string[,] array = detector.Detect();
            int length = array.GetLength(0);
            int length2 = array.GetLength(1);
            DataTable dataTable = new DataTable();
            if (length > 0 && length2 > 0)
            {
                object[][] array2 = new object[length2][];
                for (int i = tableOptions.UseRowHeader ? 1 : 0; i < length2; i++)
                {
                    string[] array3 = new string[length];
                    for (int j = 0; j < length; j++)
                    {
                        array3[j] = array[j, i];
                    }
                    Type dataType;
                    array2[i] = this.AutoDetectType(array3, tableOptions, out dataType);
                    DataColumn dataColumn = new DataColumn(tableOptions.UseColumnHeader ? array[0, i] : string.Empty, dataType);
                    dataTable.Columns.Add(dataColumn);
                    dataColumn.Caption = string.Empty;
                }
                for (int k = tableOptions.UseColumnHeader ? 1 : 0; k < length; k++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataTable.Rows.Add(dataRow);
                    int num = tableOptions.UseRowHeader ? 1 : 0;
                    int num2 = 0;
                    while (num2 + num < length2)
                    {
                        dataRow[num2] = array2[num2 + num][k];
                        num2++;
                    }
                }
            }
            return dataTable;
        }

        private object[] AutoDetectType(string[] data, ITableOptions tableOptions, out Type type)
        {
            type = TableFormatter.s_DefaultType;
            if (data == null || data.Length == 0 || tableOptions == null || !tableOptions.AutoDetectTypes)
            {
                return data;
            }
            object[] array = new object[data.Length];
            ITypeDetector[] array2 = TableFormatter.s_TypeDetectors;
            for (int i = 0; i < array2.Length; i++)
            {
                ITypeDetector typeDetector = array2[i];
                bool flag = true;
                int num = tableOptions.UseColumnHeader ? 1 : 0;
                while (num < data.Length & flag)
                {
                    if (string.IsNullOrEmpty(data[num]))
                    {
                        array[num] = DBNull.Value;
                    }
                    else if (!typeDetector.CanConvert(data[num].ToString(), out array[num]))
                    {
                        flag = false;
                    }
                    num++;
                }
                if (flag)
                {
                    type = typeDetector.Type;
                    return array;
                }
            }
            return data;
        }
    }


}

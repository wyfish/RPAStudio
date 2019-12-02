using System;
using System.Collections.Generic;

namespace Plugins.Shared.Library.Librarys
{
    public interface IFormatOptions
    {
        string ColumnSeparators
        {
            get;
        }

        bool PreserveStrings
        {
            get;
        }

        string NewLineSeparator
        {
            get;
        }

        bool CSVParsing
        {
            get;
            set;
        }

        IEnumerable<int> ColumnSizes
        {
            get;
        }

        bool PreserveNewLines
        {
            get;
        }
    }

    public class FormatOptions : IFormatOptions
    {
        public static readonly IFormatOptions Default = new FormatOptions
        {
            ColumnSeparators = " \t",
            NewLineSeparator = Environment.NewLine
        };

        private string _columnSeparators;

        private string _newLineSeparator;

        private bool _preserveStrings;

        private IEnumerable<int> _columnSizes;

        private bool _preserveNewLines;

        public string ColumnSeparators
        {
            get
            {
                return this._columnSeparators;
            }
            set
            {
                this._columnSeparators = value;
            }
        }

        public string NewLineSeparator
        {
            get
            {
                return this._newLineSeparator;
            }
            set
            {
                this._newLineSeparator = value;
            }
        }

        public bool PreserveStrings
        {
            get
            {
                return this._preserveStrings;
            }
            set
            {
                this._preserveStrings = value;
            }
        }

        public bool CSVParsing
        {
            get;
            set;
        }

        public IEnumerable<int> ColumnSizes
        {
            get
            {
                return this._columnSizes;
            }
            set
            {
                this._columnSizes = value;
            }
        }

        public bool PreserveNewLines
        {
            get
            {
                return this._preserveNewLines;
            }
            set
            {
                this._preserveNewLines = value;
            }
        }
    }
}

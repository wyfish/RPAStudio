using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Librarys
{
    public interface ITableOptions
    {
        bool AutoDetectTypes
        {
            get;
        }

        bool UseColumnHeader
        {
            get;
        }

        bool UseRowHeader
        {
            get;
        }
    }
    public class TableOptions : ITableOptions
    {
        public static readonly ITableOptions Default = new TableOptions();

        private bool _autoDetectTypes = true;

        private bool _useColumnHeader;

        private bool _useRowHeader;

        public bool AutoDetectTypes
        {
            get
            {
                return this._autoDetectTypes;
            }
            set
            {
                this._autoDetectTypes = value;
            }
        }

        public bool UseColumnHeader
        {
            get
            {
                return this._useColumnHeader;
            }
            set
            {
                this._useColumnHeader = value;
            }
        }

        public bool UseRowHeader
        {
            get
            {
                return this._useRowHeader;
            }
            set
            {
                this._useRowHeader = value;
            }
        }
    }
}

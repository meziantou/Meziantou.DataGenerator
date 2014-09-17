using System;
using System.Text.RegularExpressions;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core
{
    public class Hint
    {
        private readonly string _tablePattern;
        private readonly string _columnPattern;
        private readonly DataGenerator _dataGenerator;
        private readonly Regex _tableRegex;
        private readonly Regex _columnRegex;

        public string ColumnPattern
        {
            get { return _columnPattern; }
        }

        public string TablePattern
        {
            get { return _tablePattern; }
        }

        public DataGenerator DataGenerator
        {
            get { return _dataGenerator; }
        }

        public Hint(string tablePattern, string columnPattern, DataGenerator dataGenerator)
        {
            if (dataGenerator == null) throw new ArgumentNullException("dataGenerator");

            _tablePattern = tablePattern;
            _columnPattern = columnPattern;
            _dataGenerator = dataGenerator;

            if (_tablePattern != null)
            {
                _tableRegex = new Regex(tablePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            if (_columnPattern != null)
            {
                _columnRegex = new Regex(columnPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }

        public virtual bool Match(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            bool matchTable = true;
            bool matchColumn = true;
            if (_tableRegex != null)
            {
                matchTable = _tableRegex.IsMatch(column.Parent.Name);
            }

            if (_columnRegex != null)
            {
                matchColumn = _columnRegex.IsMatch(column.Name);
            }

            return matchTable && matchColumn;
        }
    }
}
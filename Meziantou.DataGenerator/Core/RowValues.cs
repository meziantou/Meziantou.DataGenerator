using System;
using System.Collections.Generic;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core
{
    public class RowValues
    {
        private readonly IList<Column> _columns;
        private readonly IList<DataGenerator> _dataGenerators;
        private readonly object[] _values;

        public RowValues(IList<Column> columns, IList<DataGenerator> dataGenerators)
        {
            if (columns == null) throw new ArgumentNullException("columns");
            if (dataGenerators == null) throw new ArgumentNullException("dataGenerators");
            if (columns.Count != dataGenerators.Count)
                throw new ArgumentException("Columns and data generators length are differents.");

            _columns = columns;
            _dataGenerators = dataGenerators;
            _values = new object[columns.Count];
        }

        public object GetValue(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            for (int i = 0; i < _columns.Count; i++)
            {
                if (column == _columns[i])
                {
                    return _values[i];
                }
            }

            return null;
        }

        public object GetValue(DataGenerator dataGenerator)
        {
            if (dataGenerator == null) throw new ArgumentNullException("dataGenerator");

            for (int i = 0; i < _dataGenerators.Count; i++)
            {
                if (dataGenerator == _dataGenerators[i])
                {
                    return _values[i];
                }
            }

            return null;
        }

        public object GetValue(WellKnownDataType dataType)
        {
            for (int i = 0; i < _dataGenerators.Count; i++)
            {
                if (_dataGenerators[i].WellKnownDataType == dataType)
                {
                    return _values[i];
                }
            }

            return null;
        }

        public IEnumerable<object> GetValues(WellKnownDataType dataType)
        {
            for (int i = 0; i < _dataGenerators.Count; i++)
            {
                if (_dataGenerators[i].WellKnownDataType == dataType)
                {
                    yield return _values[i];
                }
            }
        }

        public void AddValue(Column column, object value)
        {
            if (column == null) throw new ArgumentNullException("column");

            for (int i = 0; i < _columns.Count; i++)
            {
                if (column == _columns[i])
                {
                    _values[i] = value;
                }
            }
        }

        public void AddValue(DataGenerator dataGenerator, object value)
        {
            if (dataGenerator == null) throw new ArgumentNullException("dataGenerator");

            for (int i = 0; i < _dataGenerators.Count; i++)
            {
                if (dataGenerator == _dataGenerators[i])
                {
                    _values[i] = value;
                }
            }
        }
    }
}
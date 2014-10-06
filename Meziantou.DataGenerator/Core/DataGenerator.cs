using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core
{
    public abstract class DataGenerator : ICloneable, IComparable<DataGenerator>
    {
        public abstract bool CanGenerate(Column column);

        public DataGenerator()
        {
            Seed = RandomUtilities.Random.NextInt32();
        }

        public int Seed { get; set; }
        public WellKnownDataType WellKnownDataType { get; set; }

        private Random _random;

        public Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random(Seed);
                }

                return _random;
            }
        }

        public virtual int CompareTo(DataGenerator generator)
        {
            return 0;
        }

        public virtual IEnumerable<object> Generate(Project project, Column column, int count, int nullCount)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (column == null) throw new ArgumentNullException("column");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            if (count == 0)
            {
                yield break;
            }

            int modulo = -1;
            int moduloOffset = 0;
            if (!column.IsNullable)
            {
                nullCount = 0;
            }
            else
            {
                if (nullCount > count)
                {
                    nullCount = count;
                }

                if (nullCount > 0 && column.IsNullable)
                {
                    moduloOffset = Random.NextInt32(0, count / 10);
                    modulo = count / nullCount;
                }
            }

            BeforeGenerateValues(project, column, count, nullCount);

            for (int i = 0; i < count; i++)
            {
                if (nullCount > 0 && (i + moduloOffset) % modulo == 0)
                {
                    yield return DBNull.Value;
                }

                yield return GenerateCore(project, column);
            }

            AfterGenerateValues(project, column, count, nullCount);
        }

        protected virtual void AfterGenerateValues(Project project, Column column, int count, int nullCount)
        {
        }

        protected virtual void BeforeGenerateValues(Project project, Column column, int count, int nullCount)
        {
        }

        protected abstract object GenerateCore(Project project, Column column);

        protected bool IsGeneratedColumn(Column column)
        {
            return column.IsIdentity || string.Equals(column.CodeFluentType.DataType, "timestamp", StringComparison.OrdinalIgnoreCase);
        }

        protected bool IsForeginKey(Column column)
        {
            foreach (var foreignKey in column.Parent.ForeignKeys)
            {
                if (foreignKey.Columns.Contains(column))
                    return true;
            }

            return false;
        }

        protected bool IsStringDbType(DbType dbType)
        {
            DbType[] types =
            {
                DbType.String,
                DbType.StringFixedLength,
                DbType.AnsiString,
                DbType.AnsiStringFixedLength,
            };

            return types.Contains(dbType);
        }

        protected bool IsStringDbType(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            return IsStringDbType(column.CodeFluentType.DbType);
        }

        protected bool IsBinaryDbType(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            return column.CodeFluentType.DbType == DbType.Binary;
        }

        protected bool IsUnicode(Column column)
        {
            return column.CodeFluentType.IsDbUnicode;
        }

        protected bool IsNumericDbType(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            return IsNumericDbType(column.CodeFluentType.DbType);
        }

        protected bool IsNumericDbType(DbType dbType)
        {
            DbType[] types =
            {
                DbType.Byte,
                DbType.Currency,
                DbType.Decimal,
                DbType.Double,
                DbType.Int16,
                DbType.Int32,
                DbType.Int64,
                DbType.SByte,
                DbType.Single,
                DbType.UInt16,
                DbType.UInt32,
                DbType.UInt64,
            };

            return types.Contains(dbType);
        }

        public virtual object Clone()
        {
            var clone = (DataGenerator)this.MemberwiseClone();
            clone.Seed = RandomUtilities.Random.Next();
            return clone;
        }

        public virtual void Configure(XmlElement element)
        {
            if (element == null) throw new ArgumentNullException("element");

            var seed = XmlUtilities.GetAttribute(element, "seed", (int?)null);
            if (seed != null)
            {
                Seed = seed.Value;
            }

            WellKnownDataType = XmlUtilities.GetAttribute(element, "dataType", WellKnownDataType.Unknown);
        }
    }
}

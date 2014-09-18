using System;
using System.Data;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class NumberGenerator : DataGenerator
    {
        public object Minimum { get; set; }
        public object Maximum { get; set; }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsNumericDbType(column.CodeFluentType.DbType);
        }

        public override void Configure(XmlElement element)
        {
            base.Configure(element);
            Minimum = XmlUtilities.GetAttribute(element, "minimum", Minimum);
            Maximum = XmlUtilities.GetAttribute(element, "maximum", Maximum);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            if (WellKnownDataType == WellKnownDataType.TotalPrice)
            {
                decimal quantity = ConvertUtilities.ChangeType(project.CurrentRow.GetValue(WellKnownDataType.Quantity), -1L);
                decimal unitPrice = ConvertUtilities.ChangeType(project.CurrentRow.GetValue(WellKnownDataType.UnitPrice), -1L);
                if (quantity >= 0 && unitPrice >= 0)
                {
                    decimal totalPrice = quantity*unitPrice;
                    switch (column.CodeFluentType.DbType)
                    {
                        case DbType.Byte:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(byte));
                        case DbType.Currency:
                        case DbType.Decimal:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(decimal));
                        case DbType.Double:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(double));
                        case DbType.Int16:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(short));
                        case DbType.Int32:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(int));
                        case DbType.Int64:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(long));
                        case DbType.SByte:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(sbyte));
                        case DbType.Single:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(float));
                        case DbType.UInt16:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(ushort));
                        case DbType.UInt32:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(uint));
                        case DbType.UInt64:
                            return ConvertUtilities.ChangeType(totalPrice, typeof(ulong));
                    }
                }
            }

            switch (column.CodeFluentType.DbType)
            {
                case DbType.Byte:
                    return Random.NextByte(ConvertUtilities.ChangeType(Minimum, byte.MinValue), ConvertUtilities.ChangeType(Maximum, byte.MaxValue));
                case DbType.Currency:
                case DbType.Decimal:
                    return Random.NextDecimal(ConvertUtilities.ChangeType(Minimum, decimal.MinValue), ConvertUtilities.ChangeType(Maximum, decimal.MaxValue));
                case DbType.Double:
                    return Random.NextDouble(ConvertUtilities.ChangeType(Minimum, double.MinValue), ConvertUtilities.ChangeType(Maximum, double.MaxValue));
                case DbType.Int16:
                    return Random.NextInt16(ConvertUtilities.ChangeType(Minimum, short.MinValue), ConvertUtilities.ChangeType(Maximum, short.MaxValue));
                case DbType.Int32:
                    return Random.NextInt32(ConvertUtilities.ChangeType(Minimum, int.MinValue), ConvertUtilities.ChangeType(Maximum, int.MaxValue));
                case DbType.Int64:
                    return Random.NextInt64(ConvertUtilities.ChangeType(Minimum, long.MinValue), ConvertUtilities.ChangeType(Maximum, long.MaxValue));
                case DbType.SByte:
                    return Random.NextSByte(ConvertUtilities.ChangeType(Minimum, sbyte.MinValue), ConvertUtilities.ChangeType(Maximum, sbyte.MaxValue));
                case DbType.Single:
                    return Random.NextSingle(ConvertUtilities.ChangeType(Minimum, float.MinValue), ConvertUtilities.ChangeType(Maximum, float.MaxValue));
                case DbType.UInt16:
                    return Random.NextUInt16(ConvertUtilities.ChangeType(Minimum, ushort.MinValue), ConvertUtilities.ChangeType(Maximum, ushort.MaxValue));
                case DbType.UInt32:
                    return Random.NextUInt32(ConvertUtilities.ChangeType(Minimum, uint.MinValue), ConvertUtilities.ChangeType(Maximum, uint.MaxValue));
                case DbType.UInt64:
                    return Random.NextUInt64(ConvertUtilities.ChangeType(Minimum, ulong.MinValue), ConvertUtilities.ChangeType(Maximum, ulong.MaxValue));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
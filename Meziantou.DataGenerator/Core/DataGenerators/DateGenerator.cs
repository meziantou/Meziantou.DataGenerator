using System;
using System.Data;
using System.Linq;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class DateGenerator : DataGenerator
    {
        public DateTime Minimum { get; set; }
        public DateTime Maximum { get; set; }

        public int MinimumOffset { get; set; }
        public int MaximumOffset { get; set; }

        public DateGenerator()
        {
            WellKnownDataType = WellKnownDataType.Date;

            Minimum = new DateTime(1900, 1, 1);
            Maximum = new DateTime(2099, 12, 31);

            MinimumOffset = -12 * 60;
            MaximumOffset = 14 * 60;
        }

        public override int CompareTo(DataGenerator generator)
        {
            if (this.WellKnownDataType == WellKnownDataType.EndDate && (generator.WellKnownDataType == WellKnownDataType.StartDate || generator.WellKnownDataType == WellKnownDataType.Date))
            {
                return 1;
            }
            return base.CompareTo(generator);
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            DbType[] types =
            {
                DbType.Date,
                DbType.DateTime,
                DbType.DateTime2,
                DbType.DateTimeOffset,
                DbType.Time,
            };

            return types.Contains(column.CodeFluentType.DbType);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            DateTime minimum = Minimum;
            if (this.WellKnownDataType == WellKnownDataType.EndDate)
            {
                DateTime dt;
                if (ConvertUtilities.TryChangeType(project.CurrentRow.GetValue(WellKnownDataType.StartDate), out dt))
                {
                    minimum = dt;
                }
                else if (ConvertUtilities.TryChangeType(project.CurrentRow.GetValue(WellKnownDataType.Date), out dt))
                {
                    minimum = dt;
                }
            }

            var dateTime = Random.NextDateTime(minimum, Maximum);
            if (column.CodeFluentType.DbType == DbType.DateTimeOffset)
            {
                var offset = Random.NextInt32(MinimumOffset, MaximumOffset);
                return new DateTimeOffset(dateTime, TimeSpan.FromMinutes(offset));
            }

            return dateTime;
        }
    }
}
using System.Data;
using System.Linq;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class BooleanGenerator : DataGenerator
    {
        public BooleanGenerator()
        {
            WellKnownDataType = WellKnownDataType.Boolean;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            DbType[] types =
            {
                DbType.Boolean
            };

            return types.Contains(column.CodeFluentType.DbType);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            return Random.NextBoolean();
        }
    }
}
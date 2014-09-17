using System.Linq;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class ForeignKeyGenerator : DataGenerator
    {
        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column))
                return false;

            return IsForeginKey(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var table = column.Parent;
            foreach (var foreignKey in table.ForeignKeys)
            {
                Key pk = foreignKey.ReferencedTable.PrimaryKey;
                if (pk == null)
                    continue;

                var fkColumn = pk.Columns.First();

                // Generate value
                var values = project.GetGeneratedValues(fkColumn);
                if (values == null || values.Count == 0)
                    return null;

                int index = Random.NextInt32(0, values.Count);
                return values[index];
            }

            return null;
        }
    }
}
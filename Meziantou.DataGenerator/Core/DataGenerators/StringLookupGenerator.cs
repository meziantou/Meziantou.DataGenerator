using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public abstract class StringLookupGenerator : LookupGenerator
    {
        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            return StringGenerator.CoerceValue(base.GenerateCore(project, column) as string, column);
        }
    }
}
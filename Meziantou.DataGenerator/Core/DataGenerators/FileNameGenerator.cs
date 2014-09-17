using System.IO;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class FileNameGenerator : DataGenerator
    {
        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            return Path.GetRandomFileName();
        }
    }
}
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class FullNameGenerator : DataGenerator
    {
        private readonly DataGenerator _firstNameGenerator = new FirstNameGenerator();
        private readonly DataGenerator _lastNameGenerator = new LastNameGenerator();

        public string Format { get; set; }

        public FullNameGenerator()
        {
            Format = "{0} {1}";
            WellKnownDataType = WellKnownDataType.FullName;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            object firstName = _firstNameGenerator.Generate(project, column, 1, 0);
            object lastName = _lastNameGenerator.Generate(project, column, 1, 0);

            return string.Format(Format, firstName, lastName);
        }
    }
}
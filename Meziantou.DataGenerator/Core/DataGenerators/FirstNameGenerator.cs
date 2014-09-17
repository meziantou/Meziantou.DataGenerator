using System;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class FirstNameGenerator : DataGenerator
    {
        public FirstNameGenerator()
        {
            WellKnownDataType = WellKnownDataType.FirstName;
        }

        public Gender Gender { get; set; }

        public override int CompareTo(DataGenerator generator)
        {
            if (generator.WellKnownDataType == WellKnownDataType.Gender)
            {
                return 1;
            }

            return base.CompareTo(generator);
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var gender = Gender;
            if (gender == Gender.Unknown)
            {
                var columnGender = project.CurrentRow.GetValue(WellKnownDataType.Gender) as string;
                if (columnGender != null)
                {
                    if (string.Equals(columnGender, "Mr", StringComparison.OrdinalIgnoreCase) || 
                        string.Equals(columnGender, "M", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(columnGender, "M.", StringComparison.OrdinalIgnoreCase))
                    {
                        gender = Gender.Male;
                    }
                    else
                    {
                        gender = Gender.Female;
                    }
                }
            }

            switch (gender)
            {
                case Gender.Male:
                    return StringGenerator.CoerceValue(Random.NextFromList(ReferentialData.MaleFirstNames), column);
                case Gender.Female:
                    return StringGenerator.CoerceValue(Random.NextFromList(ReferentialData.FemaleFirstNames), column);
                default:
                    return StringGenerator.CoerceValue(Random.NextFromList(ReferentialData.FirstNames), column);
            }
        }
    }
}
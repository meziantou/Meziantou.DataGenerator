using System;
using System.Collections.Generic;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class CountryGenerator : StringLookupGenerator
    {
        public CountryFormat Format { get; set; }

        public CountryGenerator()
        {
            WellKnownDataType = WellKnownDataType.Country;
        }

        protected override IEnumerable<object> LoadValues()
        {
            return Country.AllCountries;
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var value = base.GenerateCore(project, column) as Country;
            if (value == null)
                return null;

            switch (Format)
            {
                case CountryFormat.TwoletterIsoName:
                    return value.TwoLetterISOName;
                case CountryFormat.NativeName:
                    return value.NativeName;
                case CountryFormat.EnglishName:
                    return value.EnglishName;
                case CountryFormat.SystemName:
                    return value.SystemName;
                case CountryFormat.MixedName:
                    return value.MixedName;
                case CountryFormat.SystemMixedName:
                    return value.SystemMixedName;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
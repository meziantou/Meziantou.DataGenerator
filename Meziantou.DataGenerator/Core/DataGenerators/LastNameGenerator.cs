using System.Collections.Generic;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class LastNameGenerator : StringLookupGenerator
    {
        public LastNameGenerator()
        {
            WellKnownDataType = WellKnownDataType.LastName;
        }

        protected override IEnumerable<object> LoadValues()
        {
            return ReferentialData.LastNames;
        }
    }
}
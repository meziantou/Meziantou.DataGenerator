using System.Collections.Generic;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class UserNameGenerator : StringLookupGenerator
    {
        BrandNameGenerator _brandNameGenerator;
        public UserNameGenerator()
        {
            WellKnownDataType = WellKnownDataType.UserName;
        }

        public override IEnumerable<object> Generate(Project project, Column column, int count, int nullCount)
        {
            // I'm not sure about license when using existing database such as 
            // http://blog.stackoverflow.com/2009/06/stack-overflow-creative-commons-data-dump/
            // so feel free to include a username.txt file in the resource folder. If this file is
            // not provided, the brand name generator will be use.
            if (ReferentialData.UserNames.Count == 0)
            {
                if (_brandNameGenerator == null)
                {
                    _brandNameGenerator = new BrandNameGenerator(); // May looks like username
                    _brandNameGenerator.Seed = this.Seed;
                }

                return _brandNameGenerator.Generate(project, column, count, nullCount);
            }

            return base.Generate(project, column, count, nullCount);
        }

        protected override IEnumerable<object> LoadValues()
        {
            return ReferentialData.UserNames;
        }
    }
}
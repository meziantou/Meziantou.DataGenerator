namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class GenderGenerator : RegexGenerator
    {
        public GenderGenerator()
        {
            WellKnownDataType = WellKnownDataType.Gender;
            RegexPattern = "Miss|Mrs|Ms|Mr";
        }
    }
}
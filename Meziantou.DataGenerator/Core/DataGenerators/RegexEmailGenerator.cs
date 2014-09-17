using System.Text.RegularExpressions;
using CodeFluent.Runtime.Rules;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class RegexEmailGenerator : RegexGenerator
    {
        public RegexEmailGenerator()
        {
            RegexPattern = EmailValidator.DefaultExpression;
            RegexOptions = RegexOptions.IgnoreCase;
        }
    }
}
using System.Linq;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class BrandNameGenerator : DataGenerator
    {
        public BrandNameGenerator()
        {
            WellKnownDataType = WellKnownDataType.BrandName;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var color = new[] { "white", "black", "blue", "green", "silver", "indigo", "gold", "golden" };
            var productFirst = new[] { "span", "dan", "scot", "lam", "new", "hot", "rank", "don", "ding", "dong", "ron", "ran", "an", "round", "re", "fin", "red", "run", "ran", "ap", "can", "don", "dam", "dom", "big", "ice", "drip", "duo", "inch", "fun", "free", "fresh", "grave", "groove", "hay", "hot", "hat", "jay", "joy", "key", "kay", "med", "bio", "geo", "alpha", "k-", "kin", "kon", "konk", "kan", "lot", "lat", "mat", "med", "math", "move", "phys", "bio", "geo", "alpha", "lexi", "beta", "nim", "nam", "open", "ope", "over", "ozer", "onto", "dento", "vento", "quad", "quote", "qvo", "quo", "stat", "stim", "stan", "sub", "sun", "sum", "super", "san", "sao", "sil", "con", "sol", "solo", "sail", "dalt", "salt", "san", "son", "ton", "tin", "tan", "temp", "tam", "tamp", "trans", "tran", "tree", "true", "trio", "trust", "tres", "tris", "u-", "uno", "una", "uni", "viva", "via", "vaia", "voya", "villa", "vila", "vol", "volt", "x-", "xxx-", "tripple", "double", "single", "y-", "year", "good", "hot", "strong", "zen", "s-", "zot", "zath", "zer", "zun", "zon", "zoo", "zone", "zoom", "zam", "zaam", "zim", "zum", "zumma", "fase" };
            var productMiddle = new[] { "dan", "lam", "don", "din", "ron", "ran", "an", "rem", "fin", "red", "run", "ran", "ap", "can", "don", "dam", "dom", "hot", "hat", "jay", "joy", "key", "kay", "lot", "lat", "mat", "to", "nim", "nam", "ove", "oze", "ot", "quad", "qvo", "quo", "stat", "sun", "san", "sao", "sil", "sol", "solo", "sail", "san", "son", "ton", "tin", "tan", "tam", "trax", "dub", "go", "hot", "zen", "zun", "zoz", "zoo", "zam", "zim", "zum" };
            var productLast = new[] { "tex", "tax", "dex", "lux", "ron", "ex", "dox", "dax", "com", "find", "lax", "ity", "fan", "phase", "nix", "ing", "tom", "zap", "lex", "kix", "dom", "tam", "core", "tone", "trax" };
            var productBoth = new[] { "tech", "fix", "fax", "bam", "eco", "fresh", "cof", "soft", "top", "tip", "job", "stock", "string", "strong", "sing", "flex", "plus", "in", "it", "is", "lam", "la", "lab", "light", "air", "touch", "tough", "home", "hold", "warm" };

            var prefix = productFirst.Concat(productLast).Concat(color).ToArray();
            var suffix = productLast.Concat(productBoth).ToArray();
            var middle = productMiddle;

            var prefixName = Random.NextFromArray(prefix);
            var middleName = Random.NextFromArray(middle);
            var suffixName = Random.NextFromArray(suffix);
            int n = Random.NextInt32(0, 9);
            if (n <= 1)
            {
                return ConvertUtilities.Camel(prefixName) + middleName + suffixName;
            }
            if (n <= 2)
            {
                return ConvertUtilities.Camel(prefixName) + "-" + ConvertUtilities.Camel(suffixName);
            }
            if (n <= 3)
            {
                return ConvertUtilities.Camel(prefixName) + suffixName;
            }
            if (n <= 4)
            {
                return ConvertUtilities.Camel(prefixName) + " " + ConvertUtilities.Camel(middleName) + suffixName;
            }
            if (n <= 5)
            {
                return ConvertUtilities.Camel(prefixName) + " " + ConvertUtilities.Camel(suffixName);
            }

            return ConvertUtilities.Camel(prefixName) + suffixName;
        }
    }
}
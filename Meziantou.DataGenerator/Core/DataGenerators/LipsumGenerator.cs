using System.Text;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class LipsumGenerator : DataGenerator
    {
        public int MinimumWords { get; set; }
        public int MaximumWords { get; set; }
        public int MinimumSentences { get; set; }
        public int MaximumSentences { get; set; }
        public int MinimumParagraphs { get; set; }
        public int MaximumParagraphs { get; set; }

        public LipsumGenerator()
        {
            MinimumParagraphs = 3;
            MaximumParagraphs = 20;

            MinimumSentences = 3;
            MaximumSentences = 20;

            MinimumWords = 3;
            MaximumWords = 20;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };
            var ponctuation = new[] { ". ", "! ", "? " };
            StringBuilder sb = new StringBuilder();

            int numParagraphs = Random.NextInt32(MinimumParagraphs, MaximumParagraphs);
            for (int p = 0; p < numParagraphs; p++)
            {
                int numSentences = Random.NextInt32(MinimumSentences, MaximumSentences) + 1;
                for (int s = 0; s < numSentences; s++)
                {
                    int numWords = Random.NextInt32(MinimumWords, MaximumWords);
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0)
                        {
                            sb.Append(" ");
                        }

                        sb.Append(Random.NextFromArray(words));
                    }

                    sb.Append(Random.NextFromArray(ponctuation));
                }

                sb.AppendLine();
            }

            return StringGenerator.CoerceValue(sb.ToString(), column);
        }
    }
}
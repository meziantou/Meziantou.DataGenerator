using System.Collections.Generic;
using System.Text.RegularExpressions;
using CodeFluent.Runtime.Database.Management;
using Rex;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class RegexGenerator : DataGenerator
    {
        private RexEngine _engine;
        private IEnumerator<string> _generator;

        public string RegexPattern { get; set; }
        public RegexOptions RegexOptions { get; set; }
        public CharacterEncoding Encoding { get; set; }

        public RegexGenerator()
        {
            this.Encoding = CharacterEncoding.Unicode;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override void BeforeGenerateValues(Project project, Column column, int count, int nullCount)
        {
            CharacterEncoding encoding = Encoding;
            if (!IsUnicode(column))
            {
                encoding = CharacterEncoding.ASCII;
            }

            _engine = new RexEngine(encoding, -1);
            _generator = _engine.GenerateMembers(RegexOptions, count, RegexPattern).GetEnumerator();
        }

        protected override void AfterGenerateValues(Project project, Column column, int count, int nullCount)
        {
            if (_generator != null)
            {
                _generator.Dispose();
                _generator = null;
            }

            _engine = null;
        }

        protected override object GenerateCore(Project project, Column column)
        {
            if (!_generator.MoveNext())
                return null;

            var value = _generator.Current;

            return value;
        }
    }
}
using System;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class StringGenerator : DataGenerator
    {
        public int MaximumLength { get; set; }
        public int MinimumLength { get; set; }
        public CharacterSet CharacterSet { get; set; }
        public string Characters { get; set; }

        public StringGenerator()
        {
            MinimumLength = 0;
            MaximumLength = int.MaxValue;
            CharacterSet = CharacterSet.Unicode; // CharacterSet.LowerAlpha | CharacterSet.UpperAlpha | CharacterSet.Digits;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            if ((CharacterSet & CharacterSet.Unicode) == CharacterSet.Unicode)
            {
                if (!column.CodeFluentType.IsDbUnicode)
                {
                    return Random.NextUnicodeString(MinimumLength, column.MaxLength > 0 ? Math.Min(column.MaxLength, MaximumLength) : MaximumLength, new RandomUtilities.Range(0x0, 0xFF));
                }

                return Random.NextUnicodeString(MinimumLength, column.MaxLength > 0 ? Math.Min(column.MaxLength, MaximumLength) : MaximumLength);
            }

            if ((CharacterSet & CharacterSet.Custom) == CharacterSet.Custom)
            {
                return Random.NextString(MinimumLength, column.MaxLength > 0 ? Math.Min(column.MaxLength, MaximumLength) : MaximumLength, Characters);
            }

            string s = null;
            if ((CharacterSet & CharacterSet.Digits) == CharacterSet.Digits)
            {
                s += RandomUtilities.Digits;
            }
            if ((CharacterSet & CharacterSet.LowerAlpha) == CharacterSet.LowerAlpha)
            {
                s += RandomUtilities.LowerAlpha;
            }
            if ((CharacterSet & CharacterSet.UpperAlpha) == CharacterSet.UpperAlpha)
            {
                s += RandomUtilities.UpperAlpha;
            }

            return Random.NextString(MinimumLength, column.MaxLength > 0 ? Math.Min(column.MaxLength, MaximumLength) : MaximumLength, s);
        }

        public static string CoerceValue(string value, Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            if (value != null && value.Length > column.MaxLength)
            {
                value = value.Substring(0, column.MaxLength);
            }

            return value;
        }
    }
}
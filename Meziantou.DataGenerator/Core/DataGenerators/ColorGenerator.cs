using System;
using System.Linq;
using System.Windows.Media;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class ColorGenerator : DataGenerator
    {
        public ColorFormat Format { get; set; }

        public ColorGenerator()
        {
            WellKnownDataType = WellKnownDataType.Color;
            Format = ColorFormat.Auto;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        private string[] _colors;

        protected override object GenerateCore(Project project, Column column)
        {
            var format = Format;
            if (format == ColorFormat.Auto)
            {
                if (IsNumericDbType(column))
                {
                    format = ColorFormat.IntegerARGB;
                }
                else
                {
                    format = ColorFormat.RGB;
                }
            }

            switch (format)
            {
                case ColorFormat.Name:
                    if (_colors == null)
                    {
                        _colors = typeof(Colors).GetProperties().Where(_ => typeof(Color).IsAssignableFrom(_.PropertyType)).Select(_ => _.Name).ToArray();
                    }
                    return Random.NextFromArray(_colors);
                case ColorFormat.IntegerRGB:
                    return Random.NextInt32(0, 1 << 24);
                case ColorFormat.IntegerARGB:
                    return Random.NextInt32();
                case ColorFormat.RGB:
                    return ConvertUtilities.ToHexa(new[] { Random.NextByte(), Random.NextByte(), Random.NextByte() });
                case ColorFormat.HtmlRGB:
                    return "#" + ConvertUtilities.ToHexa(new[] { Random.NextByte(), Random.NextByte(), Random.NextByte() });
                case ColorFormat.ARGB:
                    return ConvertUtilities.ToHexa(new[] { Random.NextByte(), Random.NextByte(), Random.NextByte(), Random.NextByte() });
                case ColorFormat.HtmlARGB:
                    return "#" + ConvertUtilities.ToHexa(new[] { Random.NextByte(), Random.NextByte(), Random.NextByte(), Random.NextByte() });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
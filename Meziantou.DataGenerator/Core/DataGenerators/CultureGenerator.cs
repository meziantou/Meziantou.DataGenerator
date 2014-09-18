using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class CultureGenerator : LookupGenerator
    {
        public CultureTypes CultureTypes { get; set; }
        public CultureFormat CultureFormat { get; set; }

        public CultureGenerator()
        {
            CultureTypes = CultureTypes.AllCultures;
            CultureFormat = CultureFormat.Auto;
        }

        public override void Configure(XmlElement element)
        {
            base.Configure(element);
            CultureTypes = XmlUtilities.GetAttribute(element, "cultureTypes", CultureTypes);
            CultureFormat = XmlUtilities.GetAttribute(element, "cultureFormat", CultureFormat);
        }

        protected override IEnumerable<object> LoadValues()
        {
            return CultureInfo.GetCultures(CultureTypes);
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return true;
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var value = base.GenerateCore(project, column) as CultureInfo;
            if (value == null)
                return null;

            var cultureFormat = CultureFormat;
            if (cultureFormat == CultureFormat.Auto)
            {
                if (IsNumericDbType(column))
                {
                    cultureFormat = CultureFormat.Lcid;
                }
                else
                {
                    if (column.MaxLength == 2)
                    {
                        cultureFormat = CultureFormat.TwoLetterISOLanguageName;
                    }
                    else if (column.MaxLength == 3)
                    {
                        cultureFormat = CultureFormat.ThreeLetterISOLanguageName;
                    }
                    else
                    {
                        cultureFormat = CultureFormat.Name;
                    }
                }
            }

            switch (cultureFormat)
            {
                case CultureFormat.Lcid:
                    return value.LCID;
                case CultureFormat.Name:
                    return value.Name;
                case CultureFormat.EnglishName:
                    return value.EnglishName;
                case CultureFormat.NativeName:
                    return value.NativeName;
                case CultureFormat.DisplayName:
                    return value.DisplayName;
                case CultureFormat.TwoLetterISOLanguageName:
                    return value.TwoLetterISOLanguageName;
                case CultureFormat.ThreeLetterISOLanguageName:
                    return value.ThreeLetterISOLanguageName;
                case CultureFormat.ThreeLetterWindowsLanguageName:
                    return value.ThreeLetterWindowsLanguageName;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
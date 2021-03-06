using System.Collections.Generic;
using System.Text;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class PhoneNumberGenerator : DataGenerator
    {
        public IList<PhoneFormat> Formats { get; private set; }

        public PhoneNumberGenerator()
        {
            Formats = new List<PhoneFormat>();
            Formats.Add(new PhoneFormat("French", "00 00 00 00 00"));
            Formats.Add(new PhoneFormat("French International", "+33 0 00 00 00 00"));
            Formats.Add(new PhoneFormat("US", "000-000-0000"));
            Formats.Add(new PhoneFormat("US", "+1 (000) 000-0000"));
            Formats.Add(new PhoneFormat("US", "(000) 000-0000"));
        }

        public override void Configure(XmlElement element)
        {
            base.Configure(element);
            string s = XmlUtilities.GetAttribute(element, "formats", (string) null);
            if (s != null)
            {
                var formats = s.Split('|');
                foreach (var format in formats)
                {
                    Formats.Add(new PhoneFormat(format, format));
                }
            }
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            var phoneFormat = Random.NextFromList(Formats);
            StringBuilder sb = new StringBuilder();
            foreach (var c in phoneFormat.Format)
            {
                if (c == '0')
                {
                    sb.Append(Random.NextInt32(0, 10));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return StringGenerator.CoerceValue(sb.ToString(), column);
        }
    }
}
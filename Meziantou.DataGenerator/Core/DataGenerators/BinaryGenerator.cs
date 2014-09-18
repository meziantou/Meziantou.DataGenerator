using System;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class BinaryGenerator : DataGenerator
    {
        public int MinimumLength { get; set; }
        public int MaximumLength { get; set; }

        public BinaryGenerator()
        {
            MaximumLength = 10 * 1024 * 1024 /* 10MB */;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsBinaryDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            int maxLength = MaximumLength;
            if (column.MaxLength > 0)
            {
                maxLength = Math.Min(column.MaxLength, maxLength);
            }

            int length = Random.NextInt32(MinimumLength, maxLength);

            byte[] bytes = new byte[length];
            Random.NextBytes(bytes);
            return bytes;
        }

        public override void Configure(XmlElement element)
        {
            base.Configure(element);

            MinimumLength = XmlUtilities.GetAttribute(element, "minimumLength", MinimumLength);
            MaximumLength = XmlUtilities.GetAttribute(element, "maximumLength", MaximumLength);
        }
    }
}
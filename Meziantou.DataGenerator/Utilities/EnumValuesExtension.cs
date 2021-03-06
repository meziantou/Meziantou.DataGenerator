﻿using System;
using System.Linq;
using System.Windows.Markup;

namespace Meziantou.DataGenerator.Utilities
{
    [MarkupExtensionReturnType(typeof(object[]))]
    public class EnumValuesExtension : MarkupExtension
    {
        public EnumValuesExtension()
        {
        }

        public EnumValuesExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType { get; set; }

        public bool SortByName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.EnumType == null)
                throw new ArgumentException("The enum type is not set");
            var values = Enum.GetValues(this.EnumType);
            if (SortByName)
            {
                var list = values.Cast<Enum>().ToList();
                list.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
                return list;
            }

            return values;
        }
    }
}
// <copyright file="CsvConverter.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.PropertyPageProxies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    public class CsvConverter : TypeConverter
    {
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            List<string> v = value as List<string>;
            if (destinationType == typeof(string))
            {
                return string.Join(",", v.ToArray());
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
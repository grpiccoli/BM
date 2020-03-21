using BiblioMit.Models;
using System;
using System.Globalization;

namespace BiblioMit.Services
{
    public class InterceptProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object obj, IFormatProvider provider)
        {
            // Display information about method call.
            if (!Equals(provider))
                return null;

            // Set default format specifier             
            if (string.IsNullOrEmpty(format))
                format = "N";

            //string numericString = obj.ToString();

            if (obj is int && format.Equals("U", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0,9:N0}-{1}", obj, StringManipulations.GetDigit((int)obj));
            }

            // If this is a byte and the "R" format string, format it with Roman numerals.
            if (obj is int && format.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                return StringManipulations.ToRomanNumeral((int)obj);
            }

            // Use default for all other formatting.
            if (obj is IFormattable)
                return ((IFormattable)obj).ToString(format, CultureInfo.CurrentCulture);
            else
                return obj?.ToString();
        }
    }
}

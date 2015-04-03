using System;
using System.Globalization;
using CsvHelper.TypeConversion;

namespace Erbsenzaehler.Importer.Converters
{
    public class InvariantAmountConverter : ITypeConverter
    {
        public string ConvertToString(TypeConverterOptions options, object value)
        {
            return value.ToString();
        }


        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            try
            {
                if (text.Contains(",") && text.Length <= 6)
                {
                    return decimal.Parse(text.Replace(".", "").Replace(" ", "").Replace("€", ""), new CultureInfo("de-DE"));
                }
                return decimal.Parse(text.Replace(",", "").Replace(" ", "").Replace("€", ""), new CultureInfo("en-US"));
            }
            catch
            {
                throw new Exception("Unable to parse amount '" + text + "'.");
            }
        }


        public bool CanConvertFrom(Type type)
        {
            return type == typeof (string);
        }


        public bool CanConvertTo(Type type)
        {
            return type == typeof (string);
        }
    }
}
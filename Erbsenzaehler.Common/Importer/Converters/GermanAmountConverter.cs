using System;
using System.Globalization;
using CsvHelper.TypeConversion;

namespace Erbsenzaehler.Importer.Converters
{
    public class GermanAmountConverter : ITypeConverter
    {
        public string ConvertToString(TypeConverterOptions options, object value)
        {
            return value.ToString();
        }


        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            try
            {
                return decimal.Parse(text.Replace(".", "").Replace(" ", "").Replace("€", ""), new CultureInfo("de-DE"));
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
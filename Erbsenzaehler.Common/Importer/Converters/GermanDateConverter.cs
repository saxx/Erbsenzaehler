using System;
using System.Globalization;
using CsvHelper.TypeConversion;

namespace Erbsenzaehler.Importer.Converters
{
    public class GermanDateConverter : ITypeConverter
    {
        public string ConvertToString(TypeConverterOptions options, object value)
        {
            return ((DateTime) value).ToShortDateString();
        }


        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            DateTime result;

            if (DateTime.TryParse(text, new CultureInfo("de-DE"), DateTimeStyles.None, out result))
            {
                return result;
            }

            throw new Exception("Unable to parse date '" + text + "'.");
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
using System;
using CsvHelper.TypeConversion;
using Erbsenzaehler.ExtensionMethods;

namespace Erbsenzaehler.Importer.Converters
{
    public class TextConverter : ITypeConverter
    {
        public string ConvertToString(TypeConverterOptions options, object value)
        {
            return value as string;
        }


        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            return text.RemoveCrlf().RemoveMultipleBlanks().Trim();
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
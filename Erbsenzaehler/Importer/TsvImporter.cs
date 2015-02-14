using System;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Erbsenzaehler.ExtensionMethods;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Importer
{
    public sealed class TsvImporter : ImporterBase
    {
        public TsvImporter(TextReader reader) : base(reader)
        {
            Configuration.RegisterClassMap<LineMap>();
            Configuration.HasHeaderRecord = false;
            Configuration.Delimiter = "\t";
            Configuration.Encoding = Encoding.Unicode;
        }


        public sealed class LineMap : CsvClassMap<Line>
        {
            public LineMap()
            {
                Map(x => x.OriginalText).Index(0).TypeConverter<TextConverter>();
                Map(x => x.OriginalDate).Index(1).TypeConverter<DateConverter>();
                Map(x => x.OriginalAmount).Index(2).TypeConverter<AmountConverter>();
            }


            public class AmountConverter : ITypeConverter
            {
                public string ConvertToString(TypeConverterOptions options, object value)
                {
                    return value.ToString();
                }


                public object ConvertFromString(TypeConverterOptions options, string text)
                {
                    return decimal.Parse(text.Replace(" ", "").Replace("€", ""), new CultureInfo("de-DE"));
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

            public class DateConverter : ITypeConverter
            {
                public string ConvertToString(TypeConverterOptions options, object value)
                {
                    return ((DateTime) value).ToShortDateString();
                }


                public object ConvertFromString(TypeConverterOptions options, string text)
                {
                    if (text.Contains("/"))
                    {
                        return DateTime.Parse(text, new CultureInfo("en-US"));
                    }
                    return DateTime.Parse(text, new CultureInfo("de-DE"));
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
    }
}
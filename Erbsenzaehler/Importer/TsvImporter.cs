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
            Configuration.HasHeaderRecord = true;
            Configuration.Delimiter = "\t";
            Configuration.Encoding = Encoding.Unicode;
        }


        public sealed class LineMap : CsvClassMap<Line>
        {
            public LineMap()
            {
                Map(x => x.OriginalText).Name("Text", "OriginalText", "Buchungstext").TypeConverter<TextConverter>();
                Map(x => x.OriginalDate).Name("Date", "OriginalDate", "Datum", "Buchungsdatum").TypeConverter<DateConverter>();
                Map(x => x.OriginalAmount).Name("Amount", "OriginalAmount", "Betrag").TypeConverter<AmountConverter>();
            }


            public class AmountConverter : ITypeConverter
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
                            return decimal.Parse(text.Replace(".", "").Replace(" ", "").Replace("€", ""), new CultureInfo("de-DE"));
                        return decimal.Parse(text.Replace(",", "").Replace(" ", "").Replace("€", ""), new CultureInfo("en-US"));
                    }
                    catch
                    {
                        throw new Exception("Unable to parse amount '" + text + "'.");
                    }
                }


                public bool CanConvertFrom(Type type)
                {
                    return type == typeof(string);
                }


                public bool CanConvertTo(Type type)
                {
                    return type == typeof(string);
                }
            }

            public class DateConverter : ITypeConverter
            {
                public string ConvertToString(TypeConverterOptions options, object value)
                {
                    return ((DateTime)value).ToShortDateString();
                }


                public object ConvertFromString(TypeConverterOptions options, string text)
                {
                    DateTime result;

                    if (text.Contains("/") && DateTime.TryParse(text, new CultureInfo("en-US"), DateTimeStyles.None, out result))
                    {
                        return result;
                    }

                    if (DateTime.TryParse(text, new CultureInfo("de-DE"), DateTimeStyles.None, out result))
                    {
                        return result;
                    }

                    if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    {
                        return result;
                    }

                    throw new Exception("Unable to parse date '" + text + "'.");
                }


                public bool CanConvertFrom(Type type)
                {
                    return type == typeof(string);
                }


                public bool CanConvertTo(Type type)
                {
                    return type == typeof(string);
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
                    return type == typeof(string);
                }


                public bool CanConvertTo(Type type)
                {
                    return type == typeof(string);
                }
            }
        }
    }
}
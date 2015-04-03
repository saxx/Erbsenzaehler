using System.IO;
using System.Text;
using CsvHelper.Configuration;
using Erbsenzaehler.Importer.Converters;
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
                Map(x => x.OriginalDate).Name("Date", "OriginalDate", "Datum", "Buchungsdatum").TypeConverter<InvariantDateConverter>();
                Map(x => x.OriginalAmount).Name("Amount", "OriginalAmount", "Betrag").TypeConverter<InvariantAmountConverter>();
            }
        }
    }
}
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using Erbsenzaehler.Importer.Converters;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Importer
{
    public sealed class EasybankImporter : ImporterBase
    {
        public EasybankImporter(TextReader reader) : base(reader)
        {
            Configuration.RegisterClassMap<LineMap>();
            Configuration.HasHeaderRecord = false;
            Configuration.Delimiter = ";";
            Configuration.Encoding = Encoding.Default;
        }


        public sealed class LineMap : CsvClassMap<Line>
        {
            public LineMap()
            {
                Map(x => x.OriginalText).Index(1).TypeConverter<TextConverter>();
                Map(x => x.OriginalDate).Index(2).TypeConverter<GermanDateConverter>();
                Map(x => x.OriginalAmount).Index(4).TypeConverter<GermanAmountConverter>();
            }
        }
    }
}
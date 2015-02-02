using System;
using System.IO;

namespace Erbsenzaehler.Importer
{
    public class ImporterFactory
    {
        public ImporterBase GetImporter(TextReader reader, ImporterType importer)
        {
            switch (importer)
            {
                case ImporterType.Easybank:
                    return new EasybankImporter(reader);
                case ImporterType.Tsv:
                    return new TsvImporter(reader);
                default:
                    throw new ApplicationException("Unknown importer '" + importer + "'.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.Import
{
    public class ImportLogsViewModel
    {
        #region Constructor

        private readonly Db _db;


        public ImportLogsViewModel(Db db)
        {
            _db = db;
        }

        #endregion

        public async Task<ImportLogsViewModel> Fill(Client client)
        {
            var lastImports = await _db.ImportLog
                .ByClient(client)
                .OrderByDescending(x => x.Date)
                .Take(100)
                .Select(x => new
                {
                    AccountName = x.Account.Name,
                    ImportLog = x
                })
                .ToListAsync();

            ImportLogs = lastImports
                .Select(x => new ImportLog
                {
                    Date = x.ImportLog.Date,
                    Account = x.AccountName,
                    DuplicateLines = x.ImportLog.LinesDuplicatesCount,
                    ImportedLines = x.ImportLog.LinesImportedCount,
                    TotalLines = x.ImportLog.LinesFoundCount,
                    ErrorMessage = x.ImportLog.Log,
                    Type = GetTypeName(x.ImportLog.Type),
                    DurationInMilliseconds = x.ImportLog.Milliseconds
                });

            return this;
        }


        private string GetTypeName(ImportLogType type)
        {
            switch (type)
            {
                case ImportLogType.Manual:
                    return "Manueller Import";
                case ImportLogType.AutomaticOnServer:
                    return "Automatisierter Import";
                case ImportLogType.AutomaticOnClient:
                    return "Import über Ihren PC";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public IEnumerable<ImportLog> ImportLogs { get; set; }

        public class ImportLog
        {
            public DateTime Date { get; set; }
            public string Type { get; set; }
            public string Account { get; set; }
            public int TotalLines { get; set; }
            public int DuplicateLines { get; set; }
            public int ImportedLines { get; set; }
            public int DurationInMilliseconds { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
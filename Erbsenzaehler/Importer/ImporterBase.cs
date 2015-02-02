using System.IO;
using CsvHelper;

namespace Erbsenzaehler.Importer
{
    public abstract class ImporterBase : CsvReader
    {
        protected ImporterBase(TextReader reader) : base(reader)
        {
        }


        public int LoadFileAndImport(Stream stream)
        {
            //var count = 0;
            //var lines = GetRecords<Line>();

            //var categoryService = new FindCategoryService();
            //var allCategories = await db.Categories.Where(x => x.ClientId == client.Id && x.Regex != null && x.Regex.Length > 0).ToListAsync();

            //foreach (var line in lines)
            //{
                /*var lineExists = db.Lines.Any(x => x.ClientId == client.Id && x.OriginalDate == line.OriginalDate && x.Text == line.Text && x.Amount == line.Amount && x.AccountId == account.Id);
                    if (!lineExists)
                    {
                        line.Client = client;
                        line.Account = account;
                        line.CreationDateUtc = DateTime.UtcNow;
                        categoryService.FindCategoryForLine(line, client, db, allCategories);

                        db.Lines.Add(line);
                        await db.SaveChangesAsync();
                        count++;
                    }*/
            //    count++;
            //}

            return 0;
        }
    }
}
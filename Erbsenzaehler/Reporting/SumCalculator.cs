using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Reporting
{
    public class SumCalculator
    {
        private readonly Client _client;
        private readonly Db _db;

        public SumCalculator(Db db, Client client)
        {
            _db = db;
            _client = client;
        }

        public async Task<IDictionary<string, decimal>> CalculateForMonth(Month month)
        {
            var lines = await (from x in _db.Lines.ByClient(_client).ByMonth(month)
                               where !x.Ignore
                               select new
                               {
                                   x.Category,
                                   Amount = x.Amount ?? x.OriginalAmount
                               }).ToListAsync();

            var result = new Dictionary<string, decimal>();
            foreach (var line in lines)
            {
                var category = line.Category ?? (line.Amount > 0 ? Constants.IncomeCategory : Constants.EmptyCategory);
                if (!result.ContainsKey(category))
                {
                    result[category] = 0;
                }
                result[category] += line.Amount;
            }

            // now, remove all categories > 0 and combine them into the income category
            foreach (var pair in result.Where(x => x.Value > 0 && x.Key != Constants.IncomeCategory).ToList())
            {
                if (!result.ContainsKey(Constants.IncomeCategory))
                {
                    result[Constants.IncomeCategory] = 0;
                }
                result[Constants.IncomeCategory] += pair.Value;
                result.Remove(pair.Key);
            }

            return result;
        }
    }
}
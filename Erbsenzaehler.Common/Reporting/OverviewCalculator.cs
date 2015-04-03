using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Reporting
{
    public class OverviewCalculator
    {
        private readonly Db _db;
        private readonly Client _client;
        private readonly SumCalculator _sumCalculator;


        public OverviewCalculator(Db db, Client client, SumCalculator sumCalculator)
        {
            _sumCalculator = sumCalculator;
            _client = client;
            _db = db;
        }


        public async Task<IEnumerable<string>> GetCategories(bool includeIncome = false)
        {
            var allCategories = await _db.Lines.Where(x => x.Account.ClientId == _client.Id && x.Category != null).Select(x => x.Category).Distinct().ToListAsync();
            allCategories.Add(Constants.EmptyCategory);
            if (includeIncome)
            {
                allCategories.Add(Constants.IncomeCategory);
            }
            return allCategories;
        }


        public async Task<IDictionary<Month, IDictionary<string, decimal>>> Calculate()
        {
            var result = new Dictionary<Month, IDictionary<string, decimal>>();

            // for some reason it's dreadfully slow to do all the grouping in the database
            // it's much, much faster to do it in memory, even if this means that we require more memory for a short period of time
            // but I think it shouldn't matter that much because no client should have more than a few thousand lines

            var lines = await (_db.Lines.ByClient(_client).ByNotIgnored().ToListAsync());

            if (!lines.Any())
            {
                return result;
            }

            var minDate = lines.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().Min();
            var maxDate = lines.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().Max();

            var month = new Month(minDate);

            while (month.Date <= maxDate)
            {
                result[month] = _sumCalculator.CalculateForMonth(lines, month);
                month = new Month(month.Date.AddMonths(1));
            }

            return result;
        }
    }
}
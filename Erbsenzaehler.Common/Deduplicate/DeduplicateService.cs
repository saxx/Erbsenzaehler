using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erbsenzaehler.ExtensionMethods;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Deduplicate
{
    public class DeduplicateService
    {
        private readonly Db _db;


        public DeduplicateService(Db db)
        {
            _db = db;
        }


        public async Task<IEnumerable<Duplicate>> FindPossibleDuplicates(int accountId, int maxDistance)
        {
            return await FindDuplicates(accountId, (s, t) => TextIsSimilar(s, t, maxDistance));
        }


        public async Task<IEnumerable<Duplicate>> FindExactDuplicates(int accountId, bool treatSpaceAsPipe)
        {
            return await FindDuplicates(accountId, TextIsIdentical);
        }


        private async Task<IEnumerable<Duplicate>> FindDuplicates(int accountId, Func<string, string, bool> textCompareFunction)
        {
            var result = new List<Duplicate>();

            foreach (var line in _db.Lines.ByAccount(accountId).OrderByDescending(x => x.Id))
            {
                // the line was already identified as a duplicate, s lets ignore it
                if (result.Any(x => x.Duplicates.Any(y => y.Id == line.Id)))
                {
                    continue;
                }

                var possibleDuplicates =
                    await _db.Lines.Where(x => x.AccountId == accountId && x.OriginalAmount == line.OriginalAmount && x.OriginalDate == line.OriginalDate && x.Id != line.Id).ToListAsync();
                possibleDuplicates = possibleDuplicates.Where(x => textCompareFunction.Invoke(line.OriginalText, x.OriginalText)).ToList();
                if (possibleDuplicates.Any())
                {
                    result.Add(new Duplicate
                    {
                        Original = line,
                        Duplicates = possibleDuplicates
                    });
                }
            }

            return result;
        }


        private bool TextIsIdentical(string s, string t)
        {

            s = s.Replace('|', ' ');
            t = t.Replace('|', ' ');

            return s.Equals(t, StringComparison.CurrentCultureIgnoreCase) || TextIsIdenticalEasybankSpecials(s, t);
        }

        // easybank has some fucked up way to change the text of lines
        private bool TextIsIdenticalEasybankSpecials(string s, string t)
        {
            var fixedS = s.Replace("Bezahlung Maestro", "Bezahlung Karte");
            var fixedT = t.Replace("Bezahlung Maestro", "Bezahlung Karte");
            fixedS = fixedS.Replace("Auszahlung Maestro", "Auszahlung Karte");
            fixedT = fixedT.Replace("Auszahlung Maestro", "Auszahlung Karte");
            var result = fixedS.Equals(fixedT, StringComparison.CurrentCultureIgnoreCase);
            if (result)
            {
                return true;
            }

            fixedS = fixedS.Replace(" EASYATW1XXX ", " ");
            fixedT = fixedT.Replace(" EASYATW1XXX ", " ");
            result = fixedS.Equals(fixedT, StringComparison.CurrentCultureIgnoreCase);
            if (result)
            {
                return true;
            }

            fixedS = Regex.Replace(fixedS, @" Karte (M )?\d\d\.\d\d MC", " Karte MC");
            fixedT = Regex.Replace(fixedT, @" Karte (M )?\d\d\.\d\d MC", " Karte MC");
            result = fixedS.Equals(fixedT, StringComparison.CurrentCultureIgnoreCase);
            if (result)
            {
                return true;
            }

            fixedS = Regex.Replace(fixedS, @" AT\d\d(\d{5})(\d{11})$", " $1 $2");
            fixedT = Regex.Replace(fixedT, @" AT\d\d(\d{5})(\d{11})$", " $1 $2");
            result = fixedS.Equals(fixedT, StringComparison.CurrentCultureIgnoreCase);
            if (result)
            {
                return true;
            }

            fixedS = Regex.Replace(fixedS, @"^Abbuchung Einzugsermächtigung ", "");
            fixedT = Regex.Replace(fixedT, @"^Abbuchung Einzugsermächtigung ", "");
            fixedS = Regex.Replace(fixedS, @"^Gutschrift Überweisung ", "");
            fixedT = Regex.Replace(fixedT, @"^Gutschrift Überweisung ", "");
            return fixedS.EndsWith(fixedT, StringComparison.CurrentCultureIgnoreCase) || fixedT.EndsWith(fixedS, StringComparison.CurrentCultureIgnoreCase);
        }


        private bool TextIsSimilar(string s, string t, int maxDistance)
        {
            return s.DamerauLevenshteinDistanceTo(t) <= maxDistance;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Rules
{
    public class RulesApplier
    {
        public async Task<RulesApplierResult> Apply(Db db, Client client, IEnumerable<Line> lines, bool resetFirst)
        {
            var result = new RulesApplierResult();
            foreach (var line in lines)
            {
                if (resetFirst)
                {
                    if (!line.IgnoreUpdatedManually)
                    {
                        line.Ignore = false;
                    }

                    if (!line.DateUpdatedManually)
                    {
                        line.Date = line.OriginalDate;
                    }

                    if (!line.CategoryUpdatedManually)
                    {
                        line.Category = null;
                    }
                }

                if (await Apply(db, client, line))
                {
                    result.LinesUpdated++;
                }
            }
            return result;
        }


        public async Task<bool> Apply(Db db, Client client, Line line)
        {
            var result = false;
            var rules = await LoadRules(db, client);
            foreach (var rule in rules)
            {
                foreach (var regex in rule.Regex.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
                {
                    if (Regex.IsMatch(line.OriginalText, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                    {
                        if (rule.ChangeCategoryTo != null && !line.CategoryUpdatedManually)
                        {
                            line.Category = string.IsNullOrEmpty(rule.ChangeCategoryTo) ? null : rule.ChangeCategoryTo;
                        }

                        if (rule.ChangeIgnoreTo.HasValue && !line.IgnoreUpdatedManually)
                        {
                            line.Ignore = rule.ChangeIgnoreTo.Value;
                        }

                        if (rule.ChangeDateTo.HasValue && !line.DateUpdatedManually)
                        {
                            switch (rule.ChangeDateTo.Value)
                            {
                                case Rule.ChangeDateToOption.FirstOfCurrentMonth:
                                    line.Date = new DateTime(line.OriginalDate.Year, line.OriginalDate.Month, 1);
                                    break;
                                case Rule.ChangeDateToOption.LastOfCurrentMonth:
                                    line.Date = new DateTime(line.OriginalDate.Year, line.OriginalDate.Month, 1).AddMonths(1).AddDays(-1);
                                    break;
                                case Rule.ChangeDateToOption.NearestFirstOfMonth:
                                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                                    if (line.OriginalDate.Day > 15)
                                    {
                                        line.Date = new DateTime(line.OriginalDate.Year, line.OriginalDate.Month, 1).AddMonths(1);
                                    }
                                    else
                                    {
                                        line.Date = new DateTime(line.OriginalDate.Year, line.OriginalDate.Month, 1);
                                    }
                                    break;
                                case Rule.ChangeDateToOption.NearestLastOfMonth:
                                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                                    if (line.OriginalDate.Day > 15)
                                    {
                                        line.Date = new DateTime(line.OriginalDate.Year, line.OriginalDate.Month, 1).AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        line.Date = new DateTime(line.OriginalDate.Year, line.OriginalDate.Month, 1).AddDays(-1);
                                    }
                                    break;
                            }
                        }

                        result = true;
                    }
                }
            }
            return result;
        }


        private IEnumerable<Rule> _rulesCache;


        private async Task<IEnumerable<Rule>> LoadRules(Db db, Client client)
        {
            return _rulesCache ?? (_rulesCache = await db.Rules.Where(x => x.ClientId == client.Id).ToListAsync());
        }
    }
}
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
                    line.Ignore = false;
                    line.Date = line.OriginalDate;
                    line.Category = null;
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
                if (Regex.IsMatch(line.OriginalText, rule.Regex, RegexOptions.IgnoreCase))
                {
                    if (rule.ChangeCategoryTo != null)
                    {
                        line.Category = string.IsNullOrEmpty(rule.ChangeCategoryTo) ? null : rule.ChangeCategoryTo;
                    }

                    if (rule.ChangeIgnoreTo.HasValue)
                    {
                        line.Ignore = rule.ChangeIgnoreTo.Value;
                    }

                    if (rule.ChangeDateTo.HasValue)
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
            return result;
        }


        private IEnumerable<Rule> _rulesCache;
        private async Task<IEnumerable<Rule>> LoadRules(Db db, Client client)
        {
            if (_rulesCache == null)
                _rulesCache = await db.Rules.Where(x => x.ClientId == client.Id).ToListAsync();
            return _rulesCache;
        }
    }
}
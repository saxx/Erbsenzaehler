﻿using System;
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
        public async Task<RulesApplierResult> Apply(Db db, int clientId, IEnumerable<Line> lines, bool resetFirst)
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

                    if (!line.AmountUpdatedManually)
                    {
                        line.Amount = null;
                    }
                }

                if (await Apply(db, clientId, line))
                {
                    result.LinesUpdated++;
                }
            }
            return result;
        }


        public async Task<bool> Apply(Db db, int clientId, Line line)
        {
            var result = false;
            var rules = await LoadRules(db, clientId);
            foreach (var rule in rules)
            {
                if (IsMatch(rule, line.Text ?? line.OriginalText))
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
            return result;
        }


        public bool IsMatch(Rule rule, string lineText)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var regex in (rule.Regex ?? "").Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
            {
                if (Regex.IsMatch(lineText, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    return true;
                }
            }
            return false;
        }


        private
            IEnumerable<Rule> _rulesCache;


        private async Task<IEnumerable<Rule>> LoadRules(Db db, int clientId)
        {
            return _rulesCache ?? (_rulesCache = await db.Rules.Where(x => x.ClientId == clientId).ToListAsync());
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using Erbsenzaehler.Models;
using Newtonsoft.Json;

namespace Erbsenzaehler.Rules
{
    public class RulesImporter
    {
        public async Task<RulesImporterResult> Import(Db db, Client client, string jsonString)
        {
            var result = new RulesImporterResult();

            var rulesFromJson = JsonConvert.DeserializeObject<IEnumerable<Rule>>(jsonString).ToList();
            foreach (var ruleFromJson in rulesFromJson)
            {
                var ruleInDatabase = db.Rules.FirstOrDefault(x => x.ClientId == client.Id && x.Id == ruleFromJson.Id);
                if (ruleInDatabase == null)
                {
                    ruleInDatabase = new Rule
                    {
                        ClientId = client.Id
                    };
                    db.Rules.Add(ruleInDatabase);
                    result.AddedRulesCount++;
                }
                else
                {
                    result.UpdatedRulesCount++;
                }

                ruleInDatabase.ChangeCategoryTo = ruleFromJson.ChangeCategoryTo;
                ruleInDatabase.ChangeDateTo = ruleFromJson.ChangeDateTo;
                ruleInDatabase.ChangeIgnoreTo = ruleFromJson.ChangeIgnoreTo;
                ruleInDatabase.Regex = ruleFromJson.Regex;
            }

            var ruleIdNotToDelete = rulesFromJson.Select(x => x.Id).ToList();
            var rulesToDelete = db.Rules.Where(x => x.ClientId == client.Id && !ruleIdNotToDelete.Contains(x.Id)).ToList();
            db.Rules.RemoveRange(rulesToDelete);
            result.DeletedRulesCount = rulesToDelete.Count;

            await db.SaveChangesAsync();
            return result;
        }
    }
}
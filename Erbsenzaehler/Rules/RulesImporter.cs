using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            // first, delete all existing rules for the client
            var rulesToDelete = db.Rules.Where(x => x.ClientId == client.Id);
            if (rulesToDelete.Any())
            {
                db.Rules.RemoveRange(rulesToDelete);
                await db.SaveChangesAsync();
            }

            // then, add the rules from the JSON
            foreach (var ruleFromJson in rulesFromJson)
            {
                var ruleInDatabase = new Rule
                {
                    ClientId = client.Id
                };
                db.Rules.Add(ruleInDatabase);

                ruleInDatabase.ChangeCategoryTo = ruleFromJson.ChangeCategoryTo;
                ruleInDatabase.ChangeDateTo = ruleFromJson.ChangeDateTo;
                ruleInDatabase.ChangeIgnoreTo = ruleFromJson.ChangeIgnoreTo;
                ruleInDatabase.Regex = ruleFromJson.Regex;

                result.RulesAddedOrUpdated++;
            }

            if (rulesFromJson.Any())
            {
                await db.SaveChangesAsync();
            }

            return result;
        }
    }
}
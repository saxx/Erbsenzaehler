using System;
using Erbsenzaehler.AutoImporter.Configuration;

namespace Erbsenzaehler.AutoImporter.Client.Recipies
{
    public static class RecipeFactory
    {
        public static AbstractRecipe GetRecipe(ConfigurationContainer configuration)
        {
            AbstractRecipe recipe = null;
            if (configuration.Easybank != null)
            {
                recipe = new EasybankRecipe(configuration.Easybank);
            }

            if (recipe != null)
            {
                recipe.SaveScreenshots = configuration.SaveScreenshots;
                return recipe;
            }

            throw new Exception("Unable to extract recipe from configuration.");
        }
    }
}
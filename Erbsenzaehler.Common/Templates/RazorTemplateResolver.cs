using System;
using System.IO;
using RazorEngine.Templating;

#pragma warning disable 618

namespace Erbsenzaehler.Templates
{
    public class RazorTemplateResolver : ITemplateResolver
    {
        public string Resolve(string templateName)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            return File.ReadAllText(GetTemplatePath(templateName));
        }


        private string GetTemplatePath(string templateName)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", templateName);
            if (!File.Exists(templatePath))
            {
                templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Templates", templateName);
            }

            if (!File.Exists(templatePath))
            {
                throw new Exception("Template '" + templateName + "' missing. It should be at '" + templatePath + "'.");
            }

            return templatePath;
        }
    }
}

#pragma warning restore 618
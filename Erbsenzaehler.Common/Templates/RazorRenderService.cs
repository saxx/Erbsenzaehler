using RazorEngine.Configuration;
using RazorEngine.Templating;

#pragma warning disable 618

namespace Erbsenzaehler.Templates
{
    public class RazorRenderService<T>
    {
        public string Render(string templateName, T model)
        {
            var razorConfig = new TemplateServiceConfiguration
            {
                CachingProvider = new DefaultCachingProvider(t => { }),
                DisableTempFileLocking = true,
                Resolver = new RazorTemplateResolver()
            };
            var razorService = RazorEngineService.Create(razorConfig);
            return razorService.RunCompile(templateName, typeof (T), model);
        }
    }
}

#pragma warning restore 618
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Erbsenzaehler.Startup))]
namespace Erbsenzaehler
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

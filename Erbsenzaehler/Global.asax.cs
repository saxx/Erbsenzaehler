using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OneTrueError.Reporting;

namespace Erbsenzaehler
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (!string.IsNullOrEmpty(OneTrueErrorAppKey) && !string.IsNullOrEmpty(OneTrueErrorAppSecret))
            {
                OneTrue.Configuration.Credentials(OneTrueErrorAppKey, OneTrueErrorAppSecret);
            }
        }


        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            if (ex is ThreadAbortException)
                return;

            if (!Request.IsLocal)
            {
                if (!string.IsNullOrEmpty(OneTrueErrorAppKey) && !string.IsNullOrEmpty(OneTrueErrorAppSecret))
                {
                    OneTrue.Report(ex);
                }
            }
        }


        private string OneTrueErrorAppKey => System.Configuration.ConfigurationManager.AppSettings["OneTrueError.AppKey"] ?? "";
        private string OneTrueErrorAppSecret => System.Configuration.ConfigurationManager.AppSettings["OneTrueError.AppSecret"] ?? "";
    }
}
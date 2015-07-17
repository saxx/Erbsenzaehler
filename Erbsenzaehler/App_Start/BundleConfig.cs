using System.Web.Optimization;

namespace Erbsenzaehler
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-*"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app/*.js",
                "~/Scripts/app/LinesEditor/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include(
                "~/Scripts/globalize/globalize.js",
                "~/Scripts/dx.chartjs.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        }
    }
}
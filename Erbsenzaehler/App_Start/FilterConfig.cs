using System.Web.Mvc;

namespace Erbsenzaehler
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogErrorAttribute());
        }


        public class LogErrorAttribute : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                if (!filterContext.HttpContext.Request.IsLocal)
                {
                    // log the exception here
                }
                base.OnException(filterContext);
            }
        }
    }
}
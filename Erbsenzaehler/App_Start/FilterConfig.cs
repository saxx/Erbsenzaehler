using System.Web.Mvc;
using SharpBrake;

namespace Erbsenzaehler
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new SendErrorToAirbrakErrorAttribute());
        }


        public class SendErrorToAirbrakErrorAttribute : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                if (!filterContext.HttpContext.Request.IsLocal)
                {
                    filterContext.Exception.SendToAirbrake();
                }
                base.OnException(filterContext);
            }
        }
    }
}
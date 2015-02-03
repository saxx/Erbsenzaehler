using System.Web.Mvc;

namespace Erbsenzaehler.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Reports");
            }

            return View();
        }


        public ActionResult Contact()
        {
            return View();
        }
    }
}
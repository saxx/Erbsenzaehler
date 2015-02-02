using System.Web.Mvc;

namespace Erbsenzaehler.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Erbsenzaehler.Controllers
{
    public class HomeController : ControllerBase
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public async Task<ActionResult> Index()
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    // make sure we can load a client. if not, we have some kind of data corruption of invalid cookie
                    await GetCurrentClient();
                }
                catch
                {
                    AuthenticationManager.SignOut();
                    return Redirect("~/");
                }

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
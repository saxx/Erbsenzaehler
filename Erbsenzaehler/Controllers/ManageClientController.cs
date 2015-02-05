using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.ManageClient;
using Microsoft.Owin.Security;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ManageClientController : ControllerBase
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;


        public async Task<ActionResult> Index()
        {
            var currentClient = await GetCurrentClient();
            return View(new IndexViewModel().Fill(currentClient));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(IndexViewModel viewModel)
        {
            var currentClient = await GetCurrentClient();

            if (ModelState.IsValid)
            {
                currentClient.Name = viewModel.ClientName;
                await Db.SaveChangesAsync();

                viewModel.SuccessMessage = "Your changes have been saved successfully";
            }

            return View(viewModel);
        }


        public ActionResult Delete()
        {
            return View();
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed()
        {
            var currentClient = await GetCurrentClient();
            Db.Clients.Remove(currentClient);
            await Db.SaveChangesAsync();

            AuthenticationManager.SignOut();

            return Redirect("~/");
        }
    }
}
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Erbsenzaehler.ViewModels.ManageUsers;
using Microsoft.AspNet.Identity;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ManageUsersController : ControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var currentClient = await GetCurrentClient();
            return View(await new IndexViewModel().Fill(Db, currentClient));
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Email,Password,ConfirmPassword")] CreateViewModel viewModel)
        {
            var currentClient = await GetCurrentClient();

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = viewModel.Email,
                    UserName = viewModel.Email,
                    ClientId = currentClient.Id,
                    Client = currentClient
                };

                var result = await UserManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            return View(viewModel);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public async Task<ActionResult> Delete(string id)
        {
            var currentClient = await GetCurrentClient();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await Db.Users.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new DeleteViewModel().Fill(Db, user, await GetCurrentUser()));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var currentClient = await GetCurrentClient();

            var user = await Db.Users.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            await UserManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
    }
}
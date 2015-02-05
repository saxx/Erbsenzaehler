using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.ManageUser;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ManageUserController : ControllerBase
    {
        private ApplicationSignInManager _signInManager;


        public ManageUserController()
        {
        }


        public ManageUserController(ApplicationSignInManager signInManager)
        {
            SignInManager = signInManager;
        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                model.WasSuccessful = true;
            }
            else
            {
                AddErrors(result);
            }
            return View(model);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Erbsenzaehler.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected Db Db { get; }
        protected UserManager<User> UserManager { get; }


        protected ControllerBase()
        {
            Db = new Db();
            UserManager = new UserManager<User>(new UserStore<User>(Db));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                Db.Dispose();
            }
            base.Dispose(disposing);
        }


        public async Task<Client> GetCurrentClient()
        {
            var currentUser = await GetCurrentUser();
            return currentUser?.Client;
        }


        public async Task<User> GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return await UserManager.FindByIdAsync(User.Identity.GetUserId());
            }
            return null;
        }
    }
}
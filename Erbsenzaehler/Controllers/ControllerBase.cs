using System.Security;
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
            Db = new Db(Config.DatabaseConnectionString);
            UserManager = new UserManager<User>(new UserStore<User>(Db));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager?.Dispose();
                Db?.Dispose();
            }
            base.Dispose(disposing);
        }


        public async Task<Client> GetCurrentClient()
        {
            var currentUser = await GetCurrentUser();
            var currentClient = currentUser?.Client;
            if (currentClient == null)
            {
                throw new SecurityException("There is no user logged in at the moment.");
            }
            return currentClient;
        }


        public async Task<User> GetCurrentUser()
        {
            if (Request.IsAuthenticated)
            {
                return await UserManager.FindByIdAsync(User.Identity.GetUserId());
            }
            throw new SecurityException("There is no user logged in at the moment.");
        }
    }
}
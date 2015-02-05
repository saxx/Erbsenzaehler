using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Erbsenzaehler.ViewModels.ManageAccounts;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ManageAccountsController : ControllerBase
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
        public async Task<ActionResult> Create([Bind(Include = "Name")] Account account)
        {
            var currentClient = await GetCurrentClient();

            if (ModelState.IsValid)
            {
                account.ClientId = currentClient.Id;
                Db.Accounts.Add(account);
                await Db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(account);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            var currentClient = await GetCurrentClient();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id.Value);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Account account)
        {
            var currentClient = await GetCurrentClient();

            if (ModelState.IsValid)
            {
                var accountFromDatabase = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == account.Id);
                if (accountFromDatabase == null)
                {
                    return HttpNotFound();
                }

                accountFromDatabase.Name = account.Name;
                await Db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(account);
        }


        public async Task<ActionResult> Delete(int? id)
        {
            var currentClient = await GetCurrentClient();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id.Value);
            if (account == null)
            {
                return HttpNotFound();
            }

            var viewModel = await new DeleteViewModel().Fill(Db, currentClient, account);
            return View(viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var currentClient = await GetCurrentClient();

            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            Db.Accounts.Remove(account);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}

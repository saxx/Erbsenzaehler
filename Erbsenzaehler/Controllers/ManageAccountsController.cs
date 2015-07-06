using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Deduplicate;
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


        [Route("ManageAccounts/{id}/Duplicates")]
        public async Task<ActionResult> Duplicates(int id, bool fuzzyMatch = false)
        {
            var currentClient = await GetCurrentClient();
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (account != null)
            {
                var dedupService = new DeduplicateService(Db);
                var viewModel = new DuplicatesViewModel();
                if (!fuzzyMatch)
                {
                    viewModel.Fill(account, await dedupService.FindExactDuplicates(account.Id, true), false);
                }
                else
                {
                    viewModel.Fill(account, await dedupService.FindPossibleDuplicates(account.Id, 50), true);
                }
                return View(viewModel);
            }
            return HttpNotFound();
        }


        [Route("ManageAccounts/{id}/DeleteDuplicates")]
        public async Task<ActionResult> DeleteDuplicates(int id, bool fuzzyMatch = false)
        {
            var currentClient = await GetCurrentClient();
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (account != null)
            {
                return View(new DeleteDuplicatesViewModel().Fill(account, fuzzyMatch));
            }
            return HttpNotFound();
        }


        [HttpPost]
        [Route("ManageAccounts/{id}/DeleteDuplicates")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDuplicatesConfirmed(int id, bool fuzzyMatch = false)
        {
            var currentClient = await GetCurrentClient();
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);

            var dedupService = new DeduplicateService(Db);
            IEnumerable<Duplicate> duplicates;
            if (!fuzzyMatch)
            {
                duplicates = await dedupService.FindExactDuplicates(account.Id, true);
            }
            else
            {
                duplicates = await dedupService.FindPossibleDuplicates(account.Id, 20);
            }

            foreach (var line in duplicates.SelectMany(x => x.Duplicates))
            {
                Db.Lines.Remove(line);
            }
            await Db.SaveChangesAsync();

            return RedirectToAction("Duplicates", new { id = account.Id, fuzzyMatch });
        }


        public async Task<ActionResult> DeleteLine(int? id)
        {
            var currentClient = await GetCurrentClient();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var line = await Db.Lines.ByClient(currentClient).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (line == null)
            {
                return HttpNotFound();
            }

            var viewModel = new DeleteLineViewModel().Fill(line);
            return View(viewModel);
        }


        [HttpPost, ActionName("DeleteLine")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLineConfirmed(int id)
        {
            var currentClient = await GetCurrentClient();

            var line = await Db.Lines.ByClient(currentClient).Where(x => x.Id == id).FirstOrDefaultAsync();
            Db.Lines.Remove(line);
            await Db.SaveChangesAsync();

            return RedirectToAction("Duplicates", new { id = line.AccountId });
        }
    }
}
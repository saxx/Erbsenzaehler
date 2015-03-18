using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Erbsenzaehler.ViewModels.LinesEditor;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    [RoutePrefix("LinesEditor")]
    public class LinesEditorController : ControllerBase
    {
        [ChildActionOnly]
        [Route("")]
        public ActionResult Index(string month)
        {
            return PartialView("_LinesEditor");
        }


        [HttpGet]
        [Route("Json")]
        public async Task<ActionResult> LoadLines(string month)
        {
            int? selectedYear = null;
            int? selectedMonth = null;
            if (!string.IsNullOrEmpty(month) && month.Contains("-"))
            {
                selectedYear = int.Parse(month.Split('-')[0]);
                selectedMonth = int.Parse(month.Split('-')[1]);
            }

            var viewModel = await new JsonViewModel().Fill(await GetCurrentClient(), Db, selectedYear, selectedMonth);
            return new JsonResult
            {
                Data = viewModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        [HttpPost]
        [Route("Json")]
        public async Task<ActionResult> CreateLine(string month)
        {
            var currentClient = await GetCurrentClient();
            var m = new Month(month);
            var account = currentClient.Accounts.First();

            var date = m.Date.AddMonths(1).AddDays(-1);
            if (m.IsCurrentMonth)
            {
                date = DateTime.Now.Date;
            }

            var line = new Line
            {
                Account = account,
                AccountId = account.Id,
                OriginalAmount = 0,
                OriginalDate = date,
                OriginalText = "Account statement added manually on " + DateTime.Now.ToShortDateString() + ".",
                LineAddedManually = true
            };
            Db.Lines.Add(line);
            await Db.SaveChangesAsync();

            return await LoadLines(month);
        }


        [HttpPut]
        [Route("Json")]
        public async Task<ActionResult> UpdateLine(JsonViewModel.Line line)
        {
            var currentClient = await GetCurrentClient();

            var lineInDatebase = Db.Lines.FirstOrDefault(x => x.Id == line.Id);
            if (lineInDatebase == null || lineInDatebase.Account.ClientId != currentClient.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var anythingHasChanged = false;

            // sanitize the category
            if (line.Category != null)
            {
                line.Category = line.Category.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("<br>", "");
                line.Category = line.Category.Trim();
                if (line.Category == "")
                {
                    line.Category = null;
                }
            }

            if (lineInDatebase.Ignore != line.Ignore)
            {
                lineInDatebase.Ignore = line.Ignore;
                lineInDatebase.IgnoreUpdatedManually = true;
                anythingHasChanged = true;
            }

            var newDate = DateTime.Parse(line.Date);
            if (lineInDatebase.Date != newDate)
            {
                lineInDatebase.Date = newDate;
                lineInDatebase.DateUpdatedManually = true;
                anythingHasChanged = true;
            }

            if (lineInDatebase.Category != line.Category)
            {
                lineInDatebase.Category = line.Category;
                lineInDatebase.CategoryUpdatedManually = true;
                anythingHasChanged = true;
            }

            decimal amountAsDecimal;
            if (decimal.TryParse(line.Amount, out amountAsDecimal))
            {
                if (lineInDatebase.Amount != amountAsDecimal)
                {
                    lineInDatebase.Amount = amountAsDecimal;
                    lineInDatebase.AmountUpdatedManually = true;
                    anythingHasChanged = true;
                }
            }

            var newText = (line.Text ?? "").Replace("\n", "").Replace("\r", "").Trim();
            if (!string.IsNullOrWhiteSpace(newText) && lineInDatebase.Text != newText)
            {
                lineInDatebase.Text = newText;
                lineInDatebase.TextUpdatedManually = true;
                anythingHasChanged = true;
            }

            if (anythingHasChanged)
            {
                await Db.SaveChangesAsync();
            }

            return Json(true);
        }


        [HttpDelete]
        [Route("Json/{id}/")]
        public async Task<ActionResult> DeleteLine(int id)
        {
            var currentClient = await GetCurrentClient();

            var lineInDatebase = await Db.Lines.FirstOrDefaultAsync(x => x.Id == id);
            if (lineInDatebase == null || lineInDatebase.Account.ClientId != currentClient.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Db.Lines.Remove(lineInDatebase);
            await Db.SaveChangesAsync();

            return Json(true);
        }
    }
}
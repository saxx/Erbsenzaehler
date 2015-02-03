using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.Lines;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class LinesController : ControllerBase
    {
        public async Task<ActionResult> Index(string date)
        {
            var viewModel = await new IndexViewModel().Fill(Db, await GetCurrentClient());
            return View(viewModel);
        }


        public async Task<ActionResult> Json(string date)
        {
            int? selectedYear = null;
            int? selectedMonth = null;
            if (!string.IsNullOrEmpty(date) && date.Contains("-"))
            {
                selectedYear = int.Parse(date.Split('-')[0]);
                selectedMonth = int.Parse(date.Split('-')[1]);
            }

            var viewModel = await new JsonViewModel().Fill(await GetCurrentClient(), Db, selectedYear, selectedMonth);
            return new JsonResult
            {
                Data = viewModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        [HttpPost]
        public async Task<ActionResult> Json(JsonViewModel.Line line)
        {
            var currentClient = await GetCurrentClient();

            var lineInDatebase = Db.Lines.FirstOrDefault(x => x.Id == line.Id);
            if (lineInDatebase == null || lineInDatebase.Account.ClientId != currentClient.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            lineInDatebase.Ignore = line.Ignore;
            lineInDatebase.Date = DateTime.Parse(line.Date);
            lineInDatebase.UpdatedManually = true;
            Db.SaveChanges();

            return await Json("");
        }
    }
}
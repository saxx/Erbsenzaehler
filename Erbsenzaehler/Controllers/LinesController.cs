using System;
using Erbsenzaehler.Models;
using Erbsenzaehler.ViewModels.Lines;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class LinesController : ControllerBase
    {
        public ActionResult Index(string date)
        {
            return View();
        }

        public async Task<ActionResult> Json(string date)
        {
            int? selectedYear = null;
            int? selectedMonth = null;
            if (!string.IsNullOrEmpty(date))
            {
                selectedYear = int.Parse(date.Split('-')[0]);
                selectedMonth = int.Parse(date.Split('-')[1]);
            }

            var viewModel = await new IndexViewModel().Fill(await GetCurrentClient(), Db, selectedYear, selectedMonth);
            return new JsonResult
            {
                Data = viewModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public async Task<ActionResult> Json(IndexViewModel.Line line)
        {
            /*var currentClient = await GetCurrentClient();

            var lineInDatebase = Db.Lines.FirstOrDefault(x => x.Id == line.Id);
            if (lineInDatebase == null || lineInDatebase.ClientId != currentClient.Id)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (string.IsNullOrEmpty(line.Category))
                lineInDatebase.CategoryId = null;
            else
            {
                var category = Db.Categories.FirstOrDefault(x => x.ClientId == currentClient.Id && x.Name == line.Category);
                if (category != null)
                    lineInDatebase.CategoryId = category.Id;
            }
            lineInDatebase.Ignore = line.IsIgnored;
            if (string.IsNullOrEmpty(line.RefundDate))
                lineInDatebase.RefundDate = null;
            else
                lineInDatebase.RefundDate = DateTime.Parse(line.RefundDate);
            lineInDatebase.Date = DateTime.Parse(line.Date);
            Db.SaveChanges();*/

            return await Json("");
        }

    }
}
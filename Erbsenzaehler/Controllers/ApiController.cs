﻿using System;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.Importer;
using Erbsenzaehler.Rules;
using Erbsenzaehler.ViewModels.Api;
using Microsoft.AspNet.Identity.Owin;

namespace Erbsenzaehler.Controllers
{
    public class ApiController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Import(ImportViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Username or password empty.");
                }

                var signInStatus = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        break;
                    case SignInStatus.LockedOut:
                        return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "User locked.");
                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Username or password invalid.");
                }

                var currentClient = (await Db.Users.FirstAsync(x => x.UserName == model.Username)).Client;
                var account = await Db.Accounts.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Name == model.Account);
                if (account == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid account.");
                }

                ImporterType importer;
                if (!Enum.TryParse(model.Importer, true, out importer))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid importer.");
                }

                if (model.File == null || model.File.Length <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Empty file.");
                }

                var result = new ImportResultViewModel();
                try
                {
                    using (var stream = new MemoryStream(model.File))
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            var concreteImporter = new ImporterFactory().GetImporter(reader, importer);
                            var importResult = await concreteImporter.LoadFileAndImport(Db, currentClient, account, new RulesApplier());

                            result.IgnoredCount = importResult.DuplicateLinesCount;
                            result.ImportedCount = importResult.NewLinesCount;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Import failed: " + ex.Message);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        private ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
    }
}
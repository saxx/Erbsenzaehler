using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;
using Erbsenzaehler.SummaryMail;
using NLog;
using OneTrueError.Reporting;

namespace Erbsenzaehler.SummaryMails.WebJob
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Log.Trace("Erbsenzaehler.SummaryMails.WebJob v" + typeof (Program).Assembly.GetName().Version + " starting up ...");

                // hard-code german culture here, we want our e-mails formatted for german
                var germanCulture = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = germanCulture;
                Thread.CurrentThread.CurrentUICulture = germanCulture;

                if (!string.IsNullOrEmpty(Config.OneTrueErrorAppKey) && !string.IsNullOrEmpty(Config.OneTrueErrorAppSecret))
                {
                    OneTrue.Configuration.Credentials(Config.OneTrueErrorAppKey, Config.OneTrueErrorAppSecret);
                }

                using (var db = new Db())
                {
                    var usersQuery = db.Users
                        .Where(x => x.SummaryMailInterval != SummaryMailIntervalOptions.Disable)
                        .Select(x => new
                        {
                            User = x,
                            x.Client,
                            LastDate = x.SummaryMailLogs.Select(y => y.Date).OrderByDescending(y => y).FirstOrDefault()
                        })
                        .ToList();

                    Log.Trace(usersQuery.Count() + " user(s) found with summary mails enabled.");
                    foreach (var user in usersQuery)
                    {
                        try
                        {
                            if (SummaryMailIntervalService.ShouldReceiveSummaryMail(user.User.SummaryMailInterval, user.LastDate))
                            {
                                SendMail(db, user.User, user.Client);
                                SaveLog(db, user.User);
                            }
                            else
                            {
                                Log.Trace("User '" + user.User.Email + "' already got summary mail.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            LogException(ex);
                        }
                    }
                }

                Log.Trace("Everything done. Goodbye.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                LogException(ex);
            }
        }


        private static void SendMail(Db db, User user, Client client)
        {
            Log.Trace("Rendering template and sending email to user '" + user.Email + "' ...");

            var budgetCalculator = new BudgetCalculator(db, client);
            var sumCalculator = new SumCalculator(db, client);
            var renderer = new SummaryMailRenderer(db, new Uri(Config.ErbsenzaehlerUrl), budgetCalculator, sumCalculator);
            var mailer = new SendGridMailer();
            var sender = new SummaryMailSender(renderer, mailer);

            sender.RenderAndSend(user);
        }


        private static void SaveLog(Db db, User user)
        {
            Log.Trace("Saving to summary mail log ...");
            db.SummaryMailLogs.Add(new SummaryMailLog
            {
                Date = DateTime.UtcNow,
                UserId = user.Id
            });
            db.SaveChanges();
        }


        private static void LogException(Exception ex)
        {
            if (!string.IsNullOrEmpty(Config.OneTrueErrorAppKey) && !string.IsNullOrEmpty(Config.OneTrueErrorAppSecret))
            {
                OneTrue.Report(ex);
            }
        }


        private static Logger Log => LogManager.GetCurrentClassLogger();
    }
}
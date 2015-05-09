using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;
using Erbsenzaehler.SummaryMail;
using OneTrueError.Reporting;

namespace Erbsenzaehler.SummaryMails.WebJob
{
    public class Program
    {
        public static void Main()
        {
            var logger = new Logger();

            try
            {
                logger.Info("Erbsenzaehler.SummaryMails.WebJob v" + typeof (Program).Assembly.GetName().Version + " starting up ...");

                // hard-code german culture here, we want our e-mails formatted for german
                var germanCulture = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = germanCulture;
                Thread.CurrentThread.CurrentUICulture = germanCulture;

                if (!string.IsNullOrEmpty(Config.OneTrueErrorAppKey) && !string.IsNullOrEmpty(Config.OneTrueErrorAppSecret))
                {
                    OneTrue.Configuration.Credentials(Config.OneTrueErrorAppKey, Config.OneTrueErrorAppSecret);
                }

                using (var db = new Db(Erbsenzaehler.Config.DatabaseConnectionString))
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

                    logger.Trace(usersQuery.Count() + " user(s) found with summary mails enabled.");
                    foreach (var user in usersQuery)
                    {
                        try
                        {
                            if (SummaryMailIntervalService.ShouldReceiveSummaryMail(user.User.SummaryMailInterval, user.LastDate))
                            {
                                SendMail(db, user.User, user.Client, logger);
                                SaveLog(db, user.User, logger);
                            }
                            else
                            {
                                logger.Info("User '" + user.User.Email + "' already got summary mail.");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            LogException(ex);
                        }
                    }
                }

                logger.Info("Everything done. Goodbye.");
            }
            catch (Exception ex)
            {
                logger.Trace(ex.ToString());
                LogException(ex);
            }
        }


        private static void SendMail(Db db, User user, Client client, ILogger logger)
        {
            logger?.Info("Rendering template and sending email to user '" + user.Email + "' ...");

            var budgetCalculator = new BudgetCalculator(db, client);
            var sumCalculator = new SumCalculator(db, client);
            var renderer = new SummaryMailRenderer(db, budgetCalculator, sumCalculator);
            var mailer = new SendGridMailer();
            var sender = new SummaryMailSender(renderer, mailer);

            sender.RenderAndSend(user);
        }


        private static void SaveLog(Db db, User user, ILogger logger)
        {
            logger?.Trace("Saving to summary mail log ...");
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
    }
}
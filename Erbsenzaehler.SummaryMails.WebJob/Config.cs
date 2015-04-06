﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erbsenzaehler.SummaryMails.WebJob
{
    public static class Config
    {
        public static string ErbsenzaehlerUrl => System.Configuration.ConfigurationManager.AppSettings["Erbsenzaehler.Url"] ?? "http://erbsenzaehler.azurewebsites.net";
        public static string OneTrueErrorAppKey => System.Configuration.ConfigurationManager.AppSettings["OneTrueError.AppKey"] ?? "";
        public static string OneTrueErrorAppSecret => System.Configuration.ConfigurationManager.AppSettings["OneTrueError.AppSecret"] ?? "";
        public static string SendGridUserName => System.Configuration.ConfigurationManager.AppSettings["SendGrid.UserName"] ?? "";
        public static string SendGridPassword => System.Configuration.ConfigurationManager.AppSettings["SendGrid.Password"] ?? "";
        public static string SendGridSender => System.Configuration.ConfigurationManager.AppSettings["SendGrid.Sender"] ?? "erbsenzaehler@erbsenzaehler.azurewebsites.net";
    }
}

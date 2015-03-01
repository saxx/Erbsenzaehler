using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using NLog;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;

namespace Erbsenzaehler.AutoImporter.Recipies
{
    public class EasybankRecipe
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Account { get; set; }


        public EasybankRecipe(string username, string password, string account)
        {
            Username = username;
            Password = password;
            Account = account;
        }


        public void DownloadFile(string filePath)
        {
            Log.Info("Downloading account statements from Easybank ...");

            using (var driverService = PhantomJSDriverService.CreateDefaultService())
            {
                driverService.HideCommandPromptWindow = true;

                using (var driver = new PhantomJSDriver(driverService))
                {
                    driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(60));
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

                    try
                    {
                        Log.Trace("Opening Easybank homepage ...");
                        driver.Navigate().GoToUrl("https://ebanking.easybank.at");
                        Log.Trace("Saved screenshot as Easybank-1.png");
                        driver.TakeScreenshot().SaveAsFile("Easybank-1.png", ImageFormat.Png);

                        Log.Trace("Filling login credentials ...");
                        driver.FindElementById("lof5").SendKeys(Username);
                        driver.FindElementById("lof9").SendKeys(Password);
                        driver.FindElementByLinkText("Login").Click();
                        Log.Trace("Saved screenshot as Easybank-2.png");
                        driver.TakeScreenshot().SaveAsFile("Easybank-2.png", ImageFormat.Png);

                        Log.Trace("Loading account overview ...");
                        driver.FindElementByLinkText(Account).Click();
                        Log.Trace("Saved screenshot as Easybank-3.png");
                        driver.TakeScreenshot().SaveAsFile("Easybank-3.png", ImageFormat.Png);

                        Log.Trace("Injecting & executing JavaScript ...");
                        const string script = "var resultField = $('<pre />').attr('id', 'csv_result');" +
                                              "var form = document.transactionSearchForm; form.csv.value = 'true';" +
                                              "$.ajax({ url: $(form).attr('action')," +
                                              "type: 'post'," +
                                              "data: $(form).serialize()," +
                                              "success: function(response) {" +
                                              "$('body').html('').append(resultField);" +
                                              "resultField.html(response);" +
                                              "}});";
                        driver.ExecuteScript(script);

                        Log.Trace("Loading file content from page and saving to {0} ...", filePath);
                        var text = driver.FindElementById("csv_result").Text;
                        Log.Trace("Saved screenshot as Easybank-4.png");
                        driver.TakeScreenshot().SaveAsFile("Easybank-4.png", ImageFormat.Png);

                        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                        {
                            writer.Write(text);
                        }

                        Log.Info("Download completed.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unable to download from Easybank: " + ex);
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
            }
        }


        private static Logger Log => LogManager.GetCurrentClassLogger();
    }
}
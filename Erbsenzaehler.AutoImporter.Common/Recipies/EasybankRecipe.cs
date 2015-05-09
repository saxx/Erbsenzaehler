using System;
using System.IO;
using System.Text;
using Erbsenzaehler.AutoImporter.Configuration;
using OpenQA.Selenium.PhantomJS;

namespace Erbsenzaehler.AutoImporter.Recipies
{
    public class EasybankRecipe : AbstractRecipe
    {
        private readonly EasybankConfiguration _configuration;


        public EasybankRecipe(EasybankConfiguration configuration)
        {
            _configuration = configuration;
        }


        public override void DownloadFile(string temporaryFilePath, ILogger logger)
        {
            logger?.Info("Downloading account statements from Easybank ...");

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
                        logger?.Trace("Opening Easybank homepage ...");
                        driver.Navigate().GoToUrl("https://ebanking.easybank.at");
                        Screenshot(driver, "Easybank-1", logger);

                        logger?.Trace("Filling login credentials ...");
                        driver.FindElementById("lof5").SendKeys(_configuration.Verfuegernummer);
                        driver.FindElementById("lof9").SendKeys(_configuration.Pin);
                        driver.FindElementByLinkText("Login").Click();
                        Screenshot(driver, "Easybank-2", logger);

                        logger?.Trace("Loading account overview ...");
                        driver.FindElementByLinkText(_configuration.Kontonummer).Click();
                        Screenshot(driver, "Easybank-3", logger);

                        logger?.Info("Injecting & executing JavaScript ...");
                        const string script = "var resultField = $('<pre />').attr('id', 'csv_result');" +
                                              "var form = document.transactionSearchForm;" +
                                              "form.csv.value = 'true';" +
                                              "$.ajax({ url: $(form).attr('action')," +
                                              "type: 'post'," +
                                              "data: $(form).serialize()," +
                                              "error: function(xhr, status, error) {" +
                                              "$('body').html('').append(resultField);" +
                                              "resultField.html('AJAX request failed: ' + status + ' / ' + error);" +
                                              "}," +
                                              "success: function(response) {" +
                                              "$('body').html('').append(resultField);" +
                                              "resultField.html(response);" +
                                              "}});";
                        driver.ExecuteScript(script);

                        logger?.Trace("Loading file content from page and saving to {0} ...", temporaryFilePath);
                        var text = driver.FindElementById("csv_result").Text;
                        Screenshot(driver, "Easybank-4", logger);

                        using (var writer = new StreamWriter(temporaryFilePath, false, Encoding.UTF8))
                        {
                            writer.Write(text);
                        }

                        logger?.Trace("Download completed.");
                    }
                    catch (Exception ex)
                    {
                        logger?.Error("Unable to download from Easybank: " + ex);
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
            }
        }
    }
}
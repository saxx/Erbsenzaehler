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


        public override void DownloadFile(string temporaryFilePath)
        {
            Console.WriteLine("Downloading account statements from Easybank ...");

            using (var driverService = PhantomJSDriverService.CreateDefaultService())
            {
                //driverService.HideCommandPromptWindow = true;

                using (var driver = new PhantomJSDriver(driverService))
                {
                    driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(60));
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

                    try
                    {
                        Console.WriteLine("Opening Easybank homepage ...");
                        driver.Navigate().GoToUrl("https://ebanking.easybank.at");
                        Screenshot(driver, "Easybank-1");

                        Console.WriteLine("Filling login credentials ...");
                        driver.FindElementById("lof5").SendKeys(_configuration.Verfuegernummer);
                        driver.FindElementById("lof9").SendKeys(_configuration.Pin);
                        driver.FindElementByLinkText("Login").Click();
                        Screenshot(driver, "Easybank-2");

                        Console.WriteLine("Loading account overview ...");
                        driver.FindElementByLinkText(_configuration.Kontonummer).Click();
                        Screenshot(driver, "Easybank-3");

                        Console.WriteLine("Injecting & executing JavaScript ...");
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

                        Console.WriteLine("Loading file content from page and saving to {0} ...", temporaryFilePath);
                        var text = driver.FindElementById("csv_result").Text;
                        Screenshot(driver, "Easybank-4");

                        using (var writer = new StreamWriter(temporaryFilePath, false, Encoding.UTF8))
                        {
                            writer.Write(text);
                        }

                        Console.WriteLine("Download completed.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to download from Easybank: " + ex);
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
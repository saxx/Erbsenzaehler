using System.Drawing.Imaging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace Erbsenzaehler.AutoImporter.Recipies
{
    public abstract class AbstractRecipe
    {
        public abstract void DownloadFile(string temporaryFilePath, ILogger logger);
        public bool SaveScreenshots { get; set; }


        protected void Screenshot(IWebDriver driver, string filename, ILogger logger)
        {
            if (SaveScreenshots)
            {
                logger?.Trace("Saving screenshot as " + filename + ".png ...");
                driver.TakeScreenshot().SaveAsFile(filename + ".png", ImageFormat.Png);
            }
        }
    }
}
using System.Drawing.Imaging;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace Erbsenzaehler.AutoImporter.Client.Recipies
{
    public abstract class AbstractRecipe
    {
        public abstract void DownloadFile(string temporaryFilePath);
        public bool SaveScreenshots { get; set; }


        protected void Screenshot(IWebDriver driver, string filename)
        {
            if (SaveScreenshots)
            {
                Log.Trace("Saving screenshot as " + filename + ".png ...");
                driver.TakeScreenshot().SaveAsFile(filename + ".png", ImageFormat.Png);
            }
        }


        protected static Logger Log => LogManager.GetCurrentClassLogger();
    }
}
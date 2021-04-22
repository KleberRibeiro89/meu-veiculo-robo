using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meu_veiculo_robo.Extensions
{
    public static class WebDriverExtensions
    {
        public  static IWebDriver CriarDriveChrome()
        {
            var options = new ChromeOptions();


            //options.AddArgument("--headless");
            //options.AddArgument("--proxy-server='direct://'");
            //options.AddArgument("--proxy-bypass-list=*");
            //options.AddArgument("--disable-gpu");

            options.AddUserProfilePreference("browser.set_download_behavior", new Dictionary<string, object>
            {
                { "behavior", "allow" }
            });


            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--ignore-certificate-errors");

            var localDownload = "./";

            options.AddArguments("--browser.download.folderList=2");
            //options.AddArguments($"--browser.helperApps.neverAsk.saveToDisk={MimeTypes}");
            options.AddArguments($"--browser.download.dir={localDownload}");


            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            options.AddUserProfilePreference("download.prompt_for_download", "false");
            options.AddUserProfilePreference("download.default_directory", localDownload);
            options.AddUserProfilePreference("download.dirctory_upgrade", "true");
            options.AddUserProfilePreference("plugins.plugins_disabled", new[] { "Chrome PDF Viewer" });

            //if (!string.IsNullOrWhiteSpace(seleniumConfiguration.DiretorioInstalacaoChrome))
            //    options.BinaryLocation = seleniumConfiguration.DiretorioInstalacaoChrome;

            return new ChromeDriver(".", options);
        }

        public static void Screenshot(this IWebDriver driver, string path)
        {
            var camera = driver as ITakesScreenshot;
            var foto = camera.GetScreenshot();
            foto.SaveAsFile(path, ScreenshotImageFormat.Png);
        }

        public static void ExecuteJavaScript(this IWebDriver driver, string script)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string title = (string)js.ExecuteScript(script);
        }

        public static void FullDispose(this IWebDriver webDriver)
        {
            webDriver?.Close();
            webDriver?.Quit();
            webDriver?.Dispose();
        }
    }
}

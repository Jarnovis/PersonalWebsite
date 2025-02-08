using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SQLitePCL;
using WebApi.Enviroment;

namespace WebApi.StudyInfo;

public class OpenWebsite : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly EnvConfig _envConfig;
    private IReadOnlyCollection<Cookie> _cookies;
    private string _mainWindowHanlde;

    public OpenWebsite()
    {
        _driver = new ChromeDriver(SetDriverOptions());
        _envConfig = new EnvConfig();
        _mainWindowHanlde = _driver.CurrentWindowHandle;
    }

    public bool SignInMicrosoftAuthenticator(string url)
    {
        try
        {
            _driver.Navigate().GoToUrl(url);


            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            Thread.Sleep(2000);
            

            var emailField = wait.Until(d => d.FindElement(By.Id("i0116")));
            emailField.SendKeys(_envConfig.Get("STUDENT_EMAIL"));

            var nextButton = _driver.FindElement(By.Id("idSIButton9"));
            nextButton.Click();

            Thread.Sleep(2000);

            var passwordField = wait.Until(d => d.FindElement(By.Id("i0118"))); 
            passwordField.SendKeys(_envConfig.Get("STUDENT_PASSWORD"));

            var loginButton = _driver.FindElement(By.Id("idSIButton9"));
            loginButton.Click();

            Thread.Sleep(3000);

            foreach (var handle in _driver.WindowHandles)
            {
                if (handle != _mainWindowHanlde)
                {
                    _driver.SwitchTo().Window(handle);
                    _driver.Close();
                }
            }

            _driver.SwitchTo().Window(_mainWindowHanlde);

            _cookies = _driver.Manage().Cookies.AllCookies;

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public string GetInfoFromWebPage(string url)
    {

        _driver.Navigate().GoToUrl(url);

        if (_cookies != null)
        {
            foreach (var cookie in _cookies)
            {
                _driver.Manage().Cookies.AddCookie(cookie);
            }
        }

        _driver.Navigate().Refresh();

        Thread.Sleep(300);

        WebDriverWait waitForExpandButtons = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var expandButtons = waitForExpandButtons.Until(d => d.FindElements(By.XPath("//i[contains(@class, 'material-icons') and text()='expand_more']")));

        foreach (var expandButton in expandButtons)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", expandButton);

            if (expandButton.Displayed && expandButton.Enabled)
            {
                expandButton.Click();
                Thread.Sleep(200);
            }
        }

        return _driver.PageSource;
    }

    private static ChromeOptions SetDriverOptions()
    {
        ChromeOptions options = new ChromeOptions();

        options.AddArguments("--headless", "--disable-gpu", "--no-sandbox", "--remote-debuggin-port=9222, --disable-dev-shm-usage");

        if (System.Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Ubuntu/Docker (Linux-based system)
            options.BinaryLocation = "/usr/bin/chromium";
        }
        else if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // Windows
            //options.BinaryLocation = @"C:\Program Files\Chromium\chromium.exe";
        }

        return options;
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
        _envConfig.Dispose();
    }
}
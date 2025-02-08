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


            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));  

            Thread.Sleep(2000);
            try
            {   
                var emailField = wait.Until(d => d.FindElement(By.Id("i0116")));
                emailField.SendKeys(_envConfig.Get("STUDENT_EMAIL"));
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"No email field: {ex.Message}");
            }
            
            try
            {
                var nextButton = _driver.FindElement(By.Id("idSIButton9"));
                nextButton.Click();
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"No next buton: {ex.Message}");
            }

            Thread.Sleep(2000);
            try
            {
                var passwordField = wait.Until(d => d.FindElement(By.Id("i0118"))); 
                passwordField.SendKeys(_envConfig.Get("STUDENT_PASSWORD"));
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"No password field: {ex.Message}");
            }

            try
            {
                var loginButton = _driver.FindElement(By.Id("idSIButton9"));
                loginButton.Click();
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"No login button: {ex.Message}");
            }

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
        catch (WebDriverException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
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

        if (System.Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Ubuntu/Docker (Linux-based system)
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "selenium-manager", "linux", "selenium-manager");
            if (File.Exists(path))
            {
                options.BinaryLocation = path;
                options.AddArguments("--headless", 
                             "--disable-gpu", 
                             "--no-sandbox", 
                             "--disable-dev-shm-usage", 
                             "--remote-debugging-port=9222", 
                             "--window-size=1920,1080");
            }
            else
            {
                Console.WriteLine($"ERROR: Selenium Manager not found at {path}");
            }
        }
        else if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // Windows
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "selenium-manager", "windows", "selenium-manager.exe");
            if (File.Exists(path))
            {
                options.BinaryLocation = path;
                options.AddArguments("--headless", "--disable-gpu", "--no-sandbox", "--remote-debugging-port=9222", "--disable-dev-shm-usage");
            }
            else
            {
                Console.WriteLine($"ERROR: Selenium Manager not found at {path}");
            }
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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace SeleniumAutomation
{

    public class ShopWithAccountsFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        public WebDriverWait Wait { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        private const string BaseUrl = "https://demowebshop.tricentis.com/";

        public ShopWithAccountsFixture()
        {
            Driver = new ChromeDriver();
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            Email = "testuser" + Guid.NewGuid() + "@mail.com";
            Password = "Test@1234";

            RegisterUser();
        }

        private void RegisterUser()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
            Driver.Manage().Window.Maximize();
            Driver.FindElement(By.LinkText("Register")).Click();
            Driver.FindElement(By.Id("gender-male")).Click();
            Driver.FindElement(By.Id("FirstName")).SendKeys("Test");
            Driver.FindElement(By.Id("LastName")).SendKeys("User");
            Driver.FindElement(By.Id("Email")).SendKeys(Email);
            Driver.FindElement(By.Id("Password")).SendKeys(Password);
            Driver.FindElement(By.Id("ConfirmPassword")).SendKeys(Password);
            Driver.FindElement(By.Id("register-button")).Click();
            Driver.FindElement(By.XPath("//input[@value='Continue']")).Click();
        }
        public void Reset()
        {
            try
            {
                var logoutLink = Driver.FindElements(By.LinkText("Log out"));
                if (logoutLink.Count > 0)
                    logoutLink[0].Click();
            }
            catch {}
            Driver.Navigate().GoToUrl(BaseUrl);
        }
        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Xunit;

namespace SeleniumAutomation
{
    public class ShopWithAccountsTest : IClassFixture<ShopWithAccountsFixture>
    {
        private readonly ShopWithAccountsFixture fixture;
        private const string BaseUrl = "https://demowebshop.tricentis.com/";

        public ShopWithAccountsTest(ShopWithAccountsFixture fixture)
        {
            this.fixture = fixture;
        }

        private void Login()
        {
            fixture.Driver.Navigate().GoToUrl(BaseUrl);
            fixture.Driver.Manage().Window.Maximize();
            fixture.Driver.FindElement(By.XPath("//li/a[@href='/login']")).Click();
            fixture.Driver.FindElement(By.Id("Email")).SendKeys(fixture.Email);
            fixture.Driver.FindElement(By.Id("Password")).SendKeys(fixture.Password);
            fixture.Driver.FindElement(By.XPath("//input[@value='Log in']")).Click();
        }
        private void AddProductsToCart(string dataFile)
        {
            fixture.Driver.FindElement(By.XPath("//li/a[@href='/digital-downloads']")).Click();

            var products = File.ReadAllLines(dataFile);
            foreach (var product in products)
            {
                IWebElement productLink = fixture.Wait.Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.XPath($"//h2/a[normalize-space(text())='{product}']")));
                productLink.Click();

                IWebElement addToCartButton = fixture.Wait.Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[@class='add-to-cart-panel']/input[@type='button']")));
                addToCartButton.Click();

                fixture.Wait.Until(
                    ExpectedConditions.ElementIsVisible(
                        By.XPath("//div[@class='bar-notification success']")));

                fixture.Driver.Navigate().GoToUrl(BaseUrl + "digital-downloads");
            }
        }

        private void Checkout()
        {
            fixture.Driver.FindElement(By.XPath("//a[span[text() = 'Shopping cart']]")).Click();
            fixture.Driver.FindElement(By.Id("termsofservice")).Click();
            fixture.Driver.FindElement(By.Id("checkout")).Click();

            var select = fixture.Driver.FindElements(By.XPath("//select[@name='billing_address_id']"));

            if (select.Count == 0)
            {
                fixture.Driver.FindElement(By.Id("BillingNewAddress_CountryId")).Click();
                fixture.Driver.FindElement(By.XPath("//option[@value='1']")).Click();
                fixture.Driver.FindElement(By.Id("BillingNewAddress_City")).SendKeys("City");
                fixture.Driver.FindElement(By.Id("BillingNewAddress_Address1")).SendKeys("test st. 13");
                fixture.Driver.FindElement(By.Id("BillingNewAddress_ZipPostalCode")).SendKeys("00000");
                fixture.Driver.FindElement(By.Id("BillingNewAddress_PhoneNumber")).SendKeys("00000000");
            }

            var billingContinue = fixture.Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@id='checkout-step-billing']//input[@value='Continue']")));
            billingContinue.Click();

            var paymentContinue = fixture.Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@id='checkout-step-payment-method']//input[@value='Continue']")));
            paymentContinue.Click();

            var paymentInfoContinue = fixture.Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@id='checkout-step-payment-info']//input[@value='Continue']")));
            paymentInfoContinue.Click();

            var confirmation = fixture.Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@id='checkout-step-confirm-order']//input[@value='Confirm']")));
            confirmation.Click();

            var orderSuccessMessage = fixture.Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@class='section order-completed']//strong")));
            Assert.Contains("Your order has been successfully processed!", orderSuccessMessage.Text);
        }

        [Theory]
        [InlineData("TestData/data1.txt")]
        [InlineData("TestData/data2.txt")]
        public void Test_Order_From_Data(string dataFile)
        {
            string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fullPath = Path.Combine(projectRoot, dataFile);

            fixture.Reset();

            Login();
            AddProductsToCart(fullPath);
            Checkout();
        }
    }
}

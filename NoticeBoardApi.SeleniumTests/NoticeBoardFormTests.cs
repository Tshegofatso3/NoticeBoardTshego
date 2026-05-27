using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace NoticeBoardApi.SeleniumTests
{
    public class NoticeBoardFormTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private const string IndexHtmlPath = "file:///C:\\Users\\User\\source\\repos\\Tshegofatso3\\NoticeBoardApi\\NoticeBoard.Html\\index.html";

        

        // update path
       

        public NoticeBoardFormTests()
        {
            var options = new ChromeOptions();



            var isCi = Environment.GetEnvironmentVariable("CI") == "true";
            if (isCi)
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
            }



            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _driver.Navigate().GoToUrl(IndexHtmlPath);
        }



        public void Dispose() => _driver.Quit();

        // ── Test 1: Successful login ─────────────────────────────── 
        [Fact]
        public void Login_ShowsSuccessMessage_WithValidCredentials()
        {
            _driver.FindElement(By.Id("username")).SendKeys("admin");
            _driver.FindElement(By.Id("password")).SendKeys("Admin@123");
            _driver.FindElement(By.Id("loginBtn")).Click();

            var msg = _wait.Until(d =>
            {
                var el = d.FindElement(By.Id("loginMessage"));
                return el.GetAttribute("data-status") == "success" ? el : null;
            });
            Assert.Contains("successful", msg.Text);
        }

        // ── Test 2: Wrong password ──────────────────────────────── 
        [Fact]
        public void Login_ShowsErrorMessage_WithWrongPassword()
        {
            _driver.FindElement(By.Id("username")).SendKeys("admin");
            _driver.FindElement(By.Id("password")).SendKeys("wrongpassword");
            _driver.FindElement(By.Id("loginBtn")).Click();

            var msg = _wait.Until(d =>
            {
                var el = d.FindElement(By.Id("loginMessage"));
                return el.GetAttribute("data-status") == "error" ? el : null;
            });
            Assert.Contains("failed", msg.Text);
        }

        // ── Test 3: Post without login ──────────────────────────── 
        [Fact]
        public void PostNotice_ShowsUnauthorised_WhenNotLoggedIn()
        {
            _driver.FindElement(By.Id("title")).SendKeys("Test Notice");
            _driver.FindElement(By.Id("body")).SendKeys("Test body");
            _driver.FindElement(By.Id("postBtn")).Click();

            var msg = _wait.Until(d =>
            {
                var el = d.FindElement(By.Id("postMessage"));
                return el.GetAttribute("data-status") == "unauthorised" ? el : null;
            });
            Assert.Contains("login", msg.Text.ToLower());
        }

        // ── Test 4: Login then post notice ──────────────────────── 
        [Fact]
        public void PostNotice_ShowsSuccess_AfterLogin()
        {
            _driver.FindElement(By.Id("username")).SendKeys("admin");
            _driver.FindElement(By.Id("password")).SendKeys("Admin@123");
            _driver.FindElement(By.Id("loginBtn")).Click();
            _wait.Until(d => d.FindElement(By.Id("loginMessage"))
                              .GetAttribute("data-status") == "success");

            _driver.FindElement(By.Id("title")).SendKeys("Selenium Test Notice");
            _driver.FindElement(By.Id("body")).SendKeys("Posted by Selenium");
            _driver.FindElement(By.Id("postBtn")).Click();

            var msg = _wait.Until(d =>
            {
                var el = d.FindElement(By.Id("postMessage"));
                return el.GetAttribute("data-status") == "success" ? el : null;
            });
            Assert.Contains("successfully", msg.Text);
        }

        // ── Test 5: Load notices ────────────────────────────────── 
        [Fact]
        public void LoadNotices_DisplaysNoticeCards_WhenNoticesExist()
        {
            _driver.FindElement(By.Id("loadBtn")).Click();
            _wait.Until(d => d.FindElements(By.ClassName("notice")).Count > 0);
            var cards = _driver.FindElements(By.ClassName("notice"));
            Assert.True(cards.Count > 0);
        }
    }
}
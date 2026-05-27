using Microsoft.Playwright;
using Xunit;

namespace NoticeBoardApi.PlaywrightTests
{
    public class NoticeBoardFormTests : IAsyncLifetime
    {
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;
        private IPage _page = null!;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new
BrowserTypeLaunchOptions
            {
                Headless = true  // set to true for CI/CD; false lets you watch the browser
            });
            _page = await _browser.NewPageAsync();
            await _page.GotoAsync(Settings.IndexHtmlPath);
        }

        public async Task DisposeAsync()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        // ── Test 1: Successful login shows green message ────────── 
        [Fact]
        public async Task Login_ShowsSuccessMessage_WithValidCredentials()
        {
            await _page.FillAsync("#username", "admin");
            await _page.FillAsync("#password", "Admin@123");
            await _page.ClickAsync("#loginBtn");

            await _page.WaitForSelectorAsync("#loginMessage[data-status='success']");
            var msg = await _page.InnerTextAsync("#loginMessage");
            Assert.Contains("successful", msg);
        }
        // ── Test 2: Wrong password shows error message ──────────── 
        [Fact]
        public async Task Login_ShowsErrorMessage_WithWrongPassword()
        {
            await _page.FillAsync("#username", "admin");
            await _page.FillAsync("#password", "wrongpassword");
            await _page.ClickAsync("#loginBtn");

            await _page.WaitForSelectorAsync("#loginMessage[data-status='error']");
            var msg = await _page.InnerTextAsync("#loginMessage");
            Assert.Contains("failed", msg);
        }

        // ── Test 3: Post notice without login shows unauthorised ── 
        [Fact]
        public async Task PostNotice_ShowsUnauthorised_WhenNotLoggedIn()
        {
            await _page.FillAsync("#title", "Test Notice");
            await _page.FillAsync("#body", "Test body text");
            await _page.ClickAsync("#postBtn");

            await _page.WaitForSelectorAsync("#postMessage[data-status='unauthorised']");
            var msg = await _page.InnerTextAsync("#postMessage");
            Assert.Contains("login", msg.ToLower());
        }

        // ── Test 4: Login then post notice shows success ────────── 
        [Fact]
        public async Task PostNotice_ShowsSuccess_AfterLogin()
        {
            // Login first 
            await _page.FillAsync("#username", "admin");
            await _page.FillAsync("#password", "Admin@123");
            await _page.ClickAsync("#loginBtn");
            await _page.WaitForSelectorAsync("#loginMessage[data-status='success']");

            // Post a notice 
            await _page.FillAsync("#title", "Playwright Test Notice");
            await _page.FillAsync("#body", "Posted by Playwright automation");
            await _page.ClickAsync("#postBtn");

            await _page.WaitForSelectorAsync("#postMessage[data-status='success']");
            var msg = await _page.InnerTextAsync("#postMessage");
            Assert.Contains("successfully", msg);
        }

        // ── Test 5: Load notices shows at least one notice card ─── 
        [Fact]
        public async Task LoadNotices_DisplaysNoticeCards_WhenNoticesExist()
        {
            await _page.ClickAsync("#loadBtn");
            await _page.WaitForSelectorAsync(".notice");
            var count = await _page.Locator(".notice").CountAsync();
            Assert.True(count > 0);
        }
    }
}
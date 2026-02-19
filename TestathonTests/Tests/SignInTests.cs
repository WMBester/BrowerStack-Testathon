using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("signin")]
    public class SignInTests : PageTest
    {
        private const string BaseUrl = "https://testathon.live";
        private const string Password = "testingisfun99";

        private async Task SelectDropdownOption(string containerId, string optionText)
        {
            await Page.Locator($"#{containerId}").ClickAsync();
            await Page.WaitForSelectorAsync($"#{containerId} [class*='menu']");
            await Page.GetByRole(AriaRole.Option, new() { Name = optionText, Exact = true }).ClickAsync();
        }

        private async Task SignIn(string username)
        {
            await SelectDropdownOption("username", username);
            await SelectDropdownOption("password", Password);
            await Page.Locator("#login-btn").ClickAsync();
        }

        [Test]
        [Description("Successful sign in with valid credentials")]
        public async Task SuccessfulSignInWithValidCredentials()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-166]]");
            await Page.GotoAsync($"{BaseUrl}/signin");

            await SignIn("demouser");

            await Expect(Page).ToHaveURLAsync(new Regex(@"testathon\.live/(\?|$)"));
            await Expect(Page.Locator("span.username")).ToHaveTextAsync("demouser");
        }

        [Test]
        [Description("Sign in redirects to checkout when coming from cart")]
        public async Task SignInRedirectsToCheckoutFromCart()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-167]]");
            await Page.GotoAsync($"{BaseUrl}/checkout");

            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?checkout=true"));

            await SignIn("demouser");

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
        }

        [Test]
        [Description("Sign in redirects to favourites when coming from favourites")]
        public async Task SignInRedirectsToFavouritesFromFavourites()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-168]]");
            await Page.GotoAsync($"{BaseUrl}/favourites");

            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?favourites=true"));

            await SignIn("demouser");

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/favourites"));
        }

        [Test]
        [Description("Sign in redirects to offers when coming from offers")]
        public async Task SignInRedirectsToOffersFromOffers()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-169]]");
            await Page.GotoAsync($"{BaseUrl}/offers");

            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?offers=true"));

            await SignIn("demouser");

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/offers"));
        }

        [Test]
        [Description("Locked user cannot sign in")]
        public async Task LockedUserCannotSignIn()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-170]]");
            await Page.GotoAsync($"{BaseUrl}/signin");

            await SignIn("locked_user");

            await Expect(Page.Locator("h3.api-error")).ToBeVisibleAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*signin"));
        }

        [Test]
        [Description("Username dropdown displays all expected options")]
        public async Task UsernameDropdownDisplaysAllExpectedOptions()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-171]]");
            await Page.GotoAsync($"{BaseUrl}/signin");

            await Page.Locator("#username").ClickAsync();

            var expectedUsers = new[]
            {
                "demouser",
                "image_not_loading_user",
                "existing_orders_user",
                "fav_user",
                "locked_user"
            };

            foreach (var user in expectedUsers)
            {
                await Expect(Page.GetByRole(AriaRole.Option, new() { Name = user, Exact = true })).ToBeVisibleAsync();
            }

            await Expect(Page.GetByRole(AriaRole.Option)).ToHaveCountAsync(5);
        }

        [Test]
        [Description("Sign In page does not allow direct text entry in Username")]
        public async Task UsernameFieldDoesNotAllowArbitraryTextEntry()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-172]]");
            await Page.GotoAsync($"{BaseUrl}/signin");

            await Page.Locator("#username").ClickAsync();
            await Page.Locator("#react-select-2-input").FillAsync("fakeuser123");

            await Expect(Page.GetByRole(AriaRole.Option, new() { Name = "fakeuser123" })).Not.ToBeVisibleAsync();

            await Page.Keyboard.PressAsync("Escape");
            await Page.Locator("#login-btn").ClickAsync();

            await Expect(Page.Locator("#login-btn")).ToBeVisibleAsync();
        }

        [Test]
        [Description("Log In button with no credentials selected shows error")]
        public async Task LoginWithNoCredentialsShowsError()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-173]]");
            await Page.GotoAsync($"{BaseUrl}/signin");

            await Page.Locator("#login-btn").ClickAsync();

            await Expect(Page.Locator("h3.api-error")).ToBeVisibleAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*signin"));
        }

        [Test]
        [Description("Sign in with username selected but no password shows error")]
        public async Task LoginWithUsernameButNoPasswordShowsError()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-174]]");
            await Page.GotoAsync($"{BaseUrl}/signin");

            await SelectDropdownOption("username", "demouser");
            await Page.Locator("#login-btn").ClickAsync();

            await Expect(Page.Locator("h3.api-error")).ToBeVisibleAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*signin"));
        }

        [Test]
        [Description("Sign in page is accessible without authentication and redirects when already logged in")]
        public async Task SignInPageAccessibilityAndRedirectWhenLoggedIn()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-175]]");
            // Not logged in: /signin loads normally
            await Page.GotoAsync($"{BaseUrl}/signin");
            await Expect(Page).ToHaveURLAsync(new Regex(@".*signin"));
            await Expect(Page.Locator("#login-btn")).ToBeVisibleAsync();

            // Sign in
            await SignIn("demouser");
            await Expect(Page).ToHaveURLAsync(new Regex(@"testathon\.live/(\?|$)"));

            // Already logged in: revisiting /signin redirects to home
            await Page.GotoAsync($"{BaseUrl}/signin");
            await Expect(Page).ToHaveURLAsync(new Regex(@"testathon\.live/(\?|$)"));
        }
    }
}

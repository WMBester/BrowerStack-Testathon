using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("navigation")]
    public class NavigationTests : PageTest
    {
        private const string BaseUrl = "https://testathon.live";
        private const string Password = "testingisfun99";

        private async Task SelectDropdownOption(string containerId, string optionText)
        {
            await Page.Locator($"#{containerId}").ClickAsync();
            await Page.GetByRole(AriaRole.Option, new() { Name = optionText, Exact = true }).ClickAsync();
        }

        private async Task SignIn(string username)
        {
            await Page.GotoAsync($"{BaseUrl}/signin");
            await SelectDropdownOption("username", username);
            await SelectDropdownOption("password", Password);
            await Page.Locator("#login-btn").ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@"testathon\.live/(\?|$)"));
        }

        private async Task AddToCart(int productIndex = 0)
        {
            await Page.WaitForSelectorAsync(".shelf-item__buy-btn");
            await Page.Locator(".shelf-item__buy-btn").Nth(productIndex).ClickAsync();
            await Page.WaitForTimeoutAsync(600);
        }

        private async Task CompleteCheckout()
        {
            await AddToCart(0);
            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.Locator("#firstNameInput").FillAsync("John");
            await Page.Locator("#lastNameInput").FillAsync("Doe");
            await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
            await Page.Locator("#provinceInput").FillAsync("California");
            await Page.Locator("#postCodeInput").FillAsync("90001");
            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/confirmation"), new() { Timeout = 15000 });
        }

        [Test]
        [Description("Header navigation links are accessible when signed in")]
        public async Task HeaderNavigationLinksAccessibleWhenSignedIn()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-223]]");
            await SignIn("demouser");

            // All nav links should be visible
            await Expect(Page.Locator("a#offers")).ToBeVisibleAsync();
            await Expect(Page.Locator("a#orders")).ToBeVisibleAsync();
            await Expect(Page.Locator("a#favourites")).ToBeVisibleAsync();

            // Click Offers and verify navigation
            await Page.Locator("a#offers").ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/offers"));

            // Click Orders and verify navigation
            await Page.Locator("a#orders").ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/orders"));

            // Click Favourites and verify navigation
            await Page.Locator("a#favourites").ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/favourites"));
        }

        [Test]
        [Description("Sign out clears session and redirects to sign in")]
        public async Task SignOutClearsSessionAndRedirects()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-224]]");
            await SignIn("demouser");

            // Click Logout
            await Page.Locator("a#logout").ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/signin"));

            // Verify session is cleared — protected routes should redirect to signin
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/signin"));

            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/signin"));
        }

        [Test]
        [Description("Cart count in header reflects items added and resets after checkout")]
        public async Task CartCountReflectsItemsAddedAndResetsAfterCheckout()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-225]]");
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item__buy-btn");

            // Initially empty
            var countBefore = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(countBefore, Is.EqualTo("0"), "Cart should start at 0");

            // Add one product
            await AddToCart(0);
            var countAfterOne = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(countAfterOne, Is.EqualTo("1"), "Cart should show 1 after first item");

            // Add another product
            await AddToCart(1);
            var countAfterTwo = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(countAfterTwo, Is.EqualTo("2"), "Cart should show 2 after second item");

            // Complete checkout and verify cart resets
            await CompleteCheckout();
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var countAfterCheckout = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(countAfterCheckout, Is.EqualTo("0"), "Cart should reset to 0 after checkout");
        }

        [Test]
        [Description("StackDemo logo navigates to home page")]
        public async Task StackDemoLogoNavigatesToHome()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-226]]");
            await SignIn("demouser");

            // Navigate away from home
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/orders"));

            // Click the logo
            await Page.Locator("a.Navbar_logo__26S5Y").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(@"testathon\.live/(\?|$)"));
        }

        [Test]
        [Description("Footer is present on all pages")]
        public async Task FooterIsPresentOnAllPages()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-227]]");
            await SignIn("demouser");

            // Home page
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page.Locator("footer").First).ToBeVisibleAsync();

            // Orders page
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page.Locator("footer").First).ToBeVisibleAsync();

            // Checkout page (with item in cart)
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForSelectorAsync(".shelf-item__buy-btn");
            await Page.Locator(".shelf-item__buy-btn").First.ClickAsync();
            await Page.WaitForTimeoutAsync(600);
            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page.Locator("footer").First).ToBeVisibleAsync();

            // Favourites page
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page.Locator("footer").First).ToBeVisibleAsync();
        }
    }
}

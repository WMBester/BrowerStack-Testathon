using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("favourites")]
    public class FavouritesTests : PageTest
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

        [Test]
        [Description("TC-FA-001: Favourites page requires authentication")]
        public async Task FavouritesPageRequiresAuthentication()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-189]]");
            // Navigate to /favourites without being signed in
            await Page.GotoAsync($"{BaseUrl}/favourites");

            // Should redirect to signin with favourites=true query param
            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?favourites=true"));

            // Sign in and verify redirect back to favourites
            await SelectDropdownOption("username", "demouser");
            await SelectDropdownOption("password", Password);
            await Page.Locator("#login-btn").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/favourites"));
        }

        [Test]
        [Description("TC-FA-002: fav_user sees pre-seeded favourites")]
        public async Task FavUserSeesPreSeededFavourites()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-190]]");
            await SignIn("fav_user");
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // At least one product should be listed
            var items = Page.Locator(".shelf-item");
            var count = await items.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "fav_user should have pre-seeded favourited products");

            // Each item should show image, title, and price
            for (int i = 0; i < count; i++)
            {
                var item = items.Nth(i);
                await Expect(item.Locator(".shelf-item__thumb img")).ToBeVisibleAsync();
                await Expect(item.Locator(".shelf-item__title")).ToBeVisibleAsync();
                await Expect(item.Locator(".shelf-item__price")).ToBeVisibleAsync();
            }
        }

        [Test]
        [Description("TC-FA-003: Empty favourites state for demouser with no items favourited")]
        public async Task EmptyFavouritesStateForNewUser()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-191]]");
            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify empty state — "0 Product(s) found."
            var foundText = await Page.Locator("small.products-found span").InnerTextAsync();
            Assert.That(foundText, Does.Contain("0 Product(s) found"), "Favourites should be empty for demouser");

            // No product cards rendered
            var itemCount = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(itemCount, Is.EqualTo(0), "No product cards should be shown in empty favourites");
        }

        [Test]
        [Description("TC-FA-004: Add to cart from favourites page")]
        public async Task AddToCartFromFavouritesPage()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-192]]");
            await SignIn("fav_user");
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var cartBefore = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();

            // Click Add to cart on the first favourited item
            await Page.Locator(".shelf-item__buy-btn").First.ClickAsync();
            await Page.WaitForTimeoutAsync(600);

            var cartAfter = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(int.Parse(cartAfter), Is.GreaterThan(int.Parse(cartBefore)),
                "Cart count should increment after adding from favourites");
        }
    }
}

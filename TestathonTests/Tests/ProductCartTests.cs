using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("product-cart")]
    public class ProductCartTests : PageTest
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

        [Test]
        [Description("Add a single product to cart")]
        public async Task AddSingleProductToCart()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-184]]");
            await SignIn("demouser");

            var cartBadgeBefore = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(cartBadgeBefore, Is.EqualTo("0"), "Cart should be empty before adding");

            await AddToCart(0);

            var cartBadgeAfter = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(cartBadgeAfter, Is.EqualTo("1"), "Cart count should be 1 after adding one product");
        }

        [Test]
        [Description("Add multiple different products to cart")]
        public async Task AddMultipleDifferentProductsToCart()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-185]]");
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item__buy-btn");

            // Add first product
            await AddToCart(0);
            var countAfterFirst = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(countAfterFirst, Is.EqualTo("1"), "Cart should show 1 after first product");

            // Add second product
            await AddToCart(1);
            var countAfterSecond = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(countAfterSecond, Is.EqualTo("2"), "Cart should show 2 after second product");

            // Verify both products appear in the float cart
            var cartItems = Page.Locator(".float-cart__shelf-container .shelf-item");
            await Expect(cartItems).ToHaveCountAsync(2);
        }

        [Test]
        [Description("Add the same product to cart multiple times")]
        public async Task AddSameProductToCartMultipleTimes()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-186]]");
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item__buy-btn");

            // Add first product twice
            await AddToCart(0);
            var countAfterFirst = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(int.Parse(countAfterFirst), Is.GreaterThanOrEqualTo(1));

            await AddToCart(0);
            var countAfterSecond = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(int.Parse(countAfterSecond), Is.GreaterThan(int.Parse(countAfterFirst)),
                "Cart count should increase when same product added again");
        }

        [Test]
        [Description("Add to Favourites from product listing")]
        public async Task AddToFavouritesFromProductListing()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-187]]");
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-stopper button");

            // Click heart on first product
            await Page.Locator(".shelf-stopper button").First.ClickAsync();
            await Page.WaitForTimeoutAsync(600);

            // Verify the button now has 'clicked' class (product is favourited)
            await Expect(Page.Locator(".shelf-stopper button.clicked").First).ToBeVisibleAsync();

            // Navigate to favourites and verify product appears
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var favCount = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(favCount, Is.GreaterThan(0), "Favourited product should appear in /favourites");
        }

        [Test]
        [Description("Remove a product from favourites")]
        public async Task RemoveProductFromFavourites()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-188]]");
            // fav_user has pre-seeded favourites
            await SignIn("fav_user");
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var countBefore = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(countBefore, Is.GreaterThan(0), "fav_user should have pre-seeded favourites");

            // Click the faved heart button on first item to unfavourite
            await Page.Locator(".shelf-stopper button.clicked").First.ClickAsync();
            await Page.WaitForTimeoutAsync(600);

            var countAfter = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(countAfter, Is.LessThan(countBefore), "Product should be removed from favourites");
        }
    }
}

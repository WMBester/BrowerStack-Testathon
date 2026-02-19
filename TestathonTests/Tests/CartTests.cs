using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("cart")]
    public class CartTests : PageTest
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
        [Description("Cart persists items when navigating away and back")]
        public async Task CartPersistsItemsAcrossNavigation()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-193]]");
            await SignIn("demouser");
            await AddToCart(0);

            var cartCountAfterAdd = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(cartCountAfterAdd, Is.EqualTo("1"), "Cart should show 1 after adding");

            // Navigate away
            await Page.GotoAsync($"{BaseUrl}/favourites");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Navigate back home
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var cartCountAfterNav = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(cartCountAfterNav, Is.EqualTo("1"), "Cart count should still be 1 after navigation");
        }

        [Test]
        [Description("Cart is cleared after successful checkout")]
        public async Task CartClearedAfterSuccessfulCheckout()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-194]]");
            await SignIn("demouser");
            await AddToCart(0);

            var cartCountBefore = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(int.Parse(cartCountBefore), Is.GreaterThan(0), "Cart should have items before checkout");

            await CompleteCheckout();

            // Navigate back to home
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var cartCountAfter = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(cartCountAfter, Is.EqualTo("0"), "Cart should be empty after checkout");
        }

        [Test]
        [Description("Cart is empty when no items added — checkout shows empty state")]
        public async Task CartEmptyWhenNoItemsAdded()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-195]]");
            await SignIn("demouser");

            // Navigate to checkout with an empty cart
            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Either empty state is shown or form has no order items
            var submitBtn = Page.Locator("#checkout-shipping-continue");
            var submitVisible = await submitBtn.IsVisibleAsync();

            if (submitVisible)
            {
                // If submit is shown, the order summary should have no items
                var orderItems = Page.Locator("ul.productList li");
                var itemCount = await orderItems.CountAsync();
                Assert.That(itemCount, Is.EqualTo(0), "Order summary should show 0 items when cart is empty");
            }
            else
            {
                // If submit is not visible, that is also an acceptable empty-cart state
                Assert.Pass("Submit button not shown for empty cart — acceptable behaviour");
            }
        }
    }
}

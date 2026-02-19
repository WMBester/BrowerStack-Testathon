using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("checkout")]
    public class CheckoutTests : PageTest
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

        private async Task GoToCheckoutWithItem()
        {
            await AddToCart(0);
            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        [Test]
        [Description("TC-CH-001: Successful checkout with all required fields filled")]
        public async Task SuccessfulCheckoutWithAllFields()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            await Page.Locator("#firstNameInput").FillAsync("John");
            await Page.Locator("#lastNameInput").FillAsync("Doe");
            await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
            await Page.Locator("#provinceInput").FillAsync("California");
            await Page.Locator("#postCodeInput").FillAsync("90001");

            await Page.Locator("#checkout-shipping-continue").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/confirmation"), new() { Timeout = 15000 });
        }

        [Test]
        [Description("TC-CH-002: Checkout blocked when First Name is empty")]
        public async Task CheckoutBlockedWhenFirstNameEmpty()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            // Leave First Name empty; fill all others
            await Page.Locator("#lastNameInput").FillAsync("Doe");
            await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
            await Page.Locator("#provinceInput").FillAsync("California");
            await Page.Locator("#postCodeInput").FillAsync("90001");

            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            // Form should not submit — must remain on /checkout
            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
            await Expect(Page.Locator("#checkout-shipping-continue")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-CH-003: Checkout blocked when Last Name is empty")]
        public async Task CheckoutBlockedWhenLastNameEmpty()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            await Page.Locator("#firstNameInput").FillAsync("John");
            // Leave Last Name empty
            await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
            await Page.Locator("#provinceInput").FillAsync("California");
            await Page.Locator("#postCodeInput").FillAsync("90001");

            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
            await Expect(Page.Locator("#checkout-shipping-continue")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-CH-004: Checkout blocked when Address is empty")]
        public async Task CheckoutBlockedWhenAddressEmpty()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            await Page.Locator("#firstNameInput").FillAsync("John");
            await Page.Locator("#lastNameInput").FillAsync("Doe");
            // Leave Address empty
            await Page.Locator("#provinceInput").FillAsync("California");
            await Page.Locator("#postCodeInput").FillAsync("90001");

            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
            await Expect(Page.Locator("#checkout-shipping-continue")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-CH-005: Checkout blocked when State/Province is empty")]
        public async Task CheckoutBlockedWhenStateEmpty()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            await Page.Locator("#firstNameInput").FillAsync("John");
            await Page.Locator("#lastNameInput").FillAsync("Doe");
            await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
            // Leave State/Province empty
            await Page.Locator("#postCodeInput").FillAsync("90001");

            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
            await Expect(Page.Locator("#checkout-shipping-continue")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-CH-006: Checkout blocked when Postal Code is empty")]
        public async Task CheckoutBlockedWhenPostalCodeEmpty()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            await Page.Locator("#firstNameInput").FillAsync("John");
            await Page.Locator("#lastNameInput").FillAsync("Doe");
            await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
            await Page.Locator("#provinceInput").FillAsync("California");
            // Leave Postal Code empty

            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
            await Expect(Page.Locator("#checkout-shipping-continue")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-CH-007: Checkout blocked when all fields are empty")]
        public async Task CheckoutBlockedWhenAllFieldsEmpty()
        {
            await SignIn("demouser");
            await GoToCheckoutWithItem();

            await Expect(Page.Locator("#firstNameInput")).ToBeVisibleAsync();

            // Submit with all fields empty
            await Page.Locator("#checkout-shipping-continue").ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/checkout"));
            await Expect(Page.Locator("#checkout-shipping-continue")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-CH-008: Order summary shows correct items and total")]
        public async Task OrderSummaryShowsCorrectItemsAndTotal()
        {
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item__buy-btn");

            // Capture prices of first two products before adding
            var price1Text = await Page.Locator(".shelf-item__price b").Nth(0).InnerTextAsync();
            var price2Text = await Page.Locator(".shelf-item__price b").Nth(1).InnerTextAsync();
            var price1 = int.Parse(price1Text.Trim());
            var price2 = int.Parse(price2Text.Trim());

            await AddToCart(0);
            await AddToCart(1);

            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify both items in order summary
            var orderItems = Page.Locator("ul.productList li.productList-item");
            await Expect(orderItems).ToHaveCountAsync(2);

            // Verify total
            var totalText = await Page.Locator("span.cart-priceItem-value span").InnerTextAsync();
            var expectedTotal = price1 + price2;
            Assert.That(totalText, Does.Contain(expectedTotal.ToString()),
                $"Total should be ${expectedTotal}.00");
        }

        [Test]
        [Description("TC-CH-009: Checkout requires authentication")]
        public async Task CheckoutRequiresAuthentication()
        {
            await Page.GotoAsync($"{BaseUrl}/checkout");

            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?checkout=true"));
        }

        [Test]
        [Description("TC-CH-010: Checkout with empty cart shows empty state or submit unavailable")]
        public async Task CheckoutWithEmptyCartShowsEmptyState()
        {
            await SignIn("demouser");

            await Page.GotoAsync($"{BaseUrl}/checkout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // When cart is empty, either the form is not shown or the order summary has no items
            var formVisible = await Page.Locator("#firstNameInput").IsVisibleAsync();

            if (formVisible)
            {
                var orderItems = Page.Locator("ul.productList li");
                var itemCount = await orderItems.CountAsync();
                Assert.That(itemCount, Is.EqualTo(0), "Order summary should show no items for empty cart");
            }
            else
            {
                // Form not shown for empty cart — acceptable
                Assert.Pass("Checkout form not shown when cart is empty");
            }
        }
    }
}

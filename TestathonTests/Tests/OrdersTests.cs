using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("orders")]
    public class OrdersTests : PageTest
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
        [Description("Orders page requires authentication")]
        public async Task OrdersPageRequiresAuthentication()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-211]]");
            await Page.GotoAsync($"{BaseUrl}/orders");

            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?orders=true"));

            // Sign in and verify redirect back
            await SelectDropdownOption("username", "demouser");
            await SelectDropdownOption("password", Password);
            await Page.Locator("#login-btn").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/orders"));
        }

        [Test]
        [Description("existing_orders_user sees pre-seeded order history")]
        public async Task ExistingOrdersUserSeesOrderHistory()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-212]]");
            await SignIn("existing_orders_user");
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // At least one order card should exist
            var orderCards = Page.Locator("div.order");
            var count = await orderCards.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "existing_orders_user should have at least one order");

            // Verify first order shows: placed date, total, ship-to
            var firstOrder = orderCards.First;
            var labels = firstOrder.Locator("span.a-color-secondary.label");
            var values = firstOrder.Locator("span.a-color-secondary.value");

            await Expect(labels.Nth(0)).ToHaveTextAsync("Order placed");
            await Expect(labels.Nth(1)).ToHaveTextAsync("Total");
            await Expect(labels.Nth(2)).ToHaveTextAsync("Ship to");

            // Verify values are not empty
            var placedValue = await values.Nth(0).InnerTextAsync();
            var totalValue = await values.Nth(1).InnerTextAsync();
            var shipToValue = await values.Nth(2).InnerTextAsync();
            Assert.That(placedValue, Is.Not.Empty);
            Assert.That(totalValue, Is.Not.Empty);
            Assert.That(shipToValue, Is.Not.Empty);

            // Verify delivered status
            await Expect(firstOrder.Locator("div.shipment.shipment-is-delivered")).ToBeVisibleAsync();
            var deliveredText = await firstOrder.Locator("span.a-size-medium.a-color-base.a-text-bold").InnerTextAsync();
            Assert.That(deliveredText, Does.StartWith("Delivered"), "Order should show a Delivered status");
        }

        [Test]
        [Description("Orders page shows empty state for user with no orders")]
        public async Task OrdersPageShowsEmptyStateForNewUser()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-213]]");
            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify empty state
            await Expect(Page.Locator("div.orders-listing h2")).ToHaveTextAsync("No orders found");

            // No order cards
            var orderCards = await Page.Locator("div.order").CountAsync();
            Assert.That(orderCards, Is.EqualTo(0), "No order cards should be shown");
        }

        [Test]
        [Description("Order placed via checkout appears in order history")]
        public async Task NewOrderAppearsInOrderHistory()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-214]]");
            await SignIn("demouser");
            await CompleteCheckout();

            // Navigate to orders
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // New order should now appear
            var orderCards = Page.Locator("div.order");
            var count = await orderCards.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "New order should appear in order history after checkout");
        }

        [Test]
        [Description("Order details display correct product information")]
        public async Task OrderDetailsDisplayCorrectProductInformation()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-215]]");
            await SignIn("existing_orders_user");
            await Page.GotoAsync($"{BaseUrl}/orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var orderCards = Page.Locator("div.order");
            Assert.That(await orderCards.CountAsync(), Is.GreaterThan(0));

            // Check first order's item details
            var firstOrder = orderCards.First;

            // Product image
            await Expect(firstOrder.Locator("img.item-image").First).ToBeVisibleAsync();

            // Product title row
            var titleRow = firstOrder.Locator(".a-fixed-left-grid-col.a-col-right .a-row").First;
            var titleText = await titleRow.InnerTextAsync();
            Assert.That(titleText, Does.Contain("Title:"), "Order item should show a Title field");

            // Product price
            await Expect(firstOrder.Locator("span.a-size-small.a-color-price").First).ToBeVisibleAsync();
            var priceText = await firstOrder.Locator("span.a-size-small.a-color-price").First.InnerTextAsync();
            Assert.That(priceText, Does.StartWith("$"), "Price should be formatted as currency");
        }
    }
}

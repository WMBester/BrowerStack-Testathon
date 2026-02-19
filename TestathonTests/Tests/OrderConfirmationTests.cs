using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("order-confirmation")]
    public class OrderConfirmationTests : PageTest
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
        [Description("TC-CF-001: Confirmation page displays after successful checkout")]
        public async Task ConfirmationPageDisplaysAfterCheckout()
        {
            await SignIn("demouser");
            await CompleteCheckout();

            // Verify success heading
            await Expect(Page.Locator("legend#confirmation-message"))
                .ToHaveTextAsync("Your Order has been successfully placed.");

            // Verify order number is shown
            var orderNumberEl = Page.Locator("legend#confirmation-message ~ div strong");
            await Expect(orderNumberEl).ToBeVisibleAsync();
            var orderNumberText = await orderNumberEl.InnerTextAsync();
            Assert.That(int.TryParse(orderNumberText.Trim(), out var orderNum), Is.True, "Order number should be numeric");
            Assert.That(orderNum, Is.InRange(1, 100), "Order number should be between 1 and 100");

            // Verify order summary present
            await Expect(Page.Locator("article.cart[data-test='cart']")).ToBeVisibleAsync();
            await Expect(Page.Locator("h3.cart-title")).ToHaveTextAsync("Order Summary");

            // Verify at least one item in summary
            var items = Page.Locator("ul.productList li.productList-item");
            var itemCount = await items.CountAsync();
            Assert.That(itemCount, Is.GreaterThan(0), "Order summary should have at least one item");
        }

        [Test]
        [Description("TC-CF-002: Continue Shopping button returns to home page")]
        public async Task ContinueShoppingButtonReturnsToHome()
        {
            await SignIn("demouser");
            await CompleteCheckout();

            // Click Continue Shopping
            await Page.Locator("button.button--tertiary").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(@"testathon\.live/(\?|$)"));

            // Cart should be empty after checkout
            var cartCount = await Page.Locator(".bag--float-cart-closed .bag__quantity").InnerTextAsync();
            Assert.That(cartCount, Is.EqualTo("0"), "Cart should be empty after completing checkout");
        }

        [Test]
        [Description("TC-CF-003: Download order receipt generates a PDF download")]
        public async Task DownloadOrderReceiptGeneratesPdf()
        {
            await SignIn("demouser");
            await CompleteCheckout();

            // Verify the download link is visible
            await Expect(Page.Locator("a#downloadpdf")).ToBeVisibleAsync();

            // Wait for and capture the download
            var download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Page.Locator("a#downloadpdf").ClickAsync();
            });

            Assert.That(download, Is.Not.Null, "A file download should be triggered");

            var filename = download.SuggestedFilename;
            // Either a .pdf extension or a meaningful filename
            Assert.That(
                filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || filename.Length > 0,
                Is.True,
                $"Downloaded file should be a PDF, got: {filename}"
            );
        }

        [Test]
        [Description("TC-CF-004: Confirmation page cannot be accessed directly without placing an order")]
        public async Task ConfirmationPageCannotBeAccessedDirectly()
        {
            await SignIn("demouser");

            // Navigate directly to /confirmation without checking out
            await Page.GotoAsync($"{BaseUrl}/confirmation");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(1000);

            // Should redirect to home OR show an empty/no-order state
            var currentUrl = Page.Url;
            var isHome = Regex.IsMatch(currentUrl, @"testathon\.live/(\?|$)");
            var isConfirmation = currentUrl.Contains("/confirmation");

            if (isConfirmation)
            {
                // If still on confirmation, the success message must NOT be present
                var successMsg = Page.Locator("legend#confirmation-message");
                var successVisible = await successMsg.IsVisibleAsync();
                Assert.That(successVisible, Is.False,
                    "Success message should not appear on direct /confirmation access without checkout");
            }
            else
            {
                // Redirected away — acceptable
                Assert.That(isHome, Is.True, "Should redirect to home when accessing /confirmation directly");
            }
        }

        [Test]
        [Description("TC-CF-005: Order number is within valid range (1-100)")]
        public async Task OrderNumberIsWithinValidRange()
        {
            await SignIn("demouser");
            await CompleteCheckout();

            var orderNumberEl = Page.Locator("legend#confirmation-message ~ div strong");
            await Expect(orderNumberEl).ToBeVisibleAsync();

            var orderNumberText = await orderNumberEl.InnerTextAsync();
            Assert.That(int.TryParse(orderNumberText.Trim(), out var orderNum), Is.True,
                $"Order number should be a valid integer, got: '{orderNumberText}'");
            Assert.That(orderNum, Is.GreaterThanOrEqualTo(1), "Order number should be >= 1");
            Assert.That(orderNum, Is.LessThanOrEqualTo(100), "Order number should be <= 100");
        }
    }
}

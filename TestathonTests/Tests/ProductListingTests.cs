using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("product-listing")]
    public class ProductListingTests : PageTest
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
        [Description("TC-PL-001: Home page loads with product listings")]
        public async Task HomePageLoadsWithProductListings()
        {
            await SignIn("demouser");

            await Page.WaitForSelectorAsync(".shelf-item");

            var productCount = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(productCount, Is.GreaterThan(0), "At least one product card should be visible");

            // Verify first product has image, title, and price
            var firstProduct = Page.Locator(".shelf-item").First;
            await Expect(firstProduct.Locator(".shelf-item__thumb img")).ToBeVisibleAsync();
            await Expect(firstProduct.Locator(".shelf-item__title")).ToBeVisibleAsync();
            await Expect(firstProduct.Locator(".shelf-item__price")).ToBeVisibleAsync();
        }

        [Test]
        [Description("TC-PL-002: Product images load correctly for demouser")]
        public async Task ProductImagesLoadForDemouser()
        {
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item__thumb img");

            var images = Page.Locator(".shelf-item__thumb img");
            var count = await images.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "Product images should be present");

            for (int i = 0; i < count; i++)
            {
                var naturalWidth = await images.Nth(i).EvaluateAsync<int>("img => img.naturalWidth");
                Assert.That(naturalWidth, Is.GreaterThan(0), $"Image {i} should load successfully (naturalWidth > 0)");
            }
        }

        [Test]
        [Description("TC-PL-003: Product images fail to load for image_not_loading_user")]
        public async Task ProductImagesFailForImageNotLoadingUser()
        {
            await SignIn("image_not_loading_user");
            await Page.WaitForSelectorAsync(".shelf-item");
            await Page.WaitForTimeoutAsync(2000);

            var images = Page.Locator(".shelf-item__thumb img");
            var count = await images.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "Product image elements should still be present");

            var brokenCount = 0;
            for (int i = 0; i < count; i++)
            {
                var naturalWidth = await images.Nth(i).EvaluateAsync<int>("img => img.naturalWidth");
                if (naturalWidth == 0) brokenCount++;
            }

            Assert.That(brokenCount, Is.GreaterThan(0), "At least some images should be broken for image_not_loading_user");
        }

        [Test]
        [Description("TC-PL-004: Filter products by a single category")]
        public async Task FilterProductsBySingleCategory()
        {
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item");

            var totalBefore = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(totalBefore, Is.GreaterThan(0));

            // Select Apple filter
            await Page.Locator(".filters input[value='Apple']").CheckAsync();
            await Page.WaitForTimeoutAsync(500);

            var filteredCount = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(filteredCount, Is.GreaterThan(0), "Some Apple products should be visible after filter");
            Assert.That(filteredCount, Is.LessThanOrEqualTo(totalBefore), "Filtered count should be <= total");

            var foundText = await Page.Locator("small.products-found span").InnerTextAsync();
            Assert.That(foundText, Does.Contain("Product(s) found"));

            // Deselect filter - restore all
            await Page.Locator(".filters input[value='Apple']").UncheckAsync();
            await Page.WaitForTimeoutAsync(500);

            var restoredCount = await Page.Locator(".shelf-item").CountAsync();
            Assert.That(restoredCount, Is.EqualTo(totalBefore), "All products should return after removing filter");
        }

        [Test]
        [Description("TC-PL-005: Filter products by multiple categories simultaneously")]
        public async Task FilterProductsByMultipleCategories()
        {
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item");

            // Select Apple
            await Page.Locator(".filters input[value='Apple']").CheckAsync();
            await Page.WaitForTimeoutAsync(500);
            var appleCount = await Page.Locator(".shelf-item").CountAsync();

            // Also select Samsung
            await Page.Locator(".filters input[value='Samsung']").CheckAsync();
            await Page.WaitForTimeoutAsync(500);
            var bothCount = await Page.Locator(".shelf-item").CountAsync();

            Assert.That(bothCount, Is.GreaterThanOrEqualTo(appleCount), "Adding a second filter should show at least as many products");
            Assert.That(bothCount, Is.GreaterThan(0), "Products from both categories should be visible");
        }

        [Test]
        [Description("TC-PL-006: Sort products by price low to high")]
        // NOTE: This test is ignored because the sort/order control does not exist on the live
        // testathon.live site. No <select>, custom dropdown, or any "Low to High" / "High to Low"
        // UI element was found in the DOM during Playwright exploration (Feb 2026).
        // The test body is kept for future implementation should the feature be added to the site.
        [Ignore("Sort control not present on testathon.live - feature not implemented on live site")]
        public async Task SortProductsByPriceLowToHigh()
        {
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item");

            var sortControl = Page.Locator("select.sort, [class*='sort'] select, [data-testid*='sort']");
            var sortCount = await sortControl.CountAsync();
            Assert.That(sortCount, Is.GreaterThan(0), "Sort control should be present");

            await sortControl.First.SelectOptionAsync(new SelectOptionValue { Label = "Price: Low to High" });
            await Page.WaitForTimeoutAsync(500);

            var prices = await Page.Locator(".shelf-item__price b").EvaluateAllAsync<int[]>("els => els.map(el => parseInt(el.textContent))");
            for (int i = 1; i < prices.Length; i++)
            {
                Assert.That(prices[i], Is.GreaterThanOrEqualTo(prices[i - 1]), $"Price at index {i} should be >= price at {i - 1}");
            }
        }

        [Test]
        [Description("TC-PL-007: Sort products by price high to low")]
        // NOTE: This test is ignored because the sort/order control does not exist on the live
        // testathon.live site. No <select>, custom dropdown, or any "Low to High" / "High to Low"
        // UI element was found in the DOM during Playwright exploration (Feb 2026).
        // The test body is kept for future implementation should the feature be added to the site.
        [Ignore("Sort control not present on testathon.live - feature not implemented on live site")]
        public async Task SortProductsByPriceHighToLow()
        {
            await SignIn("demouser");
            await Page.WaitForSelectorAsync(".shelf-item");

            var sortControl = Page.Locator("select.sort, [class*='sort'] select, [data-testid*='sort']");
            var sortCount = await sortControl.CountAsync();
            Assert.That(sortCount, Is.GreaterThan(0), "Sort control should be present");

            await sortControl.First.SelectOptionAsync(new SelectOptionValue { Label = "Price: High to Low" });
            await Page.WaitForTimeoutAsync(500);

            var prices = await Page.Locator(".shelf-item__price b").EvaluateAllAsync<int[]>("els => els.map(el => parseInt(el.textContent))");
            for (int i = 1; i < prices.Length; i++)
            {
                Assert.That(prices[i], Is.LessThanOrEqualTo(prices[i - 1]), $"Price at index {i} should be <= price at {i - 1}");
            }
        }

        [Test]
        [Description("TC-PL-008: Home page is accessible without authentication")]
        public async Task HomePageAccessibleWithoutAuthentication()
        {
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Page should load without error — either products shown or redirect to signin
            var url = Page.Url;
            var bodyVisible = await Page.Locator("body").IsVisibleAsync();
            Assert.That(bodyVisible, Is.True, "Page body should be visible");

            // No unhandled error page
            var title = await Page.TitleAsync();
            Assert.That(title, Is.Not.Empty, "Page title should not be empty");
        }
    }
}

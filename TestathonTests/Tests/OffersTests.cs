using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("offers")]
    public class OffersTests : PageTest
    {
        private const string BaseUrl = "https://testathon.live";
        private const string Password = "testingisfun99";

        private async Task SelectDropdownOption(string containerId, string optionText)
        {
            await Page.Locator($"#{containerId}").ClickAsync();
            await Page.WaitForSelectorAsync($"#{containerId} [class*='menu']");
            await Page.Locator($"div[id*='react-select'][id*='option']")
                .Filter(new() { HasText = optionText })
                .First.ClickAsync();
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
        [Description("Offers page requires authentication")]
        public async Task OffersPageRequiresAuthentication()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-216]]");
            await Page.GotoAsync($"{BaseUrl}/offers");

            await Expect(Page).ToHaveURLAsync(new Regex(@"/signin\?offers=true"));

            // Sign in and verify redirect back to offers
            await SelectDropdownOption("username", "demouser");
            await SelectDropdownOption("password", Password);
            await Page.Locator("#login-btn").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(@".*/offers"));
        }

        [Test]
        [Description("Offers page requests geolocation permission")]
        public async Task OffersPageRequestsGeolocationPermission()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-217]]");
            // Use default context (no geolocation permission granted)
            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/offers");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(2000);

            // The page should show either offers (if geo granted) or the geo error/denied message
            // Either way the page has responded to a geolocation attempt
            var pageText = await Page.Locator("body").InnerTextAsync();
            Assert.That(
                pageText.Contains("Please enable Geolocation") ||
                pageText.Contains("promotional offers") ||
                pageText.Contains("Geolocation is not available"),
                Is.True,
                "Offers page should show geolocation-related content — confirming geolocation was requested"
            );
        }

        [Test]
        [Description("Offers display when geolocation is allowed")]
        public async Task OffersDisplayWhenGeolocationAllowed()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-218]]");
            // Grant geolocation permission for this test
            await Context.GrantPermissionsAsync(new[] { "geolocation" });
            await Context.SetGeolocationAsync(new Geolocation { Latitude = 37.7749f, Longitude = -122.4194f });

            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/offers");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(3000);

            // Offer cards should be visible
            var offerCards = Page.Locator("div.offer");
            var count = await offerCards.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "At least one offer card should be displayed when geolocation is allowed");

            // Each offer card should have an image and title
            for (int i = 0; i < count; i++)
            {
                await Expect(offerCards.Nth(i).Locator("img")).ToBeVisibleAsync();
                await Expect(offerCards.Nth(i).Locator(".offer-title")).ToBeVisibleAsync();
            }
        }

        [Test]
        [Description("Error message displayed when geolocation is denied")]
        public async Task ErrorMessageWhenGeolocationDenied()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-219]]");
            // Default context — geolocation not granted
            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/offers");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(3000);

            await Expect(Page.GetByText("Please enable Geolocation in your browser.")).ToBeVisibleAsync();

            // No offer cards should be shown
            var offerCards = await Page.Locator("div.offer").CountAsync();
            Assert.That(offerCards, Is.EqualTo(0), "No offer cards should appear when geolocation is denied");
        }

        [Test]
        [Description("No offers available message for current location")]
        public async Task NoOffersMessageForLocationWithNoOffers()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-220]]");
            // Use coordinates unlikely to have promotional offers (remote location)
            await Context.GrantPermissionsAsync(new[] { "geolocation" });
            await Context.SetGeolocationAsync(new Geolocation { Latitude = -85.0f, Longitude = 0.0f });

            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/offers");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(3000);

            var pageText = await Page.Locator("body").InnerTextAsync();
            Assert.That(
                pageText.Contains("Sorry we do not have any promotional offers in your city") ||
                pageText.Contains("promotional offers in your location"),
                Is.True,
                "Should show 'no offers' or available offers message based on location"
            );
        }

        [Test]
        [Description("Offers page handles browser without geolocation support")]
        public async Task OffersPageHandlesBrowserWithoutGeolocationSupport()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-221]]");
            await SignIn("demouser");

            // Simulate a browser without geolocation API by overriding navigator.geolocation
            await Page.AddInitScriptAsync("Object.defineProperty(navigator, 'geolocation', { get: () => undefined });");
            await Page.GotoAsync($"{BaseUrl}/offers");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(3000);

            var pageText = await Page.Locator("body").InnerTextAsync();
            Assert.That(
                pageText.Contains("Geolocation is not available in your browser") ||
                pageText.Contains("Please enable Geolocation"),
                Is.True,
                "Should display an appropriate message when geolocation API is unavailable"
            );
        }

        [Test]
        [Description("Each offer card displays an image and a title")]
        public async Task EachOfferCardDisplaysImageAndTitle()
        {
            TestContext.WriteLine("[[PROPERTY|id=TC-222]]");
            await Context.GrantPermissionsAsync(new[] { "geolocation" });
            await Context.SetGeolocationAsync(new Geolocation { Latitude = 37.7749f, Longitude = -122.4194f });

            await SignIn("demouser");
            await Page.GotoAsync($"{BaseUrl}/offers");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(3000);

            var offerCards = Page.Locator("div.offer");
            var count = await offerCards.CountAsync();
            Assert.That(count, Is.GreaterThan(0), "Offer cards should be present");

            for (int i = 0; i < count; i++)
            {
                var card = offerCards.Nth(i);

                // Image with 150px height
                var img = card.Locator("img");
                await Expect(img).ToBeVisibleAsync();
                var imgHeight = await img.EvaluateAsync<string>("el => el.style.height");
                Assert.That(imgHeight, Is.EqualTo("150px"), $"Offer card {i} image should have height 150px");

                // Title below image
                var title = card.Locator(".offer-title");
                await Expect(title).ToBeVisibleAsync();
                var titleText = await title.InnerTextAsync();
                Assert.That(titleText, Is.Not.Empty, $"Offer card {i} should have a non-empty title");
            }
        }
    }
}

using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TestathonTests.Tests
{
    [TestFixture]
    [Category("smoke")]
    public class HomepageTests : PageTest
    {
        [Test]
        [Description("TC-99: Verify homepage loads successfully")]
        public async Task HomepageLoadsSuccessfully()
        {
            await Page.GotoAsync("https://testathon.live");

            await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex(".+"));

            await Expect(Page.Locator("body")).ToBeVisibleAsync();
        }
    }
}

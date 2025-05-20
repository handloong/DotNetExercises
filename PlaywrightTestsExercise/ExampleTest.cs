using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTestsExercise
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ExampleTest
    {
        //dotnet tool install --global PowerShell

        //public override BrowserNewContextOptions ContextOptions()
        //{
        //    return new BrowserNewContextOptions
        //    {
                
        //    };
        //}
        //public override BrowserTypeLaunchOptions GetLaunchOptions()
        //{
        //    return new BrowserTypeLaunchOptions
        //    {
        //        Headless = false,  // 显示浏览器窗口
        //        SlowMo = 500,      // 放慢操作速度（毫秒），便于观察
        //    };
        //}

        [Test]
        public async Task BaiduSearch()
        {
            using var playwright = await Playwright.CreateAsync();
            var chromium = playwright.Chromium;
            // Make sure to run headed.
            var browser = await chromium.LaunchAsync(new() { Headless = false });

            // Setup context however you like.
            var context = await browser.NewContextAsync(); // Pass any options
            await context.RouteAsync("**/*", route => route.ContinueAsync());

            // Pause the page, and start recording manually.
            var page = await context.NewPageAsync();

            await page.GotoAsync("http://baidu.com");
            //await page.GotoAsync("http://10.61.131.106/login");


            await page.FillAsync("//*[@id=\"kw\"]", "你好");
            await page.ClickAsync("//*[@id=\"su\"]");

            
            //await page.FillAsync("//*[@id=\"form_item_account\"]", "superAdmin");

            //await page.FillAsync("//*[@id=\"form_item_password\"]", "P@ssw0rddzz");

            //await page.ClickAsync("//button[@class='ant-btn ant-btn-primary ant-btn-lg login-button w-full']");
            await page.PauseAsync();
        }
    }
}

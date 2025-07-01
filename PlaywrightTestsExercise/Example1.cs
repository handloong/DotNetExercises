using Microsoft.Playwright;
using System.Threading.Tasks;


public class Example1
{
    public async Task Execute()
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

        //await page.GotoAsync("http://baidu.com");
        await page.GotoAsync("http://10.61.131.106/login", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });


        //await page.FillAsync("//*[@id=\"kw\"]", "你好");
        //await page.ClickAsync("//*[@id=\"su\"]");


        await page.FillAsync("//*[@id=\"form_item_account\"]", "superAdmin",new PageFillOptions { 
        
        });

        await page.FillAsync("//*[@id=\"form_item_password\"]", "P@ssw0rddzz");

        await page.ClickAsync("//button[@class='ant-btn ant-btn-primary ant-btn-lg login-button w-full']");
        await page.PauseAsync();
    }
}


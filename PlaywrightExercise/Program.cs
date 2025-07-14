using Microsoft.Playwright;
using System.Reflection.Metadata;
using System.Xml.Linq;

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
});
var context = await browser.NewContextAsync();
var page = await context.NewPageAsync();
await Svg2PngUseJs(browser, page);
await Svg2Png(browser, page);



async Task Svg2PngUseJs(IBrowser browser, IPage page)
{
    var svg = File.ReadAllText(@"C:\Users\Administrator\Desktop\svg\500.svg");

    Log("设置svg数据");

    await page.SetContentAsync(svg);

    // 获取 SVG 的 outerHTML
    string svgContent = await page.EvalOnSelectorAsync<string>(
        "svg", "el => el.outerHTML"
    );

    var js = @"
        (svgText) => {{
            return new Promise((resolve) => {{
                const svgBlob = new Blob([svgText], {{type: 'image/svg+xml'}});
                const url = URL.createObjectURL(svgBlob);
                const img = new Image();
                img.onload = function () {{
                    const canvas = document.createElement('canvas');
                    canvas.width = img.width;
                    canvas.height = img.height;
                    const ctx = canvas.getContext('2d');
                    ctx.drawImage(img, 0, 0);
                    URL.revokeObjectURL(url);
                    resolve(canvas.toDataURL('image/png').split(',')[1]);  // 仅返回 base64 内容
                }};
                img.src = url;
            }});
        }}
        ";

    var data_url = await page.EvaluateAsync<string>(js, svgContent);

    Log("截图");
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = "10M-Svg-10002.Png",
        Type = ScreenshotType.Png,
        FullPage = true,
    });
    Log("截图完成");

    Console.WriteLine("保存完成");
}

async Task Svg2Png(IBrowser browser, IPage page)
{
    var svg = File.ReadAllText(@"C:\Users\Administrator\Desktop\svg\500.svg");

    Log("设置svg数据");

    await page.SetContentAsync(svg);
    Log("截图");
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = "10M-Svg-10002.Png",
        Type = ScreenshotType.Png,
        FullPage = true,
    });
    Log("截图完成");

    Console.WriteLine("保存完成");
}

void Log(string v)
{
    Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff}: {v}");
}

await page.GotoAsync("http://ksbi.luxshare.com.cn/#/login?redirect=/generate/topic");
await page.GetByRole(AriaRole.Textbox, new() { Name = "账号" }).ClickAsync();
await page.GetByRole(AriaRole.Textbox, new() { Name = "账号" }).FillAsync("admin");
await page.GetByRole(AriaRole.Textbox, new() { Name = "账号" }).PressAsync("Tab");
await page.GetByRole(AriaRole.Textbox, new() { Name = "密码" }).FillAsync("admin123");
await page.GetByRole(AriaRole.Button, new() { Name = "登录" }).ClickAsync();
await page.GetByText("分析中心").ClickAsync();

await page.GotoAsync("http://ksbi.luxshare.com.cn/#/topic-form?id=1944556624929619968&name=%E5%B7%A5%E4%BD%9C%E7%B0%BF");

await Task.Delay(2000); // 等待页面加载
await page.GetByRole(AriaRole.Button, new() { Name = "预览最新数据" }).ClickAsync();

var canvas = page.Locator("//*[@id=\"container-canvas-1944556929890566144\"]/canvas").First;

// 获取 canvas 的实际尺寸
var boundingBox = await canvas.BoundingBoxAsync();


// 获取元素的 outerHTML（包括标签本身）
var elementHtml = await page.EvaluateAsync<string>(
    "selector => document.querySelector(selector).outerHTML",
    canvas
);



var screenshot = await canvas.ScreenshotAsync(new LocatorScreenshotOptions
{
    Path = "screenshot.Jpeg",
    Type = ScreenshotType.Jpeg,
    Quality = 100
});
File.WriteAllBytes("screenshot.Jpeg", screenshot);

using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using SkiaSharp;


IPrinterStorage printerStorage = new PrinterStorage();
var drawer = new ZplElementDrawer(printerStorage);

var analyzer = new ZplAnalyzer(printerStorage);
var analyzeInfo = analyzer.Analyze("^XA^FT100,100^A0N,67,0^FDTestLabel^FS^XZ");


/*
 
   <ItemGroup>
    <PackageReference Include="BinaryKits.Zpl.Label" Version="3.2.1" />
    <PackageReference Include="BinaryKits.Zpl.Viewer" Version="1.2.1" />
  </ItemGroup>

  Linux下需要安装相关依赖:
 
 https://www.cnblogs.com/jopny/p/18608831/SkiaSharp-no-found-libSkiaSharp_so-error

 
 */


foreach (var labelInfo in analyzeInfo.LabelInfos)
{
    var pdf = drawer.DrawPdf(labelInfo.ZplElements);
    File.WriteAllBytes("label.pdf", pdf);

    var imageData = drawer.Draw(labelInfo.ZplElements);
    File.WriteAllBytes("label.png", imageData);
}

DrawerOptions BuildDrawOption(string fontName = "NotoSansSC-VF")
{
    var fontFile = Path.Combine(@"C:\Windows\Fonts\NotoSansSC-VF.ttf");
    var drawOptions = new DrawerOptions
    {
        FontLoader = fontName => { return SKTypeface.FromFile(fontFile); },
        OpaqueBackground = false,
        PdfOutput = false,
        RenderFormat = SKEncodedImageFormat.Png,
        RenderQuality = 100,
        Antialias = true,
        ReplaceDashWithEnDash = false,
    };
    return drawOptions;
}
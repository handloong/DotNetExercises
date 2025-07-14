// See https://aka.ms/new-console-template for more information
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Svg;

Console.WriteLine("Hello, World!");



var input = @"C:\Users\Administrator\Desktop\svg\1912311205119586304_工作簿.svg";
var output = input + ".png";

ProcessSvgUseSvgNet(input, output);

Console.ReadLine();


static ReadOnlySpan<byte> ConvertToReadOnlySpan(System.Drawing.Bitmap bitmap)
{
    // 将Bitmap保存到内存流中
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        ms.Position = 0;

        // 将内存流读取为字节数组
        byte[] byteArray = ms.ToArray();

        // 创建ReadOnlySpan<byte>
        return new ReadOnlySpan<byte>(byteArray);
    }
}

static void ProcessSvgUseSvgNet(string input, string output)
{
    Console.WriteLine(DateTime.Now);

    var svgDocument = SvgDocument.Open(input);

    var w = (int)svgDocument.Width.Value;
    var h = (int)svgDocument.Height.Value;
    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(w, h);
    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
        g.Clear(System.Drawing.Color.White);

    svgDocument.Draw(bitmap);

    var bytes = ConvertToReadOnlySpan(bitmap);

    using (Image<Rgba32> image = Image.Load<Rgba32>(bytes))
    {

        //TODO:背景颜色改成白色

        // 保存修改后的图像
        image.Save(output);
    }

    Console.WriteLine(DateTime.Now);
}
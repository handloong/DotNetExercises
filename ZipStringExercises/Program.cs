// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var json = File.ReadAllText(@"t.json");

File.WriteAllText("t_comp_gzip.json",GZipCompressString(json));



#region  压缩和解压字符串
/// <summary>
/// 将传入字符串以GZip算法压缩后，返回Base64编码字符
/// </summary>
/// <param name="rawString">需要压缩的字符串</param>
/// <returns>压缩后的Base64编码的字符串</returns>
static string GZipCompressString(string rawString)
{
    if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
    {
        return "";
    }
    else
    {
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
        byte[] zippedData = Compress(rawData);
        return (string)(Convert.ToBase64String(zippedData));
    }
}

/// <summary>
/// GZip压缩
/// </summary>
/// <param name="rawData"></param>
/// <returns></returns>
static byte[] Compress(byte[] rawData)
{
    System.IO.MemoryStream ms = new System.IO.MemoryStream();
    System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
    compressedzipStream.Write(rawData, 0, rawData.Length);
    compressedzipStream.Close();
    return ms.ToArray();
}

/// <summary>
/// 解压Sring 
/// </summary>
/// <param name="Value"></param>
/// <returns></returns>
static string GetStringByString(string Value)
{
    //DataSet ds = new DataSet();
    string CC = GZipDecompressString(Value);
    //System.IO.StringReader Sr = new System.IO.StringReader(CC);
    //ds.ReadXml(Sr);
    return CC;
}

/// <summary>
/// 将传入的二进制字符串资料以GZip算法解压缩
/// </summary>
/// <param name="zippedString">经GZip压缩后的二进制字符串</param>
/// <returns>原始未压缩字符串</returns>
static string GZipDecompressString(string zippedString)
{
    if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
    {
        return "";
    }
    else
    {
        byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
        return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
    }
}

/// <summary>
/// ZIP解压
/// </summary>
/// <param name="zippedData"></param>
/// <returns></returns>
static byte[] Decompress(byte[] zippedData)
{
    System.IO.MemoryStream ms = new System.IO.MemoryStream(zippedData);
    System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
    System.IO.MemoryStream outBuffer = new System.IO.MemoryStream();
    byte[] block = new byte[1024];
    while (true)
    {
        int bytesRead = compressedzipStream.Read(block, 0, block.Length);
        if (bytesRead <= 0)
            break;
        else
            outBuffer.Write(block, 0, bytesRead);
    }
    compressedzipStream.Close();
    return outBuffer.ToArray();

}
#endregion
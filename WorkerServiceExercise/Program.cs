using System.Net.Sockets;
using System.Net;

using (TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8001))
{
    try
    {
        // 启动监听
        listener.Start();
        Console.WriteLine("Socket Created and Listening");
        while (true)
        {
            // 等待客户端连接
            using (TcpClient client = listener.AcceptTcpClient())
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                // 设置超时时间
                client.ReceiveTimeout = 10000;
                while (true)
                {
                    // 接收数据
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Exit Server");
                        break;
                    }
                    string data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Get value {data}");
                    // 返回数据
                    byte[] responseData = System.Text.Encoding.UTF8.GetBytes("OK");
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
        }
    }
    catch (SocketException e)
    {
        Console.WriteLine($"Bind Failed... {e.Message}");
    }
    finally
    {
        // 停止监听
        listener.Stop();
    }
}
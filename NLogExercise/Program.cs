namespace NLogExercise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //按照文件夹来分
            MyLog.ConfigureNLog("HTTP", "MainThread");

            MyLog.Info("HTTP", "这是一个Trace级别的日志消息");
            MyLog.Info("MainThread", "MainThread~~~");
            MyLog.Info("NO_BUS", "消息会被忽略");
        }
    }
}

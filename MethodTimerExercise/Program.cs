using MethodTimer;
using NLogExercise;
using System.Reflection;

namespace MethodTimerExercise
{
    [Time]
    internal class Program
    {
        static void Main(string[] args)
        {
            MyLog.ConfigureNLog("TimeMethodMonitor");

            TimeMethod(200);
        }

        public static void TimeMethod(int total)
        {
            for (int i = 0; i < total; i++)
            {
                Console.WriteLine($"输出结果{i}");
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 运行耗时为long（毫秒）
        /// </summary>
        public static class MethodTimeLogger
        {
            public static void Log(MethodBase methodBase, long milliseconds, string message)
            {
                if (milliseconds > 200)
                {
                    MyLog.Trace("TimeMethodMonitor", $"方法：{methodBase.ReflectedType?.FullName}.{methodBase.Name} 耗时：{milliseconds} 毫秒，信息：{message}");
                }
                //Console.WriteLine($"方法：{methodBase.Name} 耗时：{milliseconds} 毫秒，信息：{message}");
            }
        }

        ///// <summary>
        ///// 运行耗时为TimeSpan
        ///// </summary>
        //public static class MethodTimeLogger
        //{
        //    public static void Log(MethodBase methodBase, TimeSpan elapsed, string message)
        //    {
        //        MyLog.Trace("TimeMethodMonitor", $"方法：{methodBase.ReflectedType?.FullName}.{methodBase.Name} 耗时：{elapsed.TotalMilliseconds} 毫秒，信息：{message}");
        //        //Console.WriteLine($"方法：{methodBase.Name} 耗时：{elapsed.TotalMilliseconds} 毫秒，信息：{message}");
        //    }
        //}
    }
}

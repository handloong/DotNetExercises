using MethodTimer;
using NLogExercise;

namespace MethodTimerExercise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyLog.ConfigureNLog("TimeMethodMonitor");

            TimeMethod(200);
        }

        [Time]
        public static void TimeMethod(int total)
        {
            for (int i = 0; i < total; i++)
            {
                Console.WriteLine($"输出结果{i}");
                Thread.Sleep(1);
            }
        }
    }
}

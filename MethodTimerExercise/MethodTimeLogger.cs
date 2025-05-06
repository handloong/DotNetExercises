using NLogExercise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
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
}

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace NLogExercise
{
    public static class MyLog
    {
        //static MyLog()
        //{
        //    ConfigureNLog();
        //}

        public static void ConfigureNLog(params string[] businessName)
        {
            var config = new LoggingConfiguration();

            foreach (var item in businessName)
            {
                AddBusinessTarget(config, item);
            }

            LogManager.Configuration = config;
        }

        private static void AddBusinessTarget(LoggingConfiguration config, string businessName)
        {
            // 创建按业务隔离的FileTarget
            var fileTarget = new FileTarget
            {
                Name = $"file_{businessName}",
                FileName = $"${{basedir}}/Logs/{businessName}/${{level}}/${{date:format=yyyy-MM-dd}}.log",
                Layout = "${longdate} | ${message}${onexception:${exception:format=ToString}",
                ArchiveAboveSize = 10485760, // 10MB
                MaxArchiveFiles = 7,
                ConcurrentWrites = true
            };

            // 异步包装
            var asyncTarget = new AsyncTargetWrapper(fileTarget)
            {
                Name = $"async_{businessName}",
                WrappedTarget = fileTarget
            };

            config.AddTarget(asyncTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, asyncTarget, $"MyLog.{businessName}");
        }

        public static void Info(string businessName, string message)
        {
            LogManager.GetLogger($"MyLog.{businessName}").Info(message);
        }

        public static void Error(string businessName, string message)
        {
            LogManager.GetLogger($"MyLog.{businessName}").Error(message);
        }

        public static void Error(string businessName, Exception ex, string message)
        {
            LogManager.GetLogger($"MyLog.{businessName}").Error(ex, message ?? ex.Message);
        }
    }
}

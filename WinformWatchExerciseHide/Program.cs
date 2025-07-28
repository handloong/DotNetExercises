using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinformWatchExerciseHide
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Process[] running = Process.GetProcessesByName("WinformWatchExercise");
                if (running.Length == 0)
                {
                    // 进程不存在，启动它
                    Process.Start("WinformWatchExercise.exe");
                }
                Thread.Sleep(100); // 每3秒检测一次
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WinformWatchExercise
{
    public partial class Form1 : Form
    {
        private Timer moveTimer;
        private int currentCorner = 0;
        private Point[] corners;
        public Form1()
        {
            InitializeComponent();
            InitCorners();
            InitTimer();
        }


        private void InitCorners()
        {
            // 获取主屏幕工作区域
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int w = this.Width;
            int h = this.Height;
            // 四个角的坐标
            corners = new Point[]
            {
            new Point(screen.Left, screen.Top), // 左上
            new Point(screen.Right - w, screen.Top), // 右上
            new Point(screen.Right - w, screen.Bottom - h), // 右下
            new Point(screen.Left, screen.Bottom - h), // 左下
            };
        }

        private void InitTimer()
        {
            moveTimer = new Timer();
            moveTimer.Interval = 1000; // 每1秒移动一次
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            this.Location = corners[currentCorner];
            currentCorner = (currentCorner + 1) % corners.Length;
        }

        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "新建文本文档.txt");
        void CheckWatchDog()
        {
            try
            {
                while (true)
                {
                    this.TopMost = true;
                    try
                    {
                        var watchdog = Process.GetProcessesByName("WinformWatchExerciseHide");
                        if (watchdog.Length == 0)
                        {
                            if (File.Exists(file))
                            {
                                System.IO.File.Delete(file);
                                Application.Exit();
                            }
                            else
                            {
                                Process.Start("WinformWatchExerciseHide.exe");
                            }
                        }
                        else
                        {
                            if (File.Exists(file))
                            {
                                foreach (var wd in watchdog)
                                {
                                    wd.Kill();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() => { CheckWatchDog(); });
        }
    }
}

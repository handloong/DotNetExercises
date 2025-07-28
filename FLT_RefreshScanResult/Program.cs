using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FLT_RefreshScanResult
{
    internal class Program
    {
        /// <summary>
        /// 刷新扫描结果
        /// </summary>
        /// <param name="resultPath">结果路径</param>
        /// <returns>操作结果代码</returns>
        [DllImport("VerifyData.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RefreshScanResult([In][MarshalAs(UnmanagedType.LPWStr)] string resultPath);

        //D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122


        static void Main(string[] args)
        {
            Console.WriteLine($"请输入Surface.flt文件所在目录:");

            var path = Console.ReadLine();

            try
            {
                var res = RefreshScanResult(path);
                Console.WriteLine($"刷新结果:{res}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}

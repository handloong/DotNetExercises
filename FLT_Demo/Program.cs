using PostProcessing.DefectsClustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLT_Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DefectsClusteringPostProcessor defectsClusteringPostProcessor = new DefectsClusteringPostProcessor();


            string re = "";
            defectsClusteringPostProcessor.RunPostProcessor("D:\\A祝斌分享\\Camtek\\12-QM11133-FI02-D-ALLBUMP\\Setup1\\LOT0421000002", out bool s, ref  re);

        }
    }
}

using ConsoleDump;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DieInfoFileExercise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var basePath = @"D:\A祝斌分享\";
            var waferInfoFullName = Path.Combine(basePath, "WaferInfo_672.dat");


            var dieInfos = Z.GetDieInfoFileEnumerator(waferInfoFullName).ToList();
            //var dieInfo67s = Z.GetDieInfoFileEnumerator(Path.Combine(basePath, "WaferInfo_67.dat")).ToList();
            //ReadWaferInfoDat("WaferInfo_671.dat", basePath);
            //WriteUseInterface(waferInfoFullName, basePath);
            WriteUseList(basePath,waferInfoFullName);

           
        }

        private static void WriteUseList(string basePath,string waferInfoFullName)
        {
            var dieInfos = Z.GetDieInfoFileEnumerator(waferInfoFullName).ToList();
            var writeDies = new List<SDieInfoType>();

            for (int i = 0; i < dieInfos.Count(); i++)
            {
                var die = dieInfos.ElementAt(i);
                if (die.Col == 67 && die.Row == 19)
                {
                    die.Status = 0;
                    die.Scan = 0;
                    die.Attributes = 0;
                }
                writeDies.Add(die);
            }
            var st = writeDies.Where(x => x.Col == 67 && x.Row == 19).FirstOrDefault();

            var waferInfo67 = Path.Combine(basePath, "WaferInfo_671.dat");
            Z.WriteDieInfoFileEnumerator(waferInfo67, writeDies);
        }

        private static void ReadWaferInfoDat(string waferInfo, string basePath)
        {
            var dieFile = new DieInfoFile(basePath, FileAccess.ReadWrite, FileOptions.SequentialScan, waferInfo);

            var total = dieFile.Header.Rows * dieFile.Header.Cols;

            for (int i = 0; i < total; i++)
            {
                var die = dieFile.ReadNext();

                Console.WriteLine($"[{i + 1}/{total}] {die.Col} x {die.Row}  {die.Bin}");
            }
        }

        private static void WriteUseInterface(string waferdat, string basePath)
        {
            var dieFile = new DieInfoFile(basePath, FileAccess.ReadWrite, FileOptions.SequentialScan, waferdat);

            var total = dieFile.Header.Rows * dieFile.Header.Cols;

            for (int i = 0; i < total; i++)
            {
                var die = dieFile.ReadNext();

                if (die.Col == 67 && die.Row == 19)
                {
                    die.Status = 0;
                    die.Attributes = 0;
                    dieFile.Write(die);
                }

                Console.WriteLine($"[{i + 1}/{total}] {die.Col} x {die.Row}  {die.Bin}");
            }
        }
    }
}

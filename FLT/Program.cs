using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FLT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //llt文件
            var file = @"D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122\Surface.flt";

            //读取
            var ssurfaceDatas = ReadSSurfaceData(file);

            //写入
            file = @"D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122\Surface-10.flt";
            //测试删除一个数据重新写入
            ssurfaceDatas.RemoveAt(0);
            CreateSurfaceFltFile(file, ssurfaceDatas);
        }


        private static List<SSurfaceRes> ReadSSurfaceData(string file)
        {
            var ssurfaceDatas = new List<SSurfaceRes>();
            using (BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                byte[] array = new byte[Marshal.SizeOf(typeof(SSurfaceRes))];
                GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    binaryReader.Read(array, 0, array.Count());
                    SSurfaceRes item = (SSurfaceRes)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(SSurfaceRes));
                    ssurfaceDatas.Add(item);
                }
            }

            return ssurfaceDatas;
        }

        static void CreateSurfaceFltFile(string sutfaceFilePath, List<SSurfaceRes> list)
        {
            FileStream fileStream = new FileStream(sutfaceFilePath, FileMode.Create);
            foreach (SSurfaceRes item9 in list)
            {
                byte[] bytesFromRes = GetBytesFromRes(item9);
                fileStream.Write(bytesFromRes, 0, bytesFromRes.Length);
            }
            fileStream.Close();
            fileStream.Dispose();
        }

        static byte[] GetBytesFromRes(SSurfaceRes res)
        {
            int num = Marshal.SizeOf(res);
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            byte[] array = new byte[num];
            Marshal.StructureToPtr(res, intPtr, fDeleteOld: true);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [Guid("FB5A955A-1249-4C47-AFB6-94F0B70D9101")]
    public struct SSurfaceRes
    {
        public short reclassify;

        public short orgReclassify;

        public int Type;

        public byte waferRegion;

        public byte zone;

        public byte recipe;

        public double x;

        public double y;

        public double ActualX;

        public double ActualY;

        public int id;

        public int resultID;

        public double BoxWidth;

        public double BoxHeight;

        public double area;

        public double BlobBreadth;

        public double BlobLength;

        public double BlobFeretMax;

        public double BlobFeretMaxAngle;

        public double BlobFeretMin;

        public int particlesNum;

        public int particlesArea;

        public double Contrast;
    }
}

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
            //CreateUserInfo(@"D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122\userInfo.dat");
            //var userInfos = ReadUserInfo(@"D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122\userInfo.dat");

            //llt文件
            var file = @"D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122\Surface.flt";

            //读取
            var ssurfaceDatas = ReadSSurfaceData(file);


            file = @"D:\A祝斌分享\Camtek\DieAlignment.dat";
            var ssurfaceDatas1 = ReadDieAlignmentData(file);

            file = @"D:\A祝斌分享\Camtek\WaferInfo.dat";
            var waferInfos = ReadWaferInfo(file);

            var waferInfo = ReadSSurfaceDataUseSRegModelRes(file);

            //写入
            file = @"D:\A祝斌分享\Camtek\12-QM11133-FI02-D-ALLBUMP\Setup1\LOT0421000002\14000243444-04-122\Surface-rm-top5.flt";
            //测试删除一个数据重新写入
            //ssurfaceDatas.RemoveAt(ssurfaceDatas.Count-1);
            ssurfaceDatas.RemoveAt(0);
            ssurfaceDatas.RemoveAt(1);
            ssurfaceDatas.RemoveAt(2);
            ssurfaceDatas.RemoveAt(3);
            ssurfaceDatas.RemoveAt(4);

            CreateSurfaceFltFile(file, ssurfaceDatas);
        }


        private static List<SRegModelRes> ReadSSurfaceDataUseSRegModelRes(string file)
        {
            var ssurfaceDatas = new List<SRegModelRes>();
            using (BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                byte[] array = new byte[Marshal.SizeOf(typeof(SRegModelRes))];
                GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    binaryReader.Read(array, 0, array.Count());
                    SRegModelRes item = (SRegModelRes)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(SRegModelRes));
                    ssurfaceDatas.Add(item);
                }
            }

            return ssurfaceDatas;
        }

        private static List<DieAlignmentData> ReadDieAlignmentData(string file)
        {
            var ssurfaceDatas = new List<DieAlignmentData>();
            using (BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                byte[] array = new byte[Marshal.SizeOf(typeof(DieAlignmentData))];
                GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    binaryReader.Read(array, 0, array.Count());
                    DieAlignmentData item = (DieAlignmentData)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(DieAlignmentData));
                    ssurfaceDatas.Add(item);
                }
            }

            return ssurfaceDatas;
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


        static void CreateUserInfo(string sutfaceFilePath)
        {
            FileStream fileStream = new FileStream(sutfaceFilePath, FileMode.Create);
            var userINfos = new List<UserInfo>
            {
                new UserInfo { name = 1, age = 14,hex=0x002 },
                new UserInfo { name= 2, age = 11,hex= 0x003 },
                new UserInfo { name= 3, age = 23 }
            };
            foreach (UserInfo item9 in userINfos)
            {
                byte[] bytesFromRes = GetBytesFromRes1(item9);
                fileStream.Write(bytesFromRes, 0, bytesFromRes.Length);
            }
            fileStream.Close();
            fileStream.Dispose();

            byte[] GetBytesFromRes1(UserInfo res)
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

        static List<UserInfo> ReadUserInfo(string sutfaceFilePath)
        {
            var ssurfaceDatas = new List<UserInfo>();
            using (BinaryReader binaryReader = new BinaryReader(File.Open(sutfaceFilePath, FileMode.Open)))
            {
                byte[] array = new byte[Marshal.SizeOf(typeof(UserInfo))];
                GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    binaryReader.Read(array, 0, array.Count());
                    UserInfo item = (UserInfo)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(UserInfo));
                    ssurfaceDatas.Add(item);
                }
            }

            return ssurfaceDatas;
        }

        static List<WaferInfo> ReadWaferInfo(string sutfaceFilePath)
        {
            var ssurfaceDatas = new List<WaferInfo>();
            using (BinaryReader binaryReader = new BinaryReader(File.Open(sutfaceFilePath, FileMode.Open)))
            {
                byte[] array = new byte[Marshal.SizeOf(typeof(WaferInfo))];
                GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    binaryReader.Read(array, 0, array.Count());
                    WaferInfo item = (WaferInfo)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(WaferInfo));
                    ssurfaceDatas.Add(item);
                }
            }
            return ssurfaceDatas;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [Guid("FB5A955A-1249-4C47-AFB6-94F0B70D9112")]
    public struct WaferInfo
    {
        public double name;
        public double age;
        public byte hex { get; set; }
        public short reclassify;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [Guid("FB5A955A-1249-4C47-AFB6-94F0B70D9111")]
    public struct UserInfo
    {
        public double name;
        public double age;
        public byte hex { get; set; }
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DieInfoFileExercise
{
    public class Z
    {

        public static IEnumerable<SDieInfoType> GetDieInfoFileEnumerator(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(fs);
            ReadStruct<SDieInfoHeader>(br, out var header);
            for (int r = 0; r < header.Row; r++)
            {
                for (int c = 0; c < header.Col; c++)
                {
                    ReadStruct<SDieInfoType>(br, out var dieInfoType);
                    yield return dieInfoType;
                    dieInfoType = default(SDieInfoType);
                }
            }
        }

        public static void WriteDieInfoFileEnumerator(string filePath, IEnumerable<SDieInfoType> sDieInfoTypes)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            SDieInfoHeader sDieInfoHeader = new SDieInfoHeader
            {
                Col = sDieInfoTypes.Max(x => x.Col) + 1,
                Row = sDieInfoTypes.Max(x => x.Row) + 1,
                Format = 2
            };

            var headerBytes = GetBytesFromRes(sDieInfoHeader);
            fileStream.Write(headerBytes, 0, headerBytes.Length);


            foreach (SDieInfoType item9 in sDieInfoTypes)
            {
                byte[] bytesFromRes = GetBytesFromRes(item9);
                fileStream.Write(bytesFromRes, 0, bytesFromRes.Length);
            }
            fileStream.Close();
            fileStream.Dispose();

        }

        static byte[] GetBytesFromRes<T>(T res) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] array = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(res, ptr, false);
                Marshal.Copy(ptr, array, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return array;
        }


        public static void ReadStruct<T>(BinaryReader br, out T t) where T : struct
        {
            byte[] value = br.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle gCHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
            try
            {
                t = (T)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                gCHandle.Free();
            }
        }
        public static void ReadStruct<T>(string fileFullName, out T t) where T : struct
        {
            FileStream fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(fs);

            byte[] value = br.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle gCHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
            try
            {
                t = (T)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                gCHandle.Free();
            }
        }

        public static IEnumerable<T> GetDieInfoFileEnumerator<T>(string filePath)
        {
            var ssurfaceDatas = new List<T>();
            using (BinaryReader binaryReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                byte[] array = new byte[Marshal.SizeOf(typeof(T))];
                GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    binaryReader.Read(array, 0, array.Count());
                    T item = (T)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(T));
                    ssurfaceDatas.Add(item);
                }
            }
            return ssurfaceDatas;
        }
    }
}

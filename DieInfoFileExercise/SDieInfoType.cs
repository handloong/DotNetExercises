using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DieInfoFileExercise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [Guid("7A965682-B5EC-4981-8A39-9019AD92B8AA")]
    public struct SDieInfoType
    {
        public short Col;

        public short Row;

        public short Index;

        public short NullDie;

        public int temp;

        public short Attributes;

        public short ExtendScanAttribute;

        public short AlignDie;

        public byte Scan;

        public int Color;

        public int OrigColor;

        public int Score;

        public short Status;

        public SBinDieInfo BinInfo;
    }

}

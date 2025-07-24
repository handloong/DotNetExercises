using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DieInfoFileExercise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [Guid("B19CFD23-4D57-42A6-B79C-1DA104EAB826")]
    public struct SBinDieInfo
    {
        public short Bin;

        public short Priority;

        public int Color;
    }

}

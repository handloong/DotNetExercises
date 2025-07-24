using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DieInfoFileExercise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [Guid("066B6C03-7B61-4BED-A28C-BD7090B1829B")]
    public struct SDieInfoHeader
    {
        public int Format;

        public int Col;

        public int Row;
    }

}

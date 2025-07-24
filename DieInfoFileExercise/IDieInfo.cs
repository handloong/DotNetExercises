
using System.Runtime.InteropServices;

public interface IDieInfo
{
    short Col { get; set; }

    short Row { get; set; }

    short Index { get; set; }

    short NullDie { get; set; }

    ushort Attributes { get; set; }

    ushort ExtAttributes { get; set; }

    short AlignDie { get; set; }

    byte Scan { get; set; }

    int Color { get; set; }

    int OrigColor { get; set; }

    int Score { get; set; }

    short Status { get; set; }

    int Bin { get; set; }

    int Priority { get; set; }

    void GetData(ref byte[] buffer);

}

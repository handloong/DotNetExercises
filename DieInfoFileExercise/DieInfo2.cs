using System;
using System.Runtime.InteropServices;


public class DieInfo2 : IDieInfo
{
    private const int PRIORITY_MASK = 16383;

    public static readonly int DataSize = Marshal.SizeOf(typeof(DieInfoData2));

    private DieInfoData2 _data;

    public short Col
    {
        get
        {
            return _data.Col;
        }
        set
        {
            _data.Col = value;
        }
    }

    public short Row
    {
        get
        {
            return _data.Row;
        }
        set
        {
            _data.Row = value;
        }
    }

    public short Index
    {
        get
        {
            return _data.Index;
        }
        set
        {
            _data.Index = value;
        }
    }

    public short NullDie
    {
        get
        {
            return _data.NullDie;
        }
        set
        {
            _data.NullDie = value;
        }
    }

    public ushort Attributes
    {
        get
        {
            return _data.Attributes;
        }
        set
        {
            _data.Attributes = value;
        }
    }

    public ushort ExtAttributes
    {
        get
        {
            return _data.ExtAttributes;
        }
        set
        {
            _data.ExtAttributes = value;
        }
    }

    public short AlignDie
    {
        get
        {
            return _data.AlignDie;
        }
        set
        {
            _data.AlignDie = value;
        }
    }

    public byte Scan
    {
        get
        {
            return _data.Scan;
        }
        set
        {
            _data.Scan = value;
        }
    }

    public int Color
    {
        get
        {
            return _data.Color;
        }
        set
        {
            _data.Color = value;
        }
    }

    public int OrigColor
    {
        get
        {
            return _data.OrigColor;
        }
        set
        {
            _data.OrigColor = value;
        }
    }

    public int Score
    {
        get
        {
            return _data.Score;
        }
        set
        {
            _data.Score = value;
        }
    }

    public short Status
    {
        get
        {
            return _data.Status;
        }
        set
        {
            _data.Status = value;
        }
    }

    public int Bin
    {
        get
        {
            return _data.BinInfo.Bin;
        }
        set
        {
            if (value > 32767 || value < -32768)
            {
                throw new InvalidOperationException($"The value {value} is out of valid range");
            }
            _data.BinInfo.Bin = (short)value;
        }
    }

    public int Priority
    {
        get
        {
            return _data.BinInfo.Priority & 0x3FFF;
        }
        set
        {
            if (value > 32767 || value < -32768)
            {
                throw new InvalidOperationException($"The value {value} is out of valid range");
            }
            _data.BinInfo.Priority = (short)value;
        }
    }

    public DieInfo2()
    {
    }

    internal unsafe DieInfo2(byte[] buffer, int from)
    {
        int num = sizeof(DieInfoData2);
        fixed (byte* source = &buffer[from])
        {
            fixed (DieInfoData2* data = &_data)
            {
                Buffer.MemoryCopy(source, data, num, num);
            }
        }
    }

    public unsafe void GetData(ref byte[] buffer)
    {
        fixed (byte* destination = &buffer[0])
        {
            fixed (DieInfoData2* data = &_data)
            {
                Buffer.MemoryCopy(data, destination, DataSize, DataSize);
            }
        }
    }
}

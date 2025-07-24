using System;
using System.Runtime.InteropServices;


internal class DieInfo1 : IDieInfo
{
	public static readonly int DataSize = Marshal.SizeOf(typeof(DieInfoData1));

	private DieInfoData1 _data;

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
			if (value > 255 || value < 0)
			{
				throw new InvalidOperationException($"The value {value} is out of valid range");
			}
			_data.BinInfo.Bin = (byte)value;
		}
	}

	public int Priority
	{
		get
		{
			return _data.BinInfo.Priority;
		}
		set
		{
			if (value > 255 || value < 0)
			{
				throw new InvalidOperationException($"The value {value} is out of valid range");
			}
			_data.BinInfo.Priority = (byte)value;
		}
	}

	public DieInfo1()
	{
	}

	internal unsafe DieInfo1(byte[] buffer, int from)
	{
		fixed (byte* source = &buffer[from])
		{
			fixed (DieInfoData1* data = &_data)
			{
				Buffer.MemoryCopy(source, data, DataSize, DataSize);
			}
		}
	}

	public unsafe void GetData(ref byte[] buffer)
	{
		fixed (byte* destination = &buffer[0])
		{
			fixed (DieInfoData1* data = &_data)
			{
				Buffer.MemoryCopy(data, destination, DataSize, DataSize);
			}
		}
	}
}

using System;
using System.IO;
using System.Runtime.InteropServices;


public class DieInfoFile : FileStream
{
	public const string WaferInfoFileName = "WaferInfo.dat";

	public const string WaferLayoutFileName = "WaferLayout.dat";

	public static readonly int HeaderSize = Marshal.SizeOf(typeof(DieInfoHeader));

	private readonly int _recordSize;

	private byte[] _recordTempBytes;

	private readonly Func<byte[], int, IDieInfo> _createDieInfo;

	protected DieInfoHeader _header;

	public DieInfoHeader Header => _header;

	internal long RecordSize => _recordSize;

	public DieInfoFile(string path, FileAccess access, FileOptions fileOptions, string fileName)
		: base(Path.Combine(path, fileName), FileMode.Open, access | FileAccess.Read, FileShare.Read, 4096, fileOptions)
	{
		_header = ReadHeader();
		if (_header.Format != 1 && _header.Format != 2)
		{
			throw new Exception("Bad header format!!!");
		}
		_recordSize = ((_header.Format == 2) ? DieInfo2.DataSize : DieInfo1.DataSize);
		_createDieInfo = GenerateCreateDieInfo(_header.Format);
		_recordTempBytes = new byte[_recordSize];
	}

	public IDieInfo ReadNext()
	{
		Read(_recordTempBytes, 0, _recordTempBytes.Length);
		return _createDieInfo(_recordTempBytes, 0);
	}

	public void Write(IDieInfo info)
	{
		info.GetData(ref _recordTempBytes);
		Write(_recordTempBytes, 0, _recordTempBytes.Length);
	}

    private static Func<byte[], int, IDieInfo> GenerateCreateDieInfo(int format)
	{
		if (format == 2)
		{
			return (byte[] byteArray, int offset) => new DieInfo2(byteArray, offset);
		}
		return (byte[] byteArray, int offset) => new DieInfo1(byteArray, offset);
	}

	private unsafe DieInfoHeader ReadHeader()
	{
		byte[] array = new byte[HeaderSize];
		Read(array, 0, array.Length);
		fixed (byte* source = &array[0])
		{
			fixed (DieInfoHeader* header = &_header)
			{
				Buffer.MemoryCopy(source, header, array.Length, array.Length);
			}
		}
		return _header;
	}
}

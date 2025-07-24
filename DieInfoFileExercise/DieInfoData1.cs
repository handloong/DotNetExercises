
internal struct DieInfoData1
{
	public struct BinDieInfoData
	{
		public byte Bin;

		public byte Priority;

		public int Color;
	}

	public short Col;

	public short Row;

	public short Index;

	public short NullDie;

	public int Reserved;

	public ushort Attributes;

	public ushort ExtAttributes;

	public short AlignDie;

	public byte Scan;

	public int Color;

	public int OrigColor;

	public int Score;

	public short Status;

	public BinDieInfoData BinInfo;
}

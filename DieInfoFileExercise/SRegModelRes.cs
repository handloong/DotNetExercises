using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential, Pack = 8)]
[Guid("A374A2AD-0066-4364-A1B8-073107F43015")]
public struct SRegModelRes
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

	public double PosInRefFrameX;

	public double PosInRefFrameY;

	public float OffsetX;

	public float OffsetY;

	public float InsPosX;

	public float InsPosY;

	public float DistX;

	public float DistY;

	public int DieIndexCol;

	public int DieIndexRow;

	public float RefPosInBlockX;

	public float RefPosInBlockY;

	public float Score;

	public float TargetScore;

	public double FramePosX;

	public double FramePosY;

	public double FrameStageZ;

	public float VelX;

	public float VelY;

	public float AccX;

	public float AccY;

	public float JerkX;

	public float JerkY;
}

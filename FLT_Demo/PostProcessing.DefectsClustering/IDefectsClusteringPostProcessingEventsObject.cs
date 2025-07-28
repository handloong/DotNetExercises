using System.Runtime.InteropServices;

namespace PostProcessing.DefectsClustering
{
	[ComVisible(true)]
	[Guid("E6485228-9D40-4A97-BC11-1499FD2CEC13")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDefectsClusteringPostProcessingEventsObject
	{
		void DefectsClusteringPostProcessingProgress(double progress, ref int abortClustering);
	}
}

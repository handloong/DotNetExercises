using System.Runtime.InteropServices;

namespace PostProcessing.DefectsClustering
{
	[ComVisible(true)]
	[Guid("6D253840-55F1-49CC-81AD-CF3045E090C0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDefectsClusteringPostProcessingObject
	{
		void RunProcessing(string waferScanResultPath, out bool success, ref string error);

		void RegisterEvents(IDefectsClusteringPostProcessingEventsObject eventsObject);

		void UnRegisterEvents(IDefectsClusteringPostProcessingEventsObject eventsObject);
	}
}

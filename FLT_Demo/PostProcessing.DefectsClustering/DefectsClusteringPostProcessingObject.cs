using System.Runtime.InteropServices;

namespace PostProcessing.DefectsClustering
{
	[ComVisible(true)]
	[Guid("D3DD1C69-DA30-459F-8A84-70442AFF219C")]
	[ClassInterface(ClassInterfaceType.None)]
	[ProgId("PostProcessing.DefectsClusteringPostProcessingObject")]
	public class DefectsClusteringPostProcessingObject : IDefectsClusteringPostProcessingObject
	{
		private DefectsClusteringPostProcessor _defectsPostProcessor;

		public DefectsClusteringPostProcessingObject()
		{
			_defectsPostProcessor = new DefectsClusteringPostProcessor();
		}

		public void RunProcessing(string waferScanResultPath, out bool success, ref string error)
		{
			_defectsPostProcessor.RunPostProcessor(waferScanResultPath, out success, ref error);
		}

		public void RegisterEvents(IDefectsClusteringPostProcessingEventsObject eventsObject)
		{
			_defectsPostProcessor.RegisterEvents(eventsObject);
		}

		public void UnRegisterEvents(IDefectsClusteringPostProcessingEventsObject eventsObject)
		{
			_defectsPostProcessor.UnRegisterEvents(eventsObject);
		}
	}
}

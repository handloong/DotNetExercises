using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SystemTypes;
using Camtek.Clustering.IniFileReaderWrapper;
using Camtek.CoordinateSystems;
using Camtek.WaferInfo;
using DataAccess.DataLayer;
using Job;
using log4net;
using ProgressBar;

namespace PostProcessing.DefectsClustering
{
	/// <summary>
	/// 缺陷聚类后处理器类，用于对晶圆扫描结果中的缺陷进行聚类分析和处理
	/// </summary>
	public class DefectsClusteringPostProcessor
	{
		/// <summary>
		/// 聚类INI参数类，用于存储从配置文件中读取的聚类参数
		/// </summary>
		private class ClusteringIniParams
		{
			/// <summary>
			/// 获取或设置缺陷之间的距离阈值
			/// </summary>
			public Shift<WaferCS> Distance { get; set; }

			/// <summary>
			/// 获取或设置第一排序参数
			/// </summary>
			public string FirstSortParameter { get; set; }

			/// <summary>
			/// 获取或设置第二排序参数
			/// </summary>
			public string SecondSortParameter { get; set; }

			/// <summary>
			/// 获取或设置第一排序是否为降序
			/// </summary>
			public bool DescendingFirstSortingListOrder { get; set; }

			/// <summary>
			/// 获取或设置第二排序是否为降序
			/// </summary>
			public bool DescendingSecondSortingListOrder { get; set; }

			/// <summary>
			/// 获取或设置缺陷聚类算法模式
			/// </summary>
			public DefectsClusteringAlgMode DefectsClusteringAlgMode { get; set; }
		}

		/// <summary>
		/// 缺陷优先级比较器，用于比较两个缺陷的优先级
		/// </summary>
		internal class DefectsPriorityComparer : IComparer<SSurfaceRes>
		{
			/// <summary>
			/// 分类信息列表
			/// </summary>
			private IBinInfoList binInfoList;

			/// <summary>
			/// 构造函数，初始化分类信息列表
			/// </summary>
			/// <param name="binInfo">分类信息列表</param>
			public DefectsPriorityComparer(IBinInfoList binInfo)
			{
				binInfoList = binInfo;
			}

			/// <summary>
			/// 比较两个缺陷的优先级
			/// </summary>
			/// <param name="resA">第一个缺陷</param>
			/// <param name="resB">第二个缺陷</param>
			/// <returns>比较结果</returns>
			public int Compare(SSurfaceRes resA, SSurfaceRes resB)
			{
				int classPriority = binInfoList.GetClassPriority(resA.reclassify);
				int classPriority2 = binInfoList.GetClassPriority(resB.reclassify);
				return classPriority.CompareTo(classPriority2);
			}
		}

		/// <summary>
		/// 表面结果引用类，用于包装SSurfaceRes对象
		/// </summary>
		private class CSurfaceResRef
		{
			[CompilerGenerated]
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly SSurfaceRes _003CValue_003Ek__BackingField;

			/// <summary>
			/// 获取包装的SSurfaceRes对象
			/// </summary>
			public SSurfaceRes Value
			{
				[CompilerGenerated]
				get
				{
					return _003CValue_003Ek__BackingField;
				}
			}

			/// <summary>
			/// 构造函数，初始化包装的SSurfaceRes对象
			/// </summary>
			/// <param name="value">要包装的SSurfaceRes对象</param>
			public CSurfaceResRef(SSurfaceRes value)
			{
				_003CValue_003Ek__BackingField = value;
			}
		}

		/// <summary>
		/// 分类信息列表
		/// </summary>
		private IBinInfoList _binInfoList;

		/// <summary>
		/// 设置信息
		/// </summary>
		private ISetupInfo _setupInfo;

		/// <summary>
		/// 晶圆几何信息
		/// </summary>
		private IWaferGeometry _waferGeometry;

		/// <summary>
		/// 表面结果列表
		/// </summary>
		private List<SSurfaceRes> _surfaceResList;

		/// <summary>
		/// 事件对象列表
		/// </summary>
		private List<IDefectsClusteringPostProcessingEventsObject> _eventsObjects;

		/// <summary>
		/// 日志记录器
		/// </summary>
		private static readonly ILog _logger = LogManager.GetLogger(typeof(DefectsClusteringPostProcessor));

		/// <summary>
		/// 构造函数，初始化表面结果列表和事件对象列表
		/// </summary>
		public DefectsClusteringPostProcessor()
		{
			_surfaceResList = new List<SSurfaceRes>();
			_eventsObjects = new List<IDefectsClusteringPostProcessingEventsObject>();
		}

		/// <summary>
		/// 运行后处理器，对晶圆扫描结果进行缺陷聚类处理
		/// </summary>
		/// <param name="waferScanResultPath">晶圆扫描结果路径</param>
		/// <param name="success">处理是否成功</param>
		/// <param name="error">错误信息</param>
		public void RunPostProcessor(string waferScanResultPath, out bool success, ref string error)
		{
			try
			{
				success = false;
				string text = Path.Combine(waferScanResultPath, "Surface.flt");
				if (!File.Exists(text))
				{
					return;
				}
				// 备份原始文件
				File.Copy(text, Path.Combine(waferScanResultPath, "Surface_ORIG.flt"), true);
				// 加载晶圆几何信息
				LoadWaferGeometry(waferScanResultPath);
				_binInfoList = LoadBinInfoList(_setupInfo);
				// 从INI文件读取聚类参数
				DefectsClusteringParams clusteringParams = ReadParamsFromIniFile(waferScanResultPath);
				using (BinaryReader binaryReader = new BinaryReader(File.Open(text, FileMode.Open)))
				{
					// 从文件加载表面结果
					LoadSurfaceResFromFile(binaryReader);
					// 减少表面结果缺陷
					success = ReduceSurfaceResDefects(clusteringParams);
					if (!success)
					{
						string msg = "Defect clustering process was stopped, saved only partial results";
						global::ProgressBar.ProgressBar progressBar = new global::ProgressBar.ProgressBar();
						progressBar.Alert(msg, global::ProgressBar.ProgressBar.eAlert.MsgBoxWithOk, "Warining");
					}
				}
				// 将处理后的结果写回文件
				using (FileStream fileStream = new FileStream(text, FileMode.Create))
				{
					foreach (SSurfaceRes surfaceRes in _surfaceResList)
					{
						byte[] bytesFromRes = GetBytesFromRes(surfaceRes);
						fileStream.Write(bytesFromRes, 0, bytesFromRes.Length);
					}
				}
				try
				{
					// 刷新扫描结果
					RefreshScanResult(waferScanResultPath);
				}
				catch (Exception ex)
				{
					error = ex.Message;
				}
				success = true;
			}
			catch (Exception ex2)
			{
				error = ex2.Message;
				success = false;
			}
		}

		/// <summary>
		/// 注册事件对象
		/// </summary>
		/// <param name="eventsObject">要注册的事件对象</param>
		public void RegisterEvents(IDefectsClusteringPostProcessingEventsObject eventsObject)
		{
			_eventsObjects.Add(eventsObject);
		}

		/// <summary>
		/// 注销事件对象
		/// </summary>
		/// <param name="eventsObject">要注销的事件对象</param>
		public void UnRegisterEvents(IDefectsClusteringPostProcessingEventsObject eventsObject)
		{
			_eventsObjects.Remove(eventsObject);
		}

		/// <summary>
		/// 从INI文件读取聚类参数
		/// </summary>
		/// <param name="scanResultPath">扫描结果路径</param>
		/// <returns>缺陷聚类参数</returns>
		private static DefectsClusteringParams ReadParamsFromIniFile(string scanResultPath)
		{
			ISetupInfo setupInfo = SetupInfoLoader.LoadScanResultSt(scanResultPath);
			string fileName = DefectsClusteringParams.GetFileName(Path.Combine("C:\\job", setupInfo.JobName, setupInfo.SetupName));
			IniFileReaderWrapperHelper fileReader = new IniFileReaderWrapperHelper(fileName);
			DefectsClusteringParams result = new DefectsClusteringParams(fileReader);
			File.Copy(fileName, Path.Combine(scanResultPath, "DefectsClustering.ini"), true);
			return result;
		}

		/// <summary>
		/// 从文件加载表面结果
		/// </summary>
		/// <param name="binaryReader">二进制读取器</param>
		private void LoadSurfaceResFromFile(BinaryReader binaryReader)
		{
			byte[] array = new byte[Marshal.SizeOf(typeof(SSurfaceRes))];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
			{
				binaryReader.Read(array, 0, array.Count());
				SSurfaceRes item = (SSurfaceRes)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(SSurfaceRes));
				_surfaceResList.Add(item);
			}
			gCHandle.Free();
		}

		/// <summary>
		/// 减少表面结果缺陷，执行聚类处理
		/// </summary>
		/// <param name="clusteringParams">聚类参数</param>
		/// <returns>处理是否成功</returns>
		private bool ReduceSurfaceResDefects(DefectsClusteringParams clusteringParams)
		{
			_logger.Info("Clustering started");
			// 创建并填充表面结果引用的并发包
			ConcurrentBag<CSurfaceResRef> cSurfaceResRefList = new ConcurrentBag<CSurfaceResRef>();
			_surfaceResList.ForEach(delegate(SSurfaceRes d)
			{
				cSurfaceResRefList.Add(new CSurfaceResRef(d));
			});
			// 按芯片索引组织缺陷
			ConcurrentDictionary<SDieIndex, ConcurrentBag<CSurfaceResRef>> diceDict = new ConcurrentDictionary<SDieIndex, ConcurrentBag<CSurfaceResRef>>();
			foreach (CSurfaceResRef defectRef in cSurfaceResRefList)
			{
				SDieIndex defectIndex = GetDefectIndex(defectRef.Value);
				diceDict.AddOrUpdate(defectIndex, (SDieIndex key) => new ConcurrentBag<CSurfaceResRef> { defectRef }, delegate(SDieIndex key, ConcurrentBag<CSurfaceResRef> bag)
				{
					bag.Add(defectRef);
					return bag;
				});
			}
			int abortClustering = 0;
			int diesFinished = 0;
			ConcurrentBag<CSurfaceResRef> delegates = new ConcurrentBag<CSurfaceResRef>();
			int processorCount = Environment.ProcessorCount;
			List<Task> list = new List<Task>();
			Semaphore sem = new Semaphore(processorCount, processorCount);
			// 并行处理每个芯片上的缺陷
			foreach (KeyValuePair<SDieIndex, ConcurrentBag<CSurfaceResRef>> dieEntry in diceDict)
			{
				if (abortClustering != 0)
				{
					break;
				}
				sem.WaitOne();
				list.Add(Task.Run(delegate
				{
					try
					{
						_logger.DebugFormat("Start defectsDict creation for die index ({}, {}).", dieEntry.Key.Col, dieEntry.Key.Row);
						// 创建缺陷字典，记录每个缺陷与其他缺陷的关系
						ConcurrentDictionary<CSurfaceResRef, List<CSurfaceResRef>> concurrentDictionary = new ConcurrentDictionary<CSurfaceResRef, List<CSurfaceResRef>>(dieEntry.Value.Count, dieEntry.Value.Count);
						foreach (CSurfaceResRef item in dieEntry.Value)
						{
							concurrentDictionary.TryAdd(item, new List<CSurfaceResRef>());
							foreach (CSurfaceResRef item2 in dieEntry.Value)
							{
								// 检查两个缺陷是否在指定距离内
								if (!item2.Equals(item) && WithinDistance(item2.Value.ActualX, item2.Value.ActualY, item.Value.ActualX, item.Value.ActualY, clusteringParams.Distance))
								{
									List<CSurfaceResRef> value;
									if (!concurrentDictionary.TryGetValue(item, out value))
									{
										string message = string.Format("Value for key '{0}' is not contained in defectsDict", item.Value);
										_logger.Error(message);
										throw new KeyNotFoundException(message);
									}
									value.Add(item2);
								}
							}
						}
						_logger.DebugFormat("Finish defectsDict creation for die index ({}, {}).", dieEntry.Key.Col, dieEntry.Key.Row);
						// 根据聚类算法模式执行不同的聚类处理
						switch (clusteringParams.DefectsClusteringAlgMode)
						{
						case DefectsClusteringAlgMode.Grouping:
							DoStandardGrouping(clusteringParams, concurrentDictionary, delegates);
							break;
						case DefectsClusteringAlgMode.ContiniousGrouping:
							DoContinuousGrouping(clusteringParams, concurrentDictionary, delegates);
							break;
						default:
						{
							string message2 = string.Format("Unknown clustering mode: {0}", clusteringParams.DefectsClusteringAlgMode);
							_logger.Error(message2);
							throw new NotSupportedException(message2);
						}
						}
						Interlocked.Increment(ref diesFinished);
						// 更新进度条并检查是否需要中止聚类
						UpdateClusteringProgressBar(ref abortClustering, diesFinished, diceDict.Count);
					}
					catch (Exception message3)
					{
						_logger.Error(message3);
						throw;
					}
					finally
					{
						sem.Release();
					}
				}));
			}
			Task.WaitAll(list.ToArray());
			// 清空原始表面结果列表，并添加处理后的结果
			_surfaceResList.Clear();
			delegates.ToList().ForEach(delegate(CSurfaceResRef d)
			{
				_surfaceResList.Add(d.Value);
			});
			_logger.Info("Clustering finished");
			return abortClustering == 0;
		}

		/// <summary>
		/// 执行标准分组聚类
		/// </summary>
		/// <param name="clusteringParams">聚类参数</param>
		/// <param name="defectsDict">缺陷字典</param>
		/// <param name="delegates">代表缺陷集合</param>
		private void DoStandardGrouping(DefectsClusteringParams clusteringParams, 
			ConcurrentDictionary<CSurfaceResRef,
				List<CSurfaceResRef>> defectsDict,
			ConcurrentBag<CSurfaceResRef> delegates)
		{
			FieldInfo secondSortInfo = typeof(SSurfaceRes).GetField(clusteringParams.SecondSortParameter, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			DefectsPriorityComparer comparer = new DefectsPriorityComparer(_binInfoList);
			while (!defectsDict.IsEmpty)
			{
				KeyValuePair<CSurfaceResRef, List<CSurfaceResRef>> keyValuePair = defectsDict.First();
				HashSet<CSurfaceResRef> defectsToRemove = new HashSet<CSurfaceResRef>();
				List<CSurfaceResRef> value;
				foreach (CSurfaceResRef item in keyValuePair.Value)
				{
					if (defectsDict.ContainsKey(item))
					{
						defectsDict.TryRemove(item, out value);
					}
					else
					{
						defectsToRemove.Add(item);
					}
				}
				keyValuePair.Value.RemoveAll((CSurfaceResRef defectRef) => defectsToRemove.Contains(defectRef));
				keyValuePair.Value.Add(keyValuePair.Key);
				// 根据排序参数对缺陷进行排序
				List<CSurfaceResRef> source = keyValuePair.Value.OrderByWithDirection((CSurfaceResRef o) => o.Value, comparer, clusteringParams.DescendingFirstSortingListOrder).ThenByWithDirection((CSurfaceResRef o) => secondSortInfo.GetValue(o.Value), clusteringParams.DescendingSecondSortingListOrder).ToList();
				// 添加第一个缺陷作为代表
				delegates.Add(source.First());
				defectsDict.TryRemove(keyValuePair.Key, out value);
			}
		}

		/// <summary>
		/// 执行连续分组聚类
		/// </summary>
		/// <param name="clusteringParams">聚类参数</param>
		/// <param name="defectsDict">缺陷字典</param>
		/// <param name="delegates">代表缺陷集合</param>
		private void DoContinuousGrouping(DefectsClusteringParams clusteringParams, ConcurrentDictionary<CSurfaceResRef, List<CSurfaceResRef>> defectsDict, ConcurrentBag<CSurfaceResRef> delegates)
		{
			FieldInfo secondSortInfo = typeof(SSurfaceRes).GetField(clusteringParams.SecondSortParameter, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			DefectsPriorityComparer comparer = new DefectsPriorityComparer(_binInfoList);
			while (!defectsDict.IsEmpty)
			{
				HashSet<CSurfaceResRef> visited = new HashSet<CSurfaceResRef>();
				// 使用深度优先搜索找到所有相关的缺陷
				AddToVisitedByDFS(ref defectsDict, defectsDict.First().Key, ref visited);
				// 根据排序参数对缺陷进行排序
				List<CSurfaceResRef> list = visited.OrderByWithDirection((CSurfaceResRef o) => o.Value, comparer, clusteringParams.DescendingFirstSortingListOrder).ThenByWithDirection((CSurfaceResRef o) => secondSortInfo.GetValue(o.Value), clusteringParams.DescendingSecondSortingListOrder).ToList();
				// 添加第一个缺陷作为代表
				delegates.Add(list.First());
				foreach (CSurfaceResRef item in list)
				{
					List<CSurfaceResRef> value;
					defectsDict.TryRemove(item, out value);
				}
			}
		}

		/// <summary>
		/// 使用深度优先搜索将相关缺陷添加到已访问集合中
		/// </summary>
		/// <param name="defectsDict">缺陷字典</param>
		/// <param name="defectsDictEntryKey">当前缺陷键</param>
		/// <param name="visited">已访问缺陷集合</param>
		private void AddToVisitedByDFS([In] ref ConcurrentDictionary<CSurfaceResRef, List<CSurfaceResRef>> defectsDict, CSurfaceResRef defectsDictEntryKey, ref HashSet<CSurfaceResRef> visited)
		{
			if (visited.Contains(defectsDictEntryKey))
			{
				return;
			}
			visited.Add(defectsDictEntryKey);
			List<CSurfaceResRef> value;
			if (!defectsDict.TryGetValue(defectsDictEntryKey, out value))
			{
				string message = string.Format("Value for key '{0}' is not contained in defectsDict", defectsDictEntryKey.Value);
				_logger.Error(message);
				throw new KeyNotFoundException(message);
			}
			foreach (CSurfaceResRef item in value)
			{
				if (defectsDict.ContainsKey(item))
				{
					AddToVisitedByDFS(ref defectsDict, item, ref visited);
				}
			}
		}

		/// <summary>
		/// 检查两点之间的距离是否在指定阈值内
		/// </summary>
		/// <param name="x1">第一点X坐标</param>
		/// <param name="y1">第一点Y坐标</param>
		/// <param name="x2">第二点X坐标</param>
		/// <param name="y2">第二点Y坐标</param>
		/// <param name="dist">距离阈值</param>
		/// <returns>如果距离在阈值内则返回true，否则返回false</returns>
		private static bool WithinDistance(double x1, double y1, double x2, double y2, double dist)
		{
			return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) <= dist * dist;
		}

		/// <summary>
		/// 更新聚类进度条并检查是否需要中止聚类
		/// </summary>
		/// <param name="abortClustering">中止聚类标志</param>
		/// <param name="diesFinished">已完成的芯片数</param>
		/// <param name="totalDies">总芯片数</param>
		private void UpdateClusteringProgressBar(ref int abortClustering, int diesFinished, int totalDies)
		{
			double progress = (double)diesFinished / (double)totalDies * 100.0;
			foreach (IDefectsClusteringPostProcessingEventsObject eventsObject in _eventsObjects)
			{
				eventsObject.DefectsClusteringPostProcessingProgress(progress, ref abortClustering);
				if (abortClustering != 0)
				{
					break;
				}
			}
		}

		/// <summary>
		/// 获取缺陷所在的芯片索引
		/// </summary>
		/// <param name="defect">缺陷</param>
		/// <returns>芯片索引</returns>
		private SDieIndex GetDefectIndex(SSurfaceRes defect)
		{
			SDieIndex result = default(SDieIndex);
			bool FoundInDie;
			_waferGeometry.WaferToDieIndex(defect.ActualX, defect.ActualY, out result.Col, out result.Row, out FoundInDie);
			if (!FoundInDie)
			{
				throw new Exception(string.Format("Die Wasnt Found in location [{0}:{1}]", defect.ActualX, defect.ActualY));
			}
			return result;
		}

		/// <summary>
		/// 将表面结果转换为字节数组
		/// </summary>
		/// <param name="res">表面结果</param>
		/// <returns>字节数组</returns>
		private byte[] GetBytesFromRes(SSurfaceRes res)
		{
			int num = Marshal.SizeOf(res);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			byte[] array = new byte[num];
			Marshal.StructureToPtr(res, intPtr, true);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return array;
		}

		/// <summary>
		/// 检查两个缺陷是否在同一芯片上
		/// </summary>
		/// <param name="firstDefect">第一个缺陷</param>
		/// <param name="secondDieIndex">第二个芯片索引</param>
		/// <returns>如果在同一芯片上则返回true，否则返回false</returns>
		private bool DefectsInSameDie(SSurfaceRes firstDefect, SDieIndex secondDieIndex)
		{
			SDieIndex defectIndex = GetDefectIndex(firstDefect);
			return defectIndex.Col == secondDieIndex.Col && defectIndex.Row == secondDieIndex.Row;
		}

		/// <summary>
		/// 加载晶圆几何信息
		/// </summary>
		/// <param name="waferScanResultPath">晶圆扫描结果路径</param>
		private void LoadWaferGeometry(string waferScanResultPath)
		{
            _waferGeometry = new CWaferGeometryClass();


            SetupInfoLoader setupInfoLoader = new SetupInfoLoader();
			_setupInfo = setupInfoLoader.LoadScanResult(waferScanResultPath);
			_waferGeometry.Init(_setupInfo);
			_waferGeometry.Load();
		}

		/// <summary>
		/// 加载分类信息列表
		/// </summary>
		/// <param name="setupInfo">设置信息</param>
		/// <returns>分类信息列表</returns>
		private IBinInfoList LoadBinInfoList(ISetupInfo setupInfo)
		{
			return BinInfoListFactory.LoadStatic(setupInfo);
		}

		/// <summary>
		/// 获取缺陷坐标
		/// </summary>
		/// <param name="defect">缺陷</param>
		/// <returns>缺陷坐标</returns>
		private Coord<WaferCS> GetDefectCoord(SSurfaceRes defect)
		{
			return new Coord<WaferCS>(defect.ActualX, defect.ActualY);
		}

		/// <summary>
		/// 刷新扫描结果
		/// </summary>
		/// <param name="resultPath">结果路径</param>
		/// <returns>操作结果代码</returns>
		[DllImport("VerifyData.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int RefreshScanResult([In][MarshalAs(UnmanagedType.LPWStr)] string resultPath);
	}
}

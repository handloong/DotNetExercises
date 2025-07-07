using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

public class ReadFltFileService : IReadFileService
{
	public static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private const double valueForGFormat = 1000000.0;

	public string[] GetData(string sourcePath)
	{
		Logger.InfoFormat("GetData is started : file {0}", sourcePath);
		if (!File.Exists(sourcePath))
		{
			Logger.InfoFormat("File Not Exist {0}", sourcePath);
			throw new FileNotFoundException("File Not Exist", sourcePath);
		}
		string[] result = ConvertData(sourcePath);
		Logger.Info("GetData is ended : file");
		return result;
	}

	public string[] GetDataInChunck(string sourcePath, int start, int end)
	{
		Logger.InfoFormat("GetData is started : file {0}", sourcePath);
		if (!File.Exists(sourcePath))
		{
			Logger.InfoFormat("File Not Exist {0}", sourcePath);
			throw new FileNotFoundException("File Not Exist", sourcePath);
		}
		string[] result = ConvertDataInChunck(sourcePath, start, end);
		Logger.Info("GetData is ended : file");
		return result;
	}

	public string GetHeader(string sourcePath)
	{
		Logger.InfoFormat("GetHeader is started : file {0}", sourcePath);
		if (!File.Exists(sourcePath))
		{
			Logger.InfoFormat("File Not Exist {0}", sourcePath);
			throw new FileNotFoundException("File Not Exist", sourcePath);
		}
		string result = ConvertHeader(sourcePath);
		Logger.InfoFormat("GetHeader is ended : file");
		return result;
	}

	public List<string> GetHeaderListWithoutDuplicate(string resultFile)
	{
		string header = GetHeader(resultFile);
		string[] source = header.Split(',');
		return source.ToList();
	}

	private string[] ConvertData(string resultFile)
	{
		Tuple<int, XmlNodeList, long> metaData = GetMetaData(resultFile);
		return GetData(resultFile, metaData.Item1, metaData.Item2, metaData.Item3);
	}

	private string[] ConvertDataInChunck(string resultFile, int start, int end)
	{
		Tuple<int, XmlNodeList, long> metaData = GetMetaData(resultFile);
		return GetDataInChunck(resultFile, metaData.Item1, metaData.Item2, start, end);
	}

	private string ConvertHeader(string resultFile)
	{
		Tuple<int, XmlNodeList, long> metaData = GetMetaData(resultFile);
		return GetHeader(metaData.Item1, metaData.Item2, metaData.Item3);
	}

	private Tuple<int, XmlNodeList, long> GetMetaData(string resultFile)
	{
		string filename = resultFile + ".md";
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(filename);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("/root/RecordSize/@Size");
		int num = int.Parse(xmlNode.Value);
		long item = new FileInfo(resultFile).Length / num;
		XmlNodeList item2 = xmlDocument.SelectNodes("/root/Fields/Field");
		return new Tuple<int, XmlNodeList, long>(num, item2, item);
	}

	private string GetHeader(int recordSize, XmlNodeList fieldNodes, long recordCount)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (XmlNode fieldNode in fieldNodes)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(fieldNode.Attributes["Name"].Value);
		}
		return stringBuilder.ToString();
	}

	private string[] GetData(string resultFile, int recordSize, XmlNodeList fieldNodes, long recordCount)
	{
		string[] array = new string[recordCount];
		StringBuilder stringBuilder = new StringBuilder();
		using (Stream stream = new FileStream(resultFile, FileMode.Open))
		{
			for (int i = 0; i < recordCount; i++)
			{
				byte[] buffer = new byte[recordSize];
				stream.Read(buffer, 0, recordSize);
				bool flag = true;
				for (int j = 0; j < fieldNodes.Count; j++)
				{
					XmlNode fieldNode = fieldNodes[j];
					string fieldValue = GetFieldValue(fieldNode, buffer);
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(fieldValue);
				}
				array[i] = stringBuilder.ToString();
				stringBuilder.Clear();
			}
		}
		return array;
	}

	private string[] GetDataInChunck(string resultFile, int recordSize, XmlNodeList fieldNodes, int start, int end)
	{
		int num = end - start;
		string[] array = new string[num];
		StringBuilder stringBuilder = new StringBuilder();
		using (Stream stream = new FileStream(resultFile, FileMode.Open))
		{
			stream.Seek(start * recordSize, SeekOrigin.Begin);
			for (int i = 0; i < num; i++)
			{
				byte[] buffer = new byte[recordSize];
				stream.Read(buffer, 0, recordSize);
				bool flag = true;
				for (int j = 0; j < fieldNodes.Count; j++)
				{
					XmlNode fieldNode = fieldNodes[j];
					string fieldValue = GetFieldValue(fieldNode, buffer);
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(fieldValue);
				}
				array[i] = stringBuilder.ToString();
				stringBuilder.Clear();
			}
		}
		return array;
	}

	private string GetFieldValue(XmlNode fieldNode, byte[] buffer)
	{
		VarEnum varEnum = (VarEnum)int.Parse(fieldNode.Attributes["Vartype"].Value);
		int num = int.Parse(fieldNode.Attributes["Offset"].Value);
		switch (varEnum)
		{
		case VarEnum.VT_UI1:
		{
			byte b = buffer[num];
			return b.ToString();
		}
		case VarEnum.VT_R4:
		{
			float num2 = BitConverter.ToSingle(buffer, num);
			return FormatValue(num2);
		}
		case VarEnum.VT_R8:
		{
			double value = BitConverter.ToDouble(buffer, num);
			return FormatValue(value);
		}
		case VarEnum.VT_I2:
			return BitConverter.ToInt16(buffer, num).ToString();
		case VarEnum.VT_I4:
		case VarEnum.VT_INT:
			return BitConverter.ToInt32(buffer, num).ToString();
		default:
			return "";
		}
	}

	private string FormatValue(double value)
	{
		string empty = string.Empty;
		if (value > 1000000.0 || value < -1000000.0)
		{
			return value.ToString("G5");
		}
		return value.ToString("F2");
	}
}

using System.Collections.Generic;


internal interface IReadFileService
{
	string[] GetData(string sourcePath);

	string GetHeader(string sourcePath);

	List<string> GetHeaderListWithoutDuplicate(string resultFile);
}

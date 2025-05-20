using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FUJIFilterExercise
{
    public class BarcodeFilterHelper
    {
        private Dictionary<char, string> dataTypePrefixes = new Dictionary<char, string>()
        {
            {'P', "Part barcode"},
            {'D', "DID"},
            {'Q', "Quantity (remaining quantity)"},
            {'V', "Vendor"},
            {'L', "LotNumber"},
            {'A', "Panel ID"},
            {'T', "Date"},
            {'G', "Lighting Class"},
            {'I', "Line"},
            {'M', "Machine"},
            {'N', "Module No"}
        };

        public Dictionary<string, string> ExtractBarcodeData(string barcode, List<string> filterPatterns)
        {
            Dictionary<string, string> extractedData = new Dictionary<string, string>();
            string currentBarcode = barcode;

            foreach (string patternLine in filterPatterns)
            {
                // 解析模式行: "met符号模式,数据类型,删除标志"
                string[] parts = patternLine.Split(',');
                if (parts.Length != 3) continue;

                string metPattern = parts[0].Trim();
                string dataType = parts[1].Trim();
                char deletionFlag = parts[2].Trim()[0];

                // 执行匹配
                bool success = TryMatchPattern(currentBarcode, metPattern,
                    out string extractedValue, out string remainingBarcode);

                if (success && dataTypePrefixes.ContainsKey(dataType[0]))
                {
                    extractedData[dataType] = extractedValue;

                    // 根据删除标志决定是否更新当前条形码
                    if (deletionFlag == 'Y')
                    {
                        currentBarcode = remainingBarcode;
                    }
                }
            }

            return extractedData;
        }

        private bool TryMatchPattern(string barcode, string pattern,
            out string extractedValue, out string remainingBarcode)
        {
            extractedValue = string.Empty;
            remainingBarcode = string.Empty;

            // 检查模式是否包含采样标记 <>
            int startMarkerIndex = pattern.IndexOf('<');
            int endMarkerIndex = pattern.IndexOf('>');

            // 模式中必须同时包含 < 和 > 且 < 必须在 > 之前
            if (startMarkerIndex < 0 || endMarkerIndex < 0 || startMarkerIndex >= endMarkerIndex)
            {
                return false;
            }

            // 提取采样标记前后的模式部分
            string prefixPattern = pattern.Substring(0, startMarkerIndex);
            string dataPattern = pattern.Substring(startMarkerIndex + 1, endMarkerIndex - startMarkerIndex - 1);
            string suffixPattern = pattern.Substring(endMarkerIndex + 1);

            // 将 MET 符号转换为正则表达式（使用非贪婪匹配）
            string regexPattern = "^" +
                MetToRegex(prefixPattern) +
                "(?<data>" + MetToRegex(dataPattern) + ")" +
                MetToRegex(suffixPattern) + "$";

            Regex regex = new Regex(regexPattern);
            Match match = regex.Match(barcode);

            if (!match.Success)
            {
                return false;
            }

            // 提取匹配的数据
            extractedValue = match.Groups["data"].Value;

            // 构建剩余的条形码
            int matchStart = match.Index;
            int matchLength = match.Length;
            remainingBarcode = barcode.Replace(extractedValue, "");

            return true;
        }

        private string MetToRegex(string metPattern)
        {
            // 将 MET 符号转换为等效的正则表达式
            string regex = Regex.Escape(metPattern);

            // 替换 MET 符号为非贪婪匹配
            regex = regex.Replace(@"\*", ".*?");  // * 转换为 .*? (任意数量的任意字符，非贪婪)
            regex = regex.Replace(@"\?", ".+?");  // ? 转换为 .+? (至少一个任意字符，非贪婪)

            return regex;
        }
    }
}
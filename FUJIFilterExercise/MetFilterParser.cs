using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FUJIFilterExercise
{
    public class MetFilterParser
    {
        private readonly List<FilterPattern> _patterns;

        public MetFilterParser(string metFileContent)
        {
            _patterns = ParseMetFile(metFileContent);
        }

        public BarcodeData ParseBarcode(string barcode)
        {
            var result = new BarcodeData();
            string remainingBarcode = barcode;

            foreach (var pattern in _patterns)
            {
                var match = pattern.Match(remainingBarcode);
                if (match.Success)
                {
                    // Extract the value between the met symbols
                    string extractedValue = match.Groups["value"].Value;

                    // Store the extracted value based on data type
                    switch (pattern.DataType)
                    {
                        case 'P': result.PartBarcode = extractedValue; break;
                        case 'D': result.DID = extractedValue; break;
                        case 'Q': result.Quantity = extractedValue; break;
                        case 'V': result.VendorID = extractedValue; break;
                        case 'L': result.LotNumber = extractedValue; break;
                        case 'A': result.PanelID = extractedValue; break;
                        case 'T': result.DateCode = extractedValue; break;
                        case 'G': result.LightingClass = extractedValue; break;
                        case 'I': result.Line = extractedValue; break;
                        case 'M': result.Machine = extractedValue; break;
                        case 'N': result.ModuleNo = extractedValue; break;
                    }

                    // If deletion flag is Y, remove the matched part from the remaining barcode
                    if (pattern.DeleteAfterMatch)
                    {
                        remainingBarcode = remainingBarcode.Remove(match.Index, match.Length);
                    }
                }
            }

            // If no patterns matched, use the entire barcode as the part barcode
            if (string.IsNullOrEmpty(result.PartBarcode))
            {
                result.PartBarcode = barcode;
            }

            return result;
        }

        private List<FilterPattern> ParseMetFile(string metFileContent)
        {
            var patterns = new List<FilterPattern>();
            var lines = metFileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("FTVS") || line == "<EOF>")
                    continue;

                var parts = line.Split(',').Select(p => p.Trim()).ToArray();
                if (parts.Length != 3)
                    continue;

                patterns.Add(new FilterPattern(parts[0], parts[1][0], parts[2] == "Y"));
            }

            return patterns;
        }

        private class FilterPattern
        {
            public string Pattern { get; }
            public char DataType { get; }
            public bool DeleteAfterMatch { get; }
            private Regex _regex;

            public FilterPattern(string pattern, char dataType, bool deleteAfterMatch)
            {
                Pattern = pattern;
                DataType = dataType;
                DeleteAfterMatch = deleteAfterMatch;
                _regex = CreateRegex(pattern);
            }

            public Match Match(string input)
            {
                return _regex.Match(input);
            }

            private Regex CreateRegex(string metPattern)
            {
                // Convert MET pattern to regex
                string regexPattern = "^"; // Start of string
                bool inCaptureGroup = false;

                foreach (char c in metPattern)
                {
                    if (c == '<')
                    {
                        inCaptureGroup = true;
                        regexPattern += "(?<value>"; // Start named capture group
                    }
                    else if (c == '>')
                    {
                        inCaptureGroup = false;
                        regexPattern += ")"; // End capture group
                    }
                    else if (c == '?')
                    {
                        regexPattern += inCaptureGroup ? "." : ".+"; // Any single character in capture, one or more outside
                    }
                    else if (c == '*')
                    {
                        regexPattern += ".*"; // Zero or more of any character
                    }
                    else if (c == ' ')
                    {
                        regexPattern += "\\s"; // Match whitespace
                    }
                    else
                    {
                        // Escape special regex characters
                        if ("[](){}.*+?^$\\".Contains(c))
                            regexPattern += "\\";
                        regexPattern += c;
                    }
                }

                regexPattern += "$"; // End of string
                return new Regex(regexPattern);
            }
        }
    }

    public class BarcodeData
    {
        public string PartBarcode { get; set; }
        public string DID { get; set; }
        public string Quantity { get; set; }
        public string VendorID { get; set; }
        public string LotNumber { get; set; }
        public string PanelID { get; set; }
        public string DateCode { get; set; }
        public string LightingClass { get; set; }
        public string Line { get; set; }
        public string Machine { get; set; }
        public string ModuleNo { get; set; }

        public override string ToString()
        {
            return $"Part: {PartBarcode}, DID: {DID}, Qty: {Quantity}, Vendor: {VendorID}, Lot: {LotNumber}, " +
                   $"Panel: {PanelID}, Date: {DateCode}, Lighting: {LightingClass}, Line: {Line}, " +
                   $"Machine: {Machine}, Module: {ModuleNo}";
        }
    }

    // 使用示例：
    // var parser = new BarcodeParser();
    // parser.LoadMetFile(File.ReadAllText("sample.met"));
    // var result = parser.ParseBarcode("P12345678ABCDEFG");
}

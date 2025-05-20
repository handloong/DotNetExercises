using System.Text.RegularExpressions;

namespace FUJIFilterExercise
{
    public class METParser
    {
        public List<(string Pattern, string Identifier, bool RemoveAfter)> ParseExpressions(string[] expressions)
        {
            var parsed = new List<(string, string, bool)>();
            foreach (var expr in expressions)
            {
                var parts = expr.Split(',');
                if (parts.Length != 3) throw new ArgumentException("Invalid expression format");
                var pattern = parts[0];
                var identifier = parts[1];
                var remove = parts[2] == "Y";
                parsed.Add((pattern, identifier, remove));
            }
            return parsed;
        }

        public Dictionary<string, string> ProcessExpressions(string input, List<(string Pattern, string Identifier, bool RemoveAfter)> expressions)
        {
            var results = new Dictionary<string, string>();
            string current = input;

            foreach (var expr in expressions)
            {
                var parts = expr.Pattern.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) throw new ArgumentException("Invalid MET pattern");

                string prefixPattern = parts[0];
                string capturePattern = parts[1];

                // Convert MET patterns to regex
                string regexPrefix = ConvertToRegex(prefixPattern);
                string regexCapture = ConvertToRegex(capturePattern);

                // Match entire string with prefix and capture
                var regex = new Regex($"^{regexPrefix}({regexCapture})$");
                var match = regex.Match(current);

                if (match.Success && match.Groups.Count > 1)
                {
                    string captured = match.Groups[1].Value;
                    results[expr.Identifier] = captured;

                    if (expr.RemoveAfter)
                    {
                        current = current.Substring(0, match.Groups[1].Index) + current.Substring(match.Groups[1].Index + captured.Length);
                    }
                }
                else
                {
                    results[expr.Identifier] = null; // Indicate no match
                }
            }

            return results;
        }

        private string ConvertToRegex(string metPattern)
        {
            // Convert MET syntax to regex
            string regex = metPattern
                .Replace("*", ".*?") // Non-greedy match
                .Replace("?", ".");
            return regex;
        }
    }
}
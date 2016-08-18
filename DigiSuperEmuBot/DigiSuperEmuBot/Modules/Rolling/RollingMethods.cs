using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DigiSuperEmuBot.Modules.Rolling
{
    public static class RollingMethods
    {
        private static readonly Regex dndRegex = new Regex(@"(?<n1>\d+)d(?<n2>\d+)", RegexOptions.Compiled);
        public static string Evaluate(string expression)
        {
            var dataTable = new DataTable();
            var column = new DataColumn("Eval", typeof(double), expression);
            dataTable.Columns.Add(column);
            dataTable.Rows.Add(0);
            return dataTable.Rows[0]["Eval"].ToString();
        }

        public static string[] DoRoll(string arg)
        {
            var r = new Random();
            var rollExpression = "";
            var prevSubstring = 0;
            foreach (Match match in dndRegex.Matches(arg))
            {
                var newSubstring = arg.IndexOf(match.ToString(), StringComparison.Ordinal);
                var preInfo = arg.Substring(prevSubstring, newSubstring - prevSubstring);
                prevSubstring = match.ToString().Length + newSubstring;
                var n1 = 0;
                var n2 = 0;
                var computedRolls = "";
                if (int.TryParse(match.Groups["n1"].ToString(), out n1) &&
                    int.TryParse(match.Groups["n2"].ToString(), out n2) &&
                    n1 <= 50 && n2 <= 100000)
                {
                    var arr = new int[n1];
                    for (var i = 0; i < n1; i++)
                    {
                        arr[i] += r.Next(1, n2 + 1);
                    }
                    var elemCnt = 0;
                    computedRolls = "(" +
                                    string.Join("+",
                                        arr.OrderBy(x => x)
                                            .Select(x => elemCnt++ % 2 == 0 ? $"{x}" : x.ToString())) + ")";
                }
                rollExpression += preInfo + computedRolls;
            }
            rollExpression += arg.Substring(prevSubstring, arg.Length - prevSubstring);
            rollExpression = rollExpression.Replace(" ", string.Empty);
            var answer = Evaluate(rollExpression);

            return new[] { rollExpression, answer };
        }

        public static bool TestRoll(string arg)
        {
            try { DoRoll(arg); return true; }
            catch { return false; }
        }
    }
}

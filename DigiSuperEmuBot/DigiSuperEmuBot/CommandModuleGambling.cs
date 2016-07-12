using Discord;
using Discord.Commands;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace DigiSuperEmuBot
{
    static class CommandModuleGambling
    {
        public class Rolling
        {
            public void Setup(DiscordClient client)
            {
                client.GetService<CommandService>().CreateCommand("roll")
                    .Alias("r", "roll")
                    .Description("Roll dice in a #d# format")
                    .Parameter("InputRoll", ParameterType.Required)
                    .Do(RollFunc());
            }
            private static double Evaluate(string expression)
            {
                DataTable dataTable = new DataTable();
                DataColumn column = new DataColumn("Eval", typeof(double), expression);
                dataTable.Columns.Add(column);
                dataTable.Rows.Add(0);
                return (double)dataTable.Rows[0]["Eval"];
            }

            private Regex dndRegex = new Regex(@"(?<n1>\d+)d(?<n2>\d+)", RegexOptions.Compiled);

            private Func<CommandEventArgs, Task> RollFunc()
            {
                var r = new Random();
                return async e =>
                {
                    var arg = e.Args[0]?.Trim();
                    if (arg.IndexOf('d') != -1 && arg.IndexOf('d') != 0)
                    {
                        try
                        {
                            string rollExpression = "";
                            int prevSubstring = 0;
                            foreach (Match match in dndRegex.Matches(arg))
                            {
                                int newSubstring = arg.IndexOf(match.ToString(), StringComparison.Ordinal);
                                string preInfo = arg.Substring(prevSubstring, newSubstring - prevSubstring);
                                prevSubstring = match.ToString().Length + newSubstring;
                                int n1 = 0;
                                int n2 = 0;
                                string computedRolls = "";
                                if (int.TryParse(match.Groups["n1"].ToString(), out n1) &&
                                    int.TryParse(match.Groups["n2"].ToString(), out n2) &&
                                    n1 <= 50 && n2 <= 100000)
                                {
                                    var arr = new int[n1];
                                    for (int i = 0; i < n1; i++)
                                    {
                                        arr[i] += r.Next(1, n2 + 1);
                                    }
                                    int elemCnt = 0;
                                    computedRolls = "(" + string.Join("+", arr.OrderBy(x => x).Select(x => elemCnt++ % 2 == 0 ? $"{x}" : x.ToString())) + ")";
                                }
                                rollExpression += preInfo + computedRolls;
                            }
                            rollExpression += arg.Substring(prevSubstring, arg.Length - prevSubstring);
                            rollExpression = rollExpression.Replace(" ", string.Empty);
                            double answer = Evaluate(rollExpression);
                            await e.Channel.SendMessage($"`Rolled {rollExpression}`\n`Result:` {answer}").ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await e.Channel.SendMessage("`" + ex.Message + "`").ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessage("`There was nothing to process... Need a d somewhere.`").ConfigureAwait(false);
                    }
                };
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;
//TODO: Finish up alert and remind me.
//TODO: Requires a Regex with a format of 1h2m3s, with dropable values.
/*
namespace DigiSuperEmuBot.Modules.Reminder
{
    internal class ReminderModule : IModule
    {

        private DiscordClient _client;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateCommand("r")
                .Description("Roll dice in a #d# format")
                .Parameter("InputAlert", ParameterType.Required)
                .Do(AlertFunc());
            });
        }

        private Regex timeRegex = new Regex(@"(?<n1>\d+)h(?<n2>\d+)m(?<n3>\d+)s", RegexOptions.Compiled);
        private Func<CommandEventArgs, Task> AlertFunc()
        {
            return async e =>
            {

            }
        }
    }
}
*/
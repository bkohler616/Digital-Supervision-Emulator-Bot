using Discord;
using Discord.Commands;

namespace DigiSuperEmuBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();
        private static DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient();
            SetupCommands();
            _client.ExecuteAndWait(async () => {
                await _client.Connect("MjAxNDg5MzQ1NDgyNDU3MDg4.CmNHTA.vdLHCu7IXu9fnycTj9uKECX2F2Y");
            });
        }

        public void SetupCommands()
        {
            _client.UsingCommands(x =>
            {
                x.PrefixChar = '~';
                x.HelpMode = HelpMode.Disabled;
            });

            var rolling = new CommandModuleGambling.Rolling();
            rolling.Setup(_client);
        }
    }
}

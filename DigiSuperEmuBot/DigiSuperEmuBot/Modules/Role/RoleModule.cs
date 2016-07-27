using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;


namespace DigiSuperEmuBot.Modules
{
    internal class RoleModule : IModule
    {
        private DiscordClient _client;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager) {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb => {
                                           cgb.CreateCommand("role.giveRole")
                                              .Description("Give admin rights to someone")
                                              .Parameter("User", ParameterType.Multiple)
                                              .Do(GiveAdmin());
                                       });
        }

        private Func<CommandEventArgs, Task> GiveAdmin() {
            return async e => {
                             var userIDstring = e.Args [0]?.Trim();
                             var roleDesired = e.Args [1]?.Trim();
                             bool allowedRole = false;
                             foreach (var i in e.User.Roles) {
                                 
                             }
                             Discord.Role desiredRole = e.User.Roles.GetEnumerator().Current;
                             userIDstring = userIDstring.Substring(2, userIDstring.Length - 3);
                             var userID = Convert.ToUInt64(userIDstring);
                             foreach (var i in _client.Servers) {
                                 foreach (var j in i.Roles) {
                                     if (j.Name == roleDesired)
                                         desiredRole = j;
                                 }
                                 foreach (var j in i.Users) {
                                     if(j.Id == userID)
                                        await j.AddRoles(new []{desiredRole});
                                 }
                             }

                             await e.Channel.SendMessage($"`Attempting to give role to {userIDstring} from user {e.User}`");
                         };
        }
    }

    
}
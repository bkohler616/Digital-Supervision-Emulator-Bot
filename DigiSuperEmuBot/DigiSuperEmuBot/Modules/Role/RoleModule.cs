﻿using System;
using System.Threading.Tasks;
using DigiSuperEmuBot.Modules.Role;
using Discord;
using Discord.Commands;
using Discord.Modules;

namespace DigiSuperEmuBot.Modules
{
    internal class RoleModule : IModule
    {
        private DiscordClient _client;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateCommand("role.giveRole")
                    .Description("Give admin rights to someone")
                    .Parameter("User", ParameterType.Multiple)
                    .Do(GiveAdmin());
            });
        }

        private Func<CommandEventArgs, Task> GiveAdmin()
        {
            return async e =>
            {
                string userIDstring = e.Args[0]?.Trim();
                string roleDesired = e.Args[1]?.Trim();
                for (int i =2; i < e.Args.Length; i++)
                        roleDesired = roleDesired + " " + e.Args[i]?.Trim();
                
                if (userIDstring == "" || roleDesired == "" || userIDstring == null || roleDesired == null)
                {
                    await e.Channel.SendMessage("Format required: ~role.giveRole <User via @> <Role>");
                    return;
                }
                var allowedRole = false;
                foreach (var i in e.User.Roles)
                {
                    if(i.Name == TheOnyxTowerRoles.Admin||i.Name == TheOnyxTowerRoles.MinorAdmin)
                    {
                        allowedRole = true;
                    }
                }
                if (!allowedRole)
                {
                    await e.Channel.SendMessage("You do not have the proper role to do this. Sorry.");
                    return;
                }

                var desiredRole = e.User.Roles.GetEnumerator().Current; //Using this as temp role initializer.
                allowedRole = false;
                userIDstring = userIDstring.Substring(2, userIDstring.Length - 3);
                var userID = Convert.ToUInt64(userIDstring);
                foreach (var i in _client.Servers)
                {
                    foreach (var j in i.Roles)
                    {
                        if (j.Name == roleDesired)
                        {
                            desiredRole = j;
                            allowedRole = true;
                        }
                    }
                    if (!allowedRole)
                    {
                        await e.Channel.SendMessage("That role does not exist. Try again?");
                        return;
                    }
                    foreach (var j in i.Users)
                    {
                        if (j.Id == userID)
                            await j.AddRoles(desiredRole);
                    }
                }

                await e.Channel.SendMessage($"Role {roleDesired} grantede to {userIDstring} from user {e.User}");
            };
        }
    }
}
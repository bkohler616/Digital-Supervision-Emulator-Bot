using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Discord;
using Discord.Commands;
using Discord.Modules;

namespace DigiSuperEmuBot.Modules.Rolling
{
    internal class RollingModule : IModule
    {
        private static readonly string _path = "CustomRollData.xml";

        private DiscordClient _client;
        private CustomRollList _customRolls;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            if (!RetrieveCustomRolls())
            {
                _customRolls = new CustomRollList(); 
            }
                

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateCommand("roll.rollCust")
                    .Description("Roll a custom roll.")
                    .Parameter("customRoll", ParameterType.Required)
                    .Do(RollCustom());
                cgb.CreateCommand("roll.createCust")
                    .Description("Create a custom roll.")
                    .Parameter("customRoll", ParameterType.Multiple)
                    .Do(CreateCustom());
                cgb.CreateCommand("roll.changeCust")
                    .Description("Change a custom roll.")
                    .Parameter("customRoll", ParameterType.Multiple)
                    .Do(ChangeCustom());
                cgb.CreateCommand("roll.deleteCust")
                    .Description("Delete a custom roll.")
                    .Parameter("customRoll", ParameterType.Multiple)
                    .Do(DeleteCustom());
                cgb.CreateCommand("roll.listCust")
                    .Description("List the users custom rolls.")
                    .Do(ListCustom());
                cgb.CreateCommand("roll.purgeCust")
                    .Description("Purge all rolls for the user")
                    .Do(PurgeRolls());
                cgb.CreateCommand("r")
                    .Description("Roll dice in a #d# format")
                    .Parameter("InputRoll", ParameterType.Required)
                    .Do(RollFunc());
            });
        }


        private Func<CommandEventArgs, Task> RollFunc()
        {
            return async e =>
            {
                var arg = e.Args[0]?.Trim();
                if (arg.IndexOf('d') != -1 && arg.IndexOf('d') != 0)
                {
                    try
                    {
                        var evaluation = RollingMethods.DoRoll(arg);
                        await
                            e.Channel.SendMessage($"`Rolled {evaluation[0]}`\n`Result:` {evaluation[1]}")
                                .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await e.Channel.SendMessage("`" + ex.Message + "`").ConfigureAwait(false);
                    }
                }
                else
                {
                    await
                        e.Channel.SendMessage("`There was nothing to process... Need a d somewhere.`")
                            .ConfigureAwait(false);
                }
            };
        }

        private Func<CommandEventArgs, Task> RollCustom()
        {
            return async e =>
            {
                var rollName = e.Args[0].Trim();
                var uId = e.User.Id.ToString();

                foreach (var customRollData in _customRolls.CustomRollDataList)
                {
                    if (customRollData.UserId.Equals(uId))
                    {
                        var customRoll = customRollData.GetRoll(rollName);
                        if (customRoll.Equals("failure"))
                        {
                            await e.Channel.SendMessage("`Roll was not found. Try again, or create that roll name!`");
                            return;
                        }
                        string builder = $"`Roll grabbed: {rollName} - {customRoll}";
                        var evaluation = RollingMethods.DoRoll(customRoll);
                        builder += $"\nRolled {evaluation[0]}\nResult: {evaluation[1]}`";
                        await e.Channel.SendMessage(builder);
                        return;
                    }
                }

                await e.Channel.SendMessage("User not found, thus no data found. Create some stuff!");
            };
        }

        private Func<CommandEventArgs, Task> CreateCustom()
        {
            return async e =>
            {
                var rollInput = e.Args[1].Trim();
                var rollName = e.Args[0].Trim();
                var uId = e.User.Id.ToString();
                var userExists = false;
                var counter = -1;
                for (var i = 0; i < _customRolls.CustomRollDataList.Count; i++)
                {
                    if (!_customRolls.CustomRollDataList[i].UserId.Equals(uId)) continue;
                    userExists = true;
                    counter = i;
                }
                if (!userExists)
                {
                    _customRolls.CustomRollDataList.Add(new CustomRollList.CustomRollData(uId));
                    counter = _customRolls.CustomRollDataList.Count - 1;
                }

                if (!_customRolls.CustomRollDataList[counter].CreateRoll(rollName, rollInput))
                {
                    await e.Channel.SendMessage("Roll creation failed. Bad input or roll name was taken.");
                }
                else
                {
                    await e.Channel.SendMessage("`Roll creation succeeded." +
                                                $"\nRoll Name: {rollName} " +
                                                $"\nRoll when called:{rollInput}`");
                }
                SaveCustomRolls();

                
            };
        }

        private Func<CommandEventArgs, Task> DeleteCustom()
        {
            return async e =>
            {
                var rollName = e.Args[0].Trim();
                var uId = e.User.Id.ToString();

                foreach (var customRollData in _customRolls.CustomRollDataList)
                {
                    if (!customRollData.UserId.Equals(uId)) continue;
                    if (customRollData.DeleteRoll(rollName))
                    {
                        await e.Channel.SendMessage($"`Roll '{rollName}' Deleted!`");
                        SaveCustomRolls();
                        return;
                    }
                    await e.Channel.SendMessage($"`Roll '{rollName}' was not found. At least it's gone!`");
                    return;
                }

                await e.Channel.SendMessage("User not found, thus no data found. That means nothing to delete!");
            };
        }

        private Func<CommandEventArgs, Task> ChangeCustom()
        {
            return async e => {
                var rollName = e.Args[0].Trim();
                var rollInput = e.Args[1].Trim();
                var uId = e.User.Id.ToString();

                foreach (var customRollData in _customRolls.CustomRollDataList)
                {
                    if (!customRollData.UserId.Equals(uId)) continue;
                    if (customRollData.ChangeRoll(rollName, rollInput))
                    {
                        SaveCustomRolls();
                        await e.Channel.SendMessage($"`Roll '{rollName}' changed to {rollInput}!`");
                        return;
                    }
                    await e.Channel.SendMessage($"`Roll '{rollName}' was not found. Try creating it!`");
                    return;
                }

                await e.Channel.SendMessage("User not found, thus no data found. Should try to create instead!");
            };
        }

        private Func<CommandEventArgs, Task> ListCustom()
        {
            return async e =>
            {
                var uId = e.User.Id.ToString();

                foreach (var customRollData in _customRolls.CustomRollDataList)
                {
                    if (!customRollData.UserId.Equals(uId)) continue;

                    var builder = "`Rolls found:" + customRollData.ListRolls() + "`";
                    await e.Channel.SendMessage(builder);
                    return;
                }

                await e.Channel.SendMessage("No user found. Make some rolls to list your rolls!");
            };
        }

        private Func<CommandEventArgs, Task> PurgeRolls()
        {
            return async e =>
            {
                var uId = e.User.Id.ToString();

                foreach (var customRollData in _customRolls.CustomRollDataList)
                {
                    if (!customRollData.UserId.Equals(uId)) continue;

                    customRollData.PurgeRolls();
                    SaveCustomRolls();
                    await e.Channel.SendMessage("Rolls purged");
                    return;
                }

                await e.Channel.SendMessage("No user found. Make some rolls to purge those rolls!");
            };
        }

        private bool RetrieveCustomRolls()
        {
            try
            {
                if (!File.Exists(_path)) return false;
                var reader = new XmlSerializer(typeof(CustomRollList));
                var file = new StreamReader(_path);
                _customRolls = (CustomRollList) reader.Deserialize(file);
                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load data - {ex.Message}");
                return false;
            }
        }

        private void SaveCustomRolls()
        {
            try
            {
                var writer = new XmlSerializer(typeof(CustomRollList));
                var file = File.Create(_path);
                writer.Serialize(file, _customRolls);
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save data - {ex.Message}\n\n{ex.InnerException}\n\n{ex.StackTrace}");
            }
        }
    }
}
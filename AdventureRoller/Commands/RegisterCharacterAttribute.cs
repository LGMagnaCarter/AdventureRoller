using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using AdventureRoller.Services;
using System;

namespace AdventureRoller.Commands
{
    public class RegisterCharacterAttribute : ModuleBase<SocketCommandContext>
    {
        private ICharacterService CharacterService { get; }
        private ICharacterAttributeService CharacterAttributeService { get; }

        public RegisterCharacterAttribute(ICharacterService characterService, ICharacterAttributeService characterAttributeService)
        {
            CharacterService = characterService;
            CharacterAttributeService = characterAttributeService;
        }

        [Command("registercharacterattribute")]
        public async Task Setup()
        {
            await ReplyAsync(new Help().GetHelpInfo("registercharacterattribute"));
        }

        [Command("registerattribute")]
        public async Task Setup(string name, string level, [Remainder]string schema)
        {
            try
            {
                Dictionary<string, string> values;
                try
                {
                    values = JsonConvert.DeserializeObject<Dictionary<string, string>>(schema);
                }
                catch (Exception e)
                {
                    await ReplyAsync("Invalid Format!");

                    await Task.CompletedTask;
                    return;
                }

                if (!int.TryParse(level, out int level2) || level2 < 0)
                {

                    await ReplyAsync($"{nameof(level)} has to be a positive integer!");

                    await Task.CompletedTask;
                    return;
                }

                var discordId = Context.Message.Author.Id;

                var characterId = CharacterService.GetCharacter(discordId, name, level2).CharacterId;

                foreach (var value in values)
                {

                    var response = CharacterAttributeService.AddCharacterAttribute(characterId, level2, value.Key, value.Value);

                    if (response.Success)
                    {
                        await ReplyAsync($"{value.Key} registered!");
                    }
                    else
                    {
                        await ReplyAsync($"Registration failed: {response.Error}");
                    }
                }

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                await ReplyAsync("Registration Failed!");

                await Task.CompletedTask;
            }
        }

    }
}

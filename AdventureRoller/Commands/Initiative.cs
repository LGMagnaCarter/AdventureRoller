using Discord.Commands;
using System.Threading.Tasks;
using AdventureRoller.Services;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace AdventureRoller.Commands
{
    public class Initiative : ModuleBase<SocketCommandContext>
    {
        private Regex DiceRegex = new Regex("[0-9]{0,45}[d][0-9]{1,45}");
        private IDiceService DiceService { get; }
        private Dictionary<string, IEditionsService> EditionService { get; }
        private ICharacterService CharacterService { get; }

        public Initiative(IDiceService diceService, ICharacterService characterService, IEnumerable<IEditionsService> editionsService)
        {
            CharacterService = characterService;
            DiceService = diceService;
            EditionService = editionsService.ToDictionary(x => x.ToString());
        }

        [Command("initiative")]
        public async Task RollInitiative([Remainder] string edition)
        {
            if(string.IsNullOrEmpty(edition))
            {
                edition = "Default";
            }

            try
            {
                var caller = Context.Guild.GetUser(Context.Message.Author.Id);

                List<ulong> players = caller.VoiceChannel.Users.Where(u => !u.IsBot).Select(u => u.Id).ToList();

                players.Remove(caller.Id);

                SortedDictionary<ulong, int> playerScores = new SortedDictionary<ulong, int>();

                List<string> failedInitiative = new List<string>();

                foreach (var player in players)
                {
                    var attributeResponse = CharacterService.GetAttribute(player, "Init");

                    if (!attributeResponse.Success)
                    {
                        await ReplyAsync($"`could not retrieve {Context.Guild.GetUser(player).Username} initiative. Error: {attributeResponse.Error}");
                        continue;
                    }

                    if (!int.TryParse(attributeResponse.Value, out int init))
                    {
                        failedInitiative.Add(Context.Guild.GetUser(player).Mention);
                    }

                    var editionservice = EditionService[edition];

                    var roll = editionservice.GetRoll("Initiative");

                    playerScores.Add(player, DiceService.RollExactDice(roll).Sum() + init);
                }

                if (failedInitiative.Any())
                {
                    await ReplyAsync($"Players not registered: {string.Join(',', failedInitiative)}");
                }

                string response = string.Empty;
                foreach (var player in playerScores.OrderByDescending(x => x.Value))
                {
                    response += $"`[{Context.Guild.GetUser(player.Key).Username}]` : {player.Value}\r\n";
                }


                await ReplyAsync(response);
            }
            catch (Exception e)
            {
                string s = e.Message;
            }

            await Task.CompletedTask;
        }
    }
}
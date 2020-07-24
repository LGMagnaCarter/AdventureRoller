using Discord.Commands;
using System.Threading.Tasks;
using AdventureRoller.Services;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventureRoller.Commands
{
    public class Initiative : ModuleBase<SocketCommandContext>
    {
        private Regex DiceRegex = new Regex("[0-9]{0,45}[d][0-9]{1,45}");
        private IDiceService DiceService { get; }
        private ICharacterService CharacterService { get; }

        public Initiative(IDiceService diceService, ICharacterService characterService)
        {
            CharacterService = characterService;
            DiceService = diceService;
        }

        [Command("initiative")]
        public async Task RollInitiative()
        {
            var caller = Context.Guild.GetUser(Context.Message.Author.Id);

            List<ulong> players = caller.VoiceChannel.Users.Select(u => u.Id).ToList();

            players.Remove(caller.Id);

            SortedDictionary<int, ulong> playerScores = new SortedDictionary<int, ulong>();

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
                playerScores.Add(DiceService.RollExactDice("1d20").Sum() + init, player);
            }

            if (failedInitiative.Any())
            {
                await ReplyAsync($"Players not registered: {string.Join(',', failedInitiative)}");
            }

            string response = string.Empty;
            foreach (var player in playerScores.Reverse())
            {
                response += $"`[{Context.Guild.GetUser(player.Value).Username}]` : {player.Key}\r\n"; 
            }

            await ReplyAsync(response);

            await Task.CompletedTask;
        }
    }
}
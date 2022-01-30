namespace AdventureRoller.Commands
{
    using AdventureRoller.Services;
    using Discord.Commands;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Rolls : ModuleBase<SocketCommandContext>
    {
        private static ulong lastMessageSender { get; set; }
        private ICharacterService CharacterService { get; }

        private IDiceService DiceService { get; }

        private Dictionary<string, IEditionsService> EditionService { get; }

        private Regex DiceRegex = new Regex("[0-9]{0,45}[d][0-9]{1,45}(?:[r][0-9]{1,3}|)(?:[s][0-9]{1,3}|)(?:[f][0-9]{1,3}|)(?:[h][0-9]{1,3}|)(?:[l][0-9]{1,3}|)");

        private Regex ModifiersRegex = new Regex(@"(?<! )[\*\+\-\/](?! )");

        private Regex WordRegex = new Regex("[A-z]{3,15}");

        public Rolls(ICharacterService characterService, IDiceService diceService, IEnumerable<IEditionsService> editionsService)
        {
            CharacterService = characterService;
            DiceService = diceService;
            EditionService = editionsService.ToDictionary(x => x.ToString());
        }

        [Command("roll")]
        public async Task Roll([Remainder] string diceString)
        {
            foreach (string diceRoll in diceString.Split('|'))
            {
                await RollDice(diceRoll);
            }
            await Context.Message.DeleteAsync();
        }

        [Command("chance")]
        public async Task Chance()
        {
            var character = CharacterService.GetCharacter(Context.Message.Author.Id);
            var editionService = EditionService[character != null ? character.Edition : "Default"];

            string diceString = editionService.GetRoll("Chance", null);

            await RollDice(diceString);

            await Context.Message.DeleteAsync();
        }

        [Command("rote")]
        public async Task Rote(string amount)
        {
            string edition = null;
            if (string.IsNullOrEmpty(edition))
            {
                var character = CharacterService.GetCharacter(Context.Message.Author.Id);

                edition = character != null ? character.Edition : "Default";
            }

            var editionservice = EditionService[edition];

            var diceString = editionservice.GetRoll("Rote", new object[] { int.Parse(amount) });

            await RollDice(diceString);

            await Context.Message.DeleteAsync();
        }

        private async Task RollDice(string diceString)
        {
            List<string> modifiers = diceString.Split('\\').ToList();

            diceString = modifiers.First();

            modifiers.RemoveAt(0);

            if (Context.Message.Author.Id != lastMessageSender)
            {
                await ReplyAsync("_ _");
            }

            lastMessageSender = Context.Message.Author.Id;

            var character = CharacterService.GetCharacter(Context.Message.Author.Id);
            var editionService = EditionService[character != null ? character.Edition : "Default"];

            diceString = diceString.ToLower().Replace(" ", "");

            var displayEquationString = diceString;
            var equationString = diceString;

            Match match = WordRegex.Match(equationString);
            while (match.Success)
            {
                var attributeResponse = CharacterService.GetAttribute(Context.Message.Author.Id, match.Value);

                if (!attributeResponse.Success)
                {
                    await ReplyAsync($"Error: {match.Value} - {attributeResponse.Error}");
                }

                var regex = new Regex(Regex.Escape(match.Value));
                displayEquationString = regex.Replace(displayEquationString, $"[{match.Value} = {attributeResponse.Value}]", 1);
                equationString = regex.Replace(equationString, attributeResponse.Value, 1);

                match = WordRegex.Match(equationString);
            }

            equationString = editionService.PrepRoll(equationString);

            if (!DiceRegex.Match(equationString).Success)
            {
                equationString = editionService.GetDefaultRoll(equationString);
                displayEquationString = editionService.GetDefaultRoll(displayEquationString);
            }

            equationString = editionService.ModifyRoll(equationString, modifiers);
            displayEquationString = editionService.ModifyRoll(displayEquationString, modifiers);
            diceString = editionService.ModifyRoll(diceString, modifiers);

            match = DiceRegex.Match(equationString);
            while (match.Success)
            {
                var rolls = DiceService.RollExactDice(match.Value);

                var regex = new Regex(Regex.Escape(match.Value));
                displayEquationString = regex.Replace(displayEquationString, $"[{match.Value} = {string.Join(", ", rolls.Select(n => n.ToString()).ToArray())}]", 1);

                equationString = regex.Replace(equationString, editionService.ParseRoll(rolls, match.Value), 1);

                match = DiceRegex.Match(equationString);
            }

            var username = Context.Message.Author.Mention;

            var result = editionService.CompleteRoll(equationString);

            await ReplyAsync($"{username} **{CleanUp(diceString)}** roll: \r\n`{displayEquationString}` results in... **{result}**");
        }

        private string CleanUp(string s)
        {
            foreach (Match match in ModifiersRegex.Matches(s))
            {
                s = ModifiersRegex.Replace(s, $" {match.Value} ", 1);
            }
            return s;
        }
    }
}

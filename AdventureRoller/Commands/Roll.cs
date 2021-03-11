namespace AdventureRoller.Commands
{
    using AdventureRoller.Services;
    using Discord.Commands;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Roll : ModuleBase<SocketCommandContext>
    {
        private ICharacterService CharacterService { get; }

        private IDiceService DiceService { get; }

        private Regex DiceRegex = new Regex("[0-9]{0,45}[d][0-9]{1,45}[h]?[0-9]?[l]?[0-9]?");

        private Regex ModifiersRegex = new Regex(@"(?<! )[\*\+\-\/](?! )");

        private Regex WordRegex = new Regex("[A-z]{3,15}");

        public Roll(ICharacterService characterService, IDiceService diceService)
        {
            CharacterService = characterService;
            DiceService = diceService;
        }

        [Command("roll")]
        public async Task Setup([Remainder]string diceString)
        {
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
                displayEquationString = regex.Replace(displayEquationString, $"`[{match.Value} = {attributeResponse.Value}]`", 1);
                equationString = regex.Replace(equationString, attributeResponse.Value, 1);

                match = WordRegex.Match(equationString);
            }

            // if no diceroll, add 1d20
            if(!DiceRegex.Match(equationString).Success)
            {
                equationString = $"1d20 + {equationString}";
                displayEquationString = $"`1d20` + {displayEquationString}";
            }

            match = DiceRegex.Match(equationString);
            while(match.Success)
            {
                var rolls = DiceService.RollExactDice(match.Value);

                var regex = new Regex(Regex.Escape(match.Value));
                displayEquationString = regex.Replace(displayEquationString, $"[{match.Value} = {string.Join(", ", rolls.Select(n => n.ToString()).ToArray())}]", 1);

                equationString = regex.Replace(equationString, rolls.Sum().ToString(), 1);

                match = DiceRegex.Match(equationString);
            }

            var username = Context.Message.Author.Username;

            displayEquationString = $"{username} roll: {displayEquationString}";

            DataTable dt = new DataTable();

            double result = 0;

            var temp = dt.Compute(equationString, string.Empty).ToString();
            result = double.Parse(temp);

            await ReplyAsync($"{CleanUp(displayEquationString)} results in **{result}**");
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

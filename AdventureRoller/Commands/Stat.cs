namespace AdventureRoller.Commands
{
    using AdventureRoller.Services;
    using Discord.Commands;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Stat : ModuleBase<SocketCommandContext>
    {
        private ICharacterService CharacterService { get; }

        private Regex WordRegex = new Regex("[A-z]{3,15}");

        public Stat(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("stat")]
        public async Task Setup([Remainder]string statString)
        {
            statString = statString.ToLower().Replace(" ", "");

            var displayEquationString = statString;
            var equationString = statString;

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

            var username = Context.Message.Author.Username;

            displayEquationString = $"{username} stat: {displayEquationString}";

            DataTable dt = new DataTable();

            double result = 0;

            var temp = dt.Compute(equationString, string.Empty).ToString();
            result = double.Parse(temp);

            await ReplyAsync($"{displayEquationString} results in **{result}**");
        }
    }
}

using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace AdventureRoller.Commands
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Setup(string directory = "")
        {
            await ReplyAsync(GetHelpInfo(directory));
        }

        public string GetHelpInfo(string directory)
        {
            if( directory.Contains("/"))
            {
                return $"HELP! {Context.Message.Author.Mention} is trying to look under my dress!";
            }

            var path = $"../../../Documentation/help{directory.ToLower()}.txt";

            if (!File.Exists(path))
            {
                path = $"../../../Documentation/nohelp.txt";
            }

            return File.ReadAllText(path);
        }
    }
}

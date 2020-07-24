using Discord.Commands;
using System.Threading.Tasks;
using AdventureRoller.DatabaseContext;
using Newtonsoft.Json;
using AdventureRoller.Services;

namespace AdventureRoller.Commands
{
    public class Editions : ModuleBase<SocketCommandContext>
    {
        private IEditionsService EditionsService { get; }

        public Editions(IEditionsService editionsService)
        {
            EditionsService = editionsService;
        }


        [Command("editions")]
        public async Task ListEditions()
        {
            await ReplyAsync(string.Join(",", EditionsService.GetEditions()));
            await Task.CompletedTask;
        }

        [Command("editions")]
        public async Task GetEdition(string edition)
        {
            await ReplyAsync(EditionsService.GetEdition(edition));
            await Task.CompletedTask;
        }
    }
}

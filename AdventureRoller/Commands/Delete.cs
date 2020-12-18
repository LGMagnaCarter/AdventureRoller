using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using AdventureRoller.Services;
using System;
using Discord.WebSocket;
using System.Linq;
using AdventureRoller.Models;

namespace AdventureRoller.Commands
{
    public class Delete : ModuleBase<SocketCommandContext>
    {
        private static Emoji checkmark = new Emoji("\u2705");
        private static Emoji cross = new Emoji("\u274C");

        private ICharacterService CharacterService { get; }

        public Delete(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("deletelevel")]
        public async Task Setup(string name, int level)
        {
            try
            {
                var dialog = await ReplyAsync($"{Context.Message.Author.Mention}, are you sure you want to delete {name} at level {level}?");
                await dialog.AddReactionAsync(checkmark);
                await dialog.AddReactionAsync(cross);
                Context.Client.ReactionAdded += new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>(
                    (Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel message, SocketReaction reaction) =>
                    {
                        return Client_ReactionAdded(cache, message, reaction, name, level);
                    });
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        [Command("deletecharacter")]
        public async Task Setup(string name)
        {
            var dialog = await ReplyAsync($"{Context.Message.Author.Mention}, are you sure you want to delete {name} at all levels?");
            await dialog.AddReactionAsync(checkmark);
            await dialog.AddReactionAsync(cross);
            Context.Client.ReactionAdded += new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>(
                (Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel message, SocketReaction reaction) => {
                    return Client_ReactionAdded(cache, message, reaction, name);
                });
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel message, SocketReaction reaction, string name, int? level = null)
        {
            if (Context.Message.Author.Id != reaction.User.Value.Id)
            {
                return;
            }

            if (reaction.Emote.Name == cross.Name)
            {
                await ReplyAsync("Delete aborted");
                return;
            }

            if(reaction.Emote.Name == checkmark.Name)
            {
                var response = CharacterService.DeleteCharacter(Context.Message.Author.Id, name, level);

                if (response.Success)
                {
                    await ReplyAsync("Delete Completed");
                }
                else
                {
                    await ReplyAsync(response.Error);
                }

            }

            await Task.CompletedTask;
        }
    }
}

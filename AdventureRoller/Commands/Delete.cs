using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using AdventureRoller.Services;
using System;
using Discord.WebSocket;
using System.Linq;

namespace AdventureRoller.Commands
{
    public class Delete : ModuleBase<SocketCommandContext>
    {
        private static string checkmark = "✓";
        private static string cross = "✗";

        private ICharacterService CharacterService { get; }

        public Delete(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("deletelevel")]
        public async Task Setup(string name, int level)
        {
            var dialog = await ReplyAsync($"{Context.Message.Author.Mention}, are you sure you want to delete {name} at level {level}?");
            await dialog.AddReactionAsync(new Emoji(checkmark));
            await dialog.AddReactionAsync(new Emoji(cross));
            Context.Client.ReactionAdded += new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>(
                (Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel message, SocketReaction reaction) => {
                    return Client_ReactionAdded(cache, message, reaction, name, level);
                });
        }

        [Command("deletecharacter")]
        public async Task Setup(string name)
        {
            var dialog = await ReplyAsync($"{Context.Message.Author.Mention}, are you sure you want to delete {name} at all levels?");
            await dialog.AddReactionAsync(new Emoji(checkmark));
            await dialog.AddReactionAsync(new Emoji(cross));
            Context.Client.ReactionAdded += new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>(
                (Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel message, SocketReaction reaction) => {
                    return Client_ReactionAdded(cache, message, reaction, name);
                });
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel message, SocketReaction reaction, string name, int? level = null)
        {
            if (cache.Value.MentionedUserIds.FirstOrDefault() != reaction.User.Value.Id)
            {
                return;
            }

            if (reaction.Emote.Name == cross)
            {
                await ReplyAsync("Delete aborted");
                return;
            }

            if(reaction.Emote.Name == checkmark)
            {
                CharacterService.DeleteCharacter(cache.Value.MentionedUserIds.FirstOrDefault(), name, level);
            }

            await Task.CompletedTask;
        }
    }
}

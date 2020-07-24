using AdventureRoller.DatabaseContext;
using AdventureRoller.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;


namespace AdventureRoller
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private static string prefix = "!";

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddDbContext<AdventurerollerdbContext>(options => 
                options.UseSqlServer(
                    "Server=localhost\\SQLEXPRESS;Database=AdventureRollerDB; User Id=AdventureRollProgram; Password=Test123;", 
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                    }
                ))
                .AddTransient<IEditionsService, FileEditionsService>()
                .AddTransient<ICharacterAttributeService, CharacterAttributeService>()
                .AddTransient<ICharacterService, CharacterService>()
                .AddTransient<IDiceService, DiceService>()
                .BuildServiceProvider();
            string botToken = "NzIxNDU0MjExNTk4MTIzMDI5.XuUw3A.czWB2bA8J-Nm_ErKGZWUQPijUAk";

            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }


        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleComamndAsync;

            Assembly assembly = Assembly.GetEntryAssembly();

            await _commands.AddModulesAsync(assembly, _services);
        }

        private async Task HandleComamndAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message == null || message.Author.IsBot) return;

            int argPosition = 0;
            if (message.HasStringPrefix(prefix, ref argPosition) || message.HasMentionPrefix(_client.CurrentUser, ref argPosition))
            {
                SocketCommandContext context = new SocketCommandContext(_client, message);

                await _commands.ExecuteAsync(context, argPosition, _services);

                await Task.CompletedTask;
            }
        }
    }
}

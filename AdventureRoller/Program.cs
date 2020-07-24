using AdventureRoller.DatabaseContext;
using AdventureRoller.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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
        private IConfiguration _configuration;

        private static string prefix = "!";

        public async Task RunBotAsync()
        {
            var environment = string.Empty;
            #if DEBUG
                environment = ".development";
            #endif

            var configurationbuilder =  new ConfigurationBuilder().AddJsonFile($"appsettings{environment}.json", optional: false, reloadOnChange: true);
            _configuration = configurationbuilder.Build();
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddDbContext<AdventurerollerdbContext>(options => 
                options.UseSqlServer(_configuration.GetValue<string>("DefaultConnection")
                    , 
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
            string botToken = _configuration.GetValue<string>("BotToken");

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

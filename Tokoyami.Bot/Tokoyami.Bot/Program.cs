using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tokoyami.Bot.Services;

namespace Tokoyami.Bot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private IServiceProvider _services;

        private readonly CommandService _cmdService;
        private readonly LogService _logService;
        private readonly ConfigService _configService;

        private readonly Config _config;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public Program()
        {
             _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
             });

            _cmdService = new CommandService(new CommandServiceConfig { 
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });

            _logService = new LogService();
            _configService = new ConfigService();
            _config = _configService.GetConfig();
        }

        public async Task RunBotAsync()
        {
            await _client.SetGameAsync("?help to see the commands");
            await _client.LoginAsync(TokenType.Bot, _config.Token);

            await _client.StartAsync();

            _client.Log += LogAsync;
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_cmdService)
                .BuildServiceProvider();

            var cmdHandler = new CommandHandler(_client, _cmdService, _services);
            await cmdHandler.InitalizeAsync();

            await Task.Delay(-1);
        }

        private async Task LogAsync(LogMessage msg) => await _logService.LogAsync(msg);
    }
}

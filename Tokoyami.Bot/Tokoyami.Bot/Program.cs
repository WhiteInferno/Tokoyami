﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tokoyami.Bot.Services;
using Tokoyami.Context;
using Tokoyami.Context.Configuration;

namespace Tokoyami.Bot
{
    class Program
    {
        static public IConfigurationRoot Configuration { get; set; }
        static public IServiceProvider Service { get; set; }

        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly ILogServices _logService;
        private readonly IConfigServices _config;

        static void Main(string[] args)
        {
            Configuration = AppConfiguration.Get(ContentDirectoryFinder.CalculateContentRootFolder());

            Service = ConfigureServices().BuildServiceProvider();

            Service.GetService<Program>().RunBotAsync().GetAwaiter().GetResult();
        }

        public static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug
            }));

            services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            }));

            services.AddSingleton<ILogServices, LogService>();

            services.AddSingleton<IConfigServices, ConfigService>();

            services.AddDbContext<TokoyamiDbContext>(opt => opt.UseSqlServer(Configuration["ConnectionString"]));

            services.AddTransient<Program>();

            return services;
        }

        public Program(DiscordSocketClient discordClient, IConfigServices configServices, CommandService cmdService, ILogServices logService)
        {
            this._client = discordClient;
            this._config = configServices;
            this._cmdService = cmdService;
            this._logService = logService;
        }

        public async Task RunBotAsync()
        {
            await _client.SetGameAsync("::help to see the commands");
            await _client.LoginAsync(TokenType.Bot, _config.GetConfig().Token);

            await _client.StartAsync();

            _client.Log += LogAsync;

            await InitializeHandlers();

            await Task.Delay(-1);
        }

        private async Task InitializeHandlers()
        {
            var cmdHandler = new CommandHandler(_client, _cmdService, Service);
            await cmdHandler.InitalizeAsync();
            var reactHandle = new ReactionHandler(_client, _cmdService, Service);
            await reactHandle.InitalizeAsync();
        }

        private async Task LogAsync(LogMessage msg) => await _logService.LogAsync(msg);
    }
}

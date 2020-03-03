using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tokoyami.Bot.Common;
using Tokoyami.Bot.Handlers;
using Tokoyami.Bot.Services;
using Tokoyami.Context;
using Tokoyami.Context.Configuration;
using Tokoyami.EF.Hangman.Entities;
using Victoria;

namespace Tokoyami.Bot
{
    class Program
    {
        static public IConfigurationRoot Configuration { get; set; }
        static public IServiceProvider Service { get; set; }

        private readonly DiscordSocketClient _client;
        private readonly LavaNode _lavaNode;
        private readonly CommandService _cmdService;
        private readonly ILogServices _logService;
        private readonly IConfigServices _config;
        private readonly UnitOfWork _unitOfWork;

        public static UnitOfWork UnitOfWork;

        //Hangman
        public static HangmanState HangmanState = HangmanState.STOPPED;
        public static Word curWord = null;
        public static List<char> letters = new List<char>();
        public static List<char> foundLetters = new List<char>();
        public static List<char> wrongLetters = new List<char>();
        public static int errors = 0;
        public static Boolean Found = false;
        public static string tempword = null;
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

            services.AddSingleton(new LavaConfig() {
                SelfDeaf = false
            });

            services.AddSingleton<LavaNode>();

            services.AddScoped<UnitOfWork>();

            services.AddDbContext<TokoyamiDbContext>(opt => opt.UseSqlServer(Configuration["ConnectionStrings:Default"]));//ConnectionString

            services.AddTransient<Program>();

            return services;
        }

        public Program(DiscordSocketClient discordClient,
            IConfigServices configServices,
            CommandService cmdService,
            ILogServices logService,
            UnitOfWork unitOfWork,
            LavaNode lavaNode)
        {
            this._client = discordClient;
            this._config = configServices;
            this._cmdService = cmdService;
            this._logService = logService;
            this._unitOfWork = unitOfWork;
            UnitOfWork = _unitOfWork;
            this._lavaNode = lavaNode;
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
            var cmdHandler = new CommandHandler(_client, _cmdService, Service, _logService);
            await cmdHandler.InitalizeAsync();
            var reactHandle = new ReactionHandler(_client, _cmdService, Service, _logService);
            await reactHandle.InitalizeAsync();
            var musicHandler = new MusicHandler(_client,_lavaNode, _logService);
            await musicHandler.InitalizeAsync();
        }

        private async Task LogAsync(LogMessage msg) => await _logService.LogAsync(msg);
    }
}

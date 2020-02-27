using System;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System.Reflection;
using Tokoyami.Context.Configuration;
using Tokoyami.Business.Business;

namespace Tokoyami.Bot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;
        private readonly UnitOfWork _unitOfWork;

        private HangmanService HangmanService { get; set; }

        public CommandHandler(DiscordSocketClient client
            , CommandService cmdService
            , IServiceProvider services
            , UnitOfWork unitOfWork)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
            _unitOfWork = unitOfWork;
        }

        public async Task InitalizeAsync()
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _cmdService.Log += LogAsync;
            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;

            var userMessage = socketMessage as SocketUserMessage;

            if (userMessage is null || socketMessage.Author.IsBot) return;

            if (userMessage.HasStringPrefix("::", ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, userMessage);
                await _cmdService.ExecuteAsync(context, argPos, _services);
            }
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}

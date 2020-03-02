using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tokoyami.Bot.Handlers
{
    public class MusicHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public MusicHandler(DiscordSocketClient client,
            CommandService cmdService,
            IServiceProvider services)
        {
            this._client = client;
            this._cmdService = cmdService;
            this._services = services;
        }

        public async Task InitalizeAsync()
        {
            await this._cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            this._cmdService.Log += LogAsync;
            this._client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;

            var userMessage = socketMessage as SocketUserMessage;

            if (userMessage is null || socketMessage.Author.IsBot) return;

            if (userMessage.HasStringPrefix("m!", ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
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

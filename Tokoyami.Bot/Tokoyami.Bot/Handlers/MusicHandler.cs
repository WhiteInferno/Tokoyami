using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.Bot.Services;
using Victoria;

namespace Tokoyami.Bot.Handlers
{
    public class MusicHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly LavaNode _lavaNode;
        private readonly ILogServices _logService;

        public MusicHandler(DiscordSocketClient client, LavaNode lavaNode, ILogServices logService)
        {
            this._client = client;
            this._lavaNode = lavaNode;
            this._logService = logService;
        }

        public Task InitalizeAsync()
        {
            this._client.Ready += ClientReadyAsync;
            this._lavaNode.OnLog += LogAsync;
            return Task.CompletedTask;
        }

        private async Task ClientReadyAsync()
        {
            await _lavaNode.ConnectAsync();
        }

        private async Task LogAsync(LogMessage msg) => await _logService.LogAsync(msg);

    }
}

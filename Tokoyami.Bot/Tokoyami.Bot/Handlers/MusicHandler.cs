using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.Bot.Services;
using Victoria;
using Victoria.EventArgs;

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
            this._client.Ready += OnReady;
            this._lavaNode.OnLog += LogAsync;
            this._lavaNode.OnPlayerUpdated += OnPlayerUpdated;
            this._lavaNode.OnStatsReceived += OnStatsReceived;
            this._lavaNode.OnTrackEnded += OnTrackEnded;
            this._lavaNode.OnTrackException += OnTrackException;
            this._lavaNode.OnTrackStuck += OnTrackStuck;
            this._lavaNode.OnWebSocketClosed += OnWebSocketClosed;
            return Task.CompletedTask;
        }

        private Task OnPlayerUpdated(PlayerUpdateEventArgs arg) 
            => this.LogAsync(new LogMessage(LogSeverity.Info, "Victoria", $"Player update received for {arg.Player.VoiceChannel.Name}.", null));

        private Task OnStatsReceived(StatsEventArgs arg)
            => this.LogAsync(new LogMessage(LogSeverity.Info, "Victoria", $"Lavalink Uptime {arg.Uptime}.", null));

        private async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            if (!args.Reason.ShouldPlayNext())
                return;

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable))
            {
                await player.TextChannel.SendMessageAsync("No more tracks to play.");
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync(
                $"{args.Reason}: {args.Track.Title}\nNow playing: {track.Title}");
        }

        private Task OnTrackException(TrackExceptionEventArgs arg)
            => this.LogAsync(new LogMessage(LogSeverity.Critical, "Victoria", $"Track exception received for {arg.Track.Title}.", null));

        private Task OnTrackStuck(TrackStuckEventArgs arg) 
            => this.LogAsync(new LogMessage(LogSeverity.Error, "Victoria", $"Track stuck received for {arg.Track.Title}.", null));

        private Task OnWebSocketClosed(WebSocketClosedEventArgs arg)
            => this.LogAsync(new LogMessage(LogSeverity.Critical, "Victoria", $"Discord WebSocket connection closed with following reason: {arg.Reason}", null));

        private async Task OnReady() => await _lavaNode.ConnectAsync();

        private async Task LogAsync(LogMessage msg) => await _logService.LogAsync(msg);

    }
}

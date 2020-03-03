﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.Bot.Services;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Rest;

namespace Tokoyami.Bot.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);
        private readonly ILogServices _logService;

        public Music(LavaNode lavaNode, ILogServices logService)
        {
            this._lavaNode = lavaNode;
            this._logService = logService;
        }

        [Command("join")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice Channel");
                return;
            }

            var voiceState = Context.User as IVoiceState;

            if(voiceState!.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            }
            catch(Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't join to the VC!");
                return;
            }
        }

        [Command("play")]
        public async Task PlayAsync([Remainder] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            SearchResponse searchResponse;
            
            try
            {
                searchResponse = await _lavaNode.SearchAsync(query);
            }
            catch(Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't play the music!");
                return;
            }

            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    foreach (var track in searchResponse.Tracks)
                    {
                        player.Queue.Enqueue(track);
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    var track = searchResponse.Tracks[0];
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
            }
            else
            {
                var track = searchResponse.Tracks[0];

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++)
                    {
                        if (i == 0)
                        {
                            await player.PlayAsync(track);
                            await ReplyAsync($"Now Playing: {track.Title}");
                        }
                        else
                        {
                            player.Queue.Enqueue(searchResponse.Tracks[i]);
                        }
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    try
                    {
                        await player.PlayAsync(track);
                        await ReplyAsync($"Now Playing: {track.Title}");
                    }
                    catch(Exception ex)
                    {
                        await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                        await ReplyAsync($"{track.Title} couldn't be played!");
                        return;
                    }
                }
            }
        }

        [Command("Leave")]
        public async Task LeaveAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to any voice channels!");
                return;
            }

            var voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null)
            {
                await ReplyAsync("Not sure which voice channel to disconnect from.");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                await ReplyAsync($"I've left {voiceChannel.Name}!");
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't leave the VC!");
            }
        }

        [Command("Pause")]
        public async Task PauseAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("I cannot pause when I'm not playing anything!");
                return;
            }

            try
            {
                await player.PauseAsync();
                await ReplyAsync($"Paused: {player.Track.Title}");
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't pause the music!");
            }
        }

        [Command("Resume")]
        public async Task ResumeAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Paused)
            {
                await ReplyAsync("I cannot resume when I'm not playing anything!");
                return;
            }

            try
            {
                await player.ResumeAsync();
                await ReplyAsync($"Resumed: {player.Track.Title}");
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't resume the music!");
            }
        }

        [Command("Stop")]
        public async Task StopAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Woaaah there, I can't stop the stopped forced.");
                return;
            }

            try
            {
                await player.StopAsync();
                await ReplyAsync("No longer playing anything.");
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't stop the music!");
            }
        }

        [Command("Skip")]
        public async Task SkipAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I can't skip when nothing is playing.");
                return;
            }

            var voiceChannelUsers = (player.VoiceChannel as SocketVoiceChannel).Users.Where(x => !x.IsBot).ToArray();

            try
            {
                var oldTrack = player.Track;
                var currenTrack = await player.SkipAsync();
                await ReplyAsync($"Skipped: {oldTrack.Title}\nNow Playing: {currenTrack.Title}");
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't skip the music!");
            }
        }

        [Command("Seek")]
        public async Task SeekAsync(TimeSpan? timeSpan = null)
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I can't seek when nothing is playing.");
                return;
            }

            if(timeSpan == null)
            {
                await ReplyAsync("You have to send me the time with the follow format `0`h`00`m`00`s");
                return;
            }

            try
            {
                await player.SeekAsync(timeSpan);
                await ReplyAsync($"I've seeked `{player.Track.Title}` to {timeSpan}.");
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't seek the music!");
            }
        }

        [Command("Volume")]
        public async Task VolumeAsync(ushort volume)
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            try
            {
                if(volume >= 0 && volume <= 100)
                {
                    await player.UpdateVolumeAsync(volume);
                    await ReplyAsync($"I've changed the player volume to {volume}.");
                }
                else
                {
                    await ReplyAsync("Set the volume in a value between 0 to 100.");
                }
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, "Music Module", ex.Message));
                await ReplyAsync("Something happened that i can't set the volume!");
            }
        }

        [Command("NowPlaying"), Alias("Np")]
        public async Task NowPlayingAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var track = player.Track;
            var artwork = await track.FetchArtworkAsync();

            var embed = new EmbedBuilder
            {
                Title = $"{track.Author} - {track.Title}",
                ThumbnailUrl = artwork,
                Url = track.Url
            }
                .AddField("Id", track.Id)
                .AddField("Duration", track.Duration)
                .AddField("Position", track.Position);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Genius", RunMode = RunMode.Async)]
        public async Task ShowGeniusLyrics() //Nani?
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var lyrics = await player.Track.FetchLyricsFromGeniusAsync();
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync($"No lyrics found for {player.Track.Title}");
                return;
            }

            var splitLyrics = lyrics.Split('\n');
            var stringBuilder = new StringBuilder();
            foreach (var line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }
        private Task LogAsync(LogMessage msg) => _logService.LogAsync(msg);
    }
}

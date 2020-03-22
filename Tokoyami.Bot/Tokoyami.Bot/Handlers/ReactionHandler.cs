using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Tokoyami.Bot.Modules;
using Tokoyami.Bot.Services;

namespace Tokoyami.Bot
{
    public class ReactionHandler
    {
        private DiscordSocketClient Client { get; }
        private CommandService CdmService { get; }
        private IServiceProvider Service { get; }
        private ILogServices LogService { get; }

        private string[] wordsToReact = new string[] { "banana", "apple", "orange", "birthday" };

        public ReactionHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider service, ILogServices logService)
        {
            this.Client = client;
            this.CdmService = cmdService;
            this.Service = service;
            this.LogService = logService;
        }

        public async Task InitalizeAsync()
        {
            await this.CdmService.AddModulesAsync(Assembly.GetEntryAssembly(), Service);

            this.CdmService.Log += LogAsync;
            this.Client.MessageReceived += AnalyzeMessage;
        }

        public async Task AnalyzeMessage(SocketMessage socketMessage)
        {
            var userMessage = socketMessage as SocketUserMessage;

            if (userMessage is null || socketMessage.Author.IsBot) return;

            foreach (string word in wordsToReact)
            {
                if (userMessage.Content.ToUpper().Contains(word.ToUpper()))
                {
                    await ReactMessage(socketMessage, word.ToUpper());
                    return;
                }
            }
        }

        private async Task ReactMessage(SocketMessage socketMessage, string reaction)
        {
            var mesagge = (RestUserMessage)await socketMessage.Channel.GetMessageAsync(socketMessage.Id);
            Emoji emoji;
            switch (reaction)
            {
                case ("BANANA"):
                    emoji = new Emoji(Emojis.BANANA_EMOJI);
                    break;
                case ("APPLE"):
                    emoji = new Emoji(Emojis.APPLE_EMOJI);
                    break;
                case ("ORANGE"):
                    emoji = new Emoji(Emojis.ORANGE_EMOJI);
                    break;
                case ("BIRTHDAY"):
                    emoji = new Emoji(Emojis.BIRTHDAY_EMOJI);
                    break;
                default:
                    emoji = new Emoji(string.Empty);
                    break;
            }
            await mesagge.AddReactionAsync(emoji);
        }

        private async Task LogAsync(LogMessage msg) => await LogService.LogAsync(msg);
    }
}

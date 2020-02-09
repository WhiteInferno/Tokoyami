using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;

namespace Tokoyami.Bot.Modules
{
    public class Common : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        public async Task Pong()
        {
            await ReplyAsync("PONG!");
        }

        [Command("Hi")]
        public async Task Hi()
        {
            var userMention = this.Context.Message.Author.Mention;
            await ReplyAsync($"Hi {userMention}!");
        }
    }
}

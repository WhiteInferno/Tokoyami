using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tokoyami.Bot.Services
{
    public interface ILogServices
    {
        Task LogAsync(LogMessage arg);
    }

    public class LogService : ILogServices
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public LogService()
        {
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task LogAsync(LogMessage arg)
        {
            await _semaphoreSlim.WaitAsync();

            var timeStamp = DateTimeOffset.Now.ToString("dd/MM/yyyy hh:mm tt");
            const string format = "{0,-10} {1,10}";

            Console.WriteLine($"[{timeStamp}] {string.Format(format, arg.Source, $":{arg.Message}")}");

            _semaphoreSlim.Release();
        }
    }
}

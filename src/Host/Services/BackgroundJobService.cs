using Host.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Host.Services
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly IHubContext<JobProgressHub> _hubContext;
        private readonly Random _random;
        private readonly PeriodicTimer _timer;
        private readonly IMemoryCache _memoryCache;

        public BackgroundJobService(IHubContext<JobProgressHub> hub, IMemoryCache memoryCache)
        {
            _hubContext = hub;
            _random = new Random();
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await SendToHubWorkAsync(stoppingToken);
            }
        }

        public async Task SendToHubWorkAsync(CancellationToken stoppingToken)
        {
            var encodedCharacters = _memoryCache.Get<List<char>>("EncodedCharacters");
            if (encodedCharacters != null)
            {
                foreach (var encodedCharacter in encodedCharacters)
                {
                    if (stoppingToken.IsCancellationRequested || _memoryCache.Get<bool>("CancelOperation"))
                    {
                        break;
                    }

                    await _hubContext.Clients.All.SendAsync("ReceiveEncodedCharacter", encodedCharacter, stoppingToken);
                    await RandomPause(stoppingToken);
                }

                _memoryCache.Remove("EncodedCharacters");
            }
        }

        private async Task RandomPause(CancellationToken stoppingToken)
        {
            var pauseTime = _random.Next(1000, 5000);
            await Task.Delay(pauseTime, stoppingToken);
        }
    }
}

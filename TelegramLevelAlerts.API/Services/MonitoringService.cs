using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramLevelAlerts.API.Models;

namespace TelegramLevelAlerts.API.Services
{
    public class MonitoringService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private Task _executingTask;
        private readonly ConfigurationSettings _configurationSettings;
        private readonly AlertService _alertService;

        public MonitoringService(ConfigurationSettings configurationSettings, AlertService alertService)
        {
            _configurationSettings = configurationSettings;
            _alertService = alertService;
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(cts.Token);

            if (_executingTask.IsCompleted)            
                return _executingTask;            

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
                return;
            
            try
            {
                cts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            var bot = new TelegramBotClient(_configurationSettings.TelegramBotToken);

            while (!token.IsCancellationRequested)
            {
                //Get all messages
                var messagesToNotify = _alertService.GetAlertsToNotify();

                Parallel.ForEach(messagesToNotify,
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    message =>
                    {

                    });
                
                await Task.Delay(TimeSpan.FromSeconds(10), token);
            }
        }
    }
}

using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramLevelAlerts.API.Services
{
    public class MonitoringService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private Task _executingTask;

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
            while (!token.IsCancellationRequested)
            {
                
                
                await Task.Delay(TimeSpan.FromSeconds(10), token);
            }
        }
    }
}

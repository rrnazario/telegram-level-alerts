using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramLevelAlerts.API.Models;

namespace TelegramLevelAlerts.API.Services
{
    public class BotService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private Task _executingTask;
        private readonly ConfigurationSettings _configurationSettings;
        private readonly TelegramBotClient _bot;
        private readonly AlertService _alertService;

        public BotService(ConfigurationSettings configurationSettings, AlertService alertService)
        {
            _configurationSettings = configurationSettings;

            _bot = new TelegramBotClient(_configurationSettings.TelegramBotToken);
            _alertService = alertService;
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _bot.OnUpdate += botUpdate;
            _bot.StartReceiving();

            _executingTask = ExecuteAsync(cts.Token);

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        private void botUpdate(object sender, UpdateEventArgs e)
        {
            try
            {
                var channelPost = e.Update.Message.Text;

                if (channelPost.StartsWith("/channel", StringComparison.InvariantCulture)) //Add chanel
                {
                    // /channel ALERT_ID CHANNEL_INDEX: /channel 000-000-00000-000 0
                    var groupConfigInfo = channelPost.Split(" ");

                    if (groupConfigInfo.Length < 3)
                        throw new ArgumentException("Unrecognized format");

                    var alertId = groupConfigInfo[1];
                    var levelSeverity = int.Parse(groupConfigInfo[2]);

                    var alert = _alertService.GetById(alertId).GetAwaiter().GetResult();

                    if (alert == null)
                        throw new KeyNotFoundException("Alert not found");

                    var level = alert.Levels.FirstOrDefault(f => f.Severity == levelSeverity);

                    if (level == null)
                        throw new KeyNotFoundException("Alert level not found.");

                    level.Id = e.Update.Message.Chat.Id.ToString();

                    _alertService.UpdateAsync(alertId, alert).GetAwaiter().GetResult();
                }
            }
            catch { }
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

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

                if (channelPost.StartsWith("/channel", StringComparison.InvariantCulture))
                {
                    RegisterAlert(e);
                    return;
                }

                if (channelPost.StartsWith("/id", StringComparison.InvariantCulture))
                {
                    ShowChannelId(e);
                    return;
                }
            }
            catch { }
        }

        private void ShowChannelId(UpdateEventArgs e)
        {
            _bot.SendTextMessageAsync(e.Update.Message.Chat.Id, $"The channel Id is: {e.Update.Message.Chat.Id}");
        }

        private void RegisterAlert(UpdateEventArgs e)
        {
            // /channel ALERT_ID CHANNEL_INDEX: /channel 000-000-00000-000 0
            var groupConfigInfo = e.Update.Message.Text.Split(" ");

            if (groupConfigInfo.Length < 3)
            {
                _bot.SendTextMessageAsync(e.Update.Message.Chat.Id, $"The channel could not be configured. The data format is invalid.");
                return;
            }

            var alertId = groupConfigInfo[1];
            var levelSeverity = int.Parse(groupConfigInfo[2]);

            var alert = _alertService.GetById(alertId).GetAwaiter().GetResult();

            if (alert == null)
                throw new KeyNotFoundException("Alert not found");

            var level = alert.Levels.FirstOrDefault(f => f.Severity == levelSeverity);

            if (level == null)
            {
                _bot.SendTextMessageAsync(e.Update.Message.Chat.Id, $"The channel could not be configured. Alert level not found.");
                return;
            }

            level.Id = e.Update.Message.Chat.Id.ToString();

            _alertService.UpdateAsync(alertId, alert).GetAwaiter().GetResult();

            _bot.SendTextMessageAsync(e.Update.Message.Chat.Id, $"Channel well configured as level {level.Severity} of alert {alertId}.");
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

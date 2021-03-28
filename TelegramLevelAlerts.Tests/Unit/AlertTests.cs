using System;
using System.Linq;
using TelegramLevelAlerts.API.Models;
using Xunit;

namespace TelegramLevelAlerts.Tests.Unit
{
    public class AlertTests
    {
        private const string Token = @"1714073653:AAGf_uCPu4m1x6wuWofPdtkNOkIqLHqMWuI";
        
        [Fact]
        public void CreateAlert()
        {
            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                Message = Guid.NewGuid().ToString(),
                Often = 1,
                TimeFormat = TimeFormat.Minutes,
                TimesPerLevel = 3,
                Levels = new System.Collections.Generic.List<GroupLevel>
                {
                    new GroupLevel
                    {
                        Name = "Peoes",
                        Severity = 1
                    },
                    new GroupLevel
                    {
                        Name = "Patrões",
                        Severity = 2
                    }
                }
            };

            alert.Notify(new Telegram.Bot.TelegramBotClient(Token));
        }
    }
}

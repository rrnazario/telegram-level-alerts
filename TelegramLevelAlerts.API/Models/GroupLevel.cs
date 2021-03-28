using System;
using Telegram.Bot;

namespace TelegramLevelAlerts.API.Models
{
    public class GroupLevel
    {
        public int Severity { get; set; } = 1;
        public string Name { get; set; }
        public string Id { private get; set; }
        public int AlertedCount { get; private set; } = 0;

        internal bool Notify(TelegramBotClient bot, string message)
        {
            try
            {
                bot.SendTextMessageAsync(Id, message, Telegram.Bot.Types.Enums.ParseMode.Markdown).GetAwaiter().GetResult();

                AlertedCount++;

                return true;
            }
            catch { }

            return false;
        }
    }
}

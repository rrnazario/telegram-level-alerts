using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace TelegramLevelAlerts.API.Models
{
    public class Alert
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime LastAlertedTime { get; private set; } = DateTime.Now;
        public int Often { get; set; } = 1;
        public TimeFormat TimeFormat { get; set; } = TimeFormat.Minutes;
        /// <summary>
        /// Times to start elevating alert
        /// </summary>
        public int TimesPerLevel { get; set; } = 3;

        private int currentSeverity;

        private DateTime NextTimeToAlert
        {
            get
            {
                return TimeFormat switch
                {
                    var x when x.Equals(TimeFormat.Hours) => LastAlertedTime.AddHours(Often),
                    var x when x.Equals(TimeFormat.Minutes) => LastAlertedTime.AddMinutes(Often),
                    var x when x.Equals(TimeFormat.Seconds) => LastAlertedTime.AddSeconds(Often),
                    var x when x.Equals(TimeFormat.Days) => LastAlertedTime.AddDays(Often),
                    _ => throw new Exception("Time format not defined.")
                };
            }
        }
        public List<GroupLevel> Levels { get; set; }
        public bool Alerting { get; set; } = true;

        public Alert() { }

        public Alert(Alert alertToClone)
        {
            Message = alertToClone.Message;
            LastAlertedTime = alertToClone.LastAlertedTime;
            Often = alertToClone.Often;
            TimeFormat = alertToClone.TimeFormat;
            TimesPerLevel = alertToClone.TimesPerLevel;
            Levels = alertToClone.Levels;
            Alerting = alertToClone.Alerting;
        }

        public void Notify(TelegramBotClient bot)
        {
            if (Alerting && NextTimeToAlert < LastAlertedTime)
                return;

            LastAlertedTime = DateTime.Now;

            foreach (var level in Levels.OrderBy(o => o.Severity))
            {
                var notified = level.Notify(bot, GetMessage());

                if (!notified) break;
                
                if (level.AlertedCount < TimesPerLevel)
                {
                    currentSeverity = level.Severity;
                    break;
                }
            }
        }

        public override string ToString() => JsonConvert.SerializeObject(this);

        #region Private Methods
        string GetMessage()
        {
            string msg = "";

            if (!string.IsNullOrEmpty(Message))
                msg = $"*Message:* _{Message}_";

            msg += $"\n\n*Severity: *{currentSeverity}";

            return msg;
        }
        #endregion
    }
}
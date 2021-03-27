using System;
using System.Collections.Generic;

namespace TelegramLevelAlerts.API.Models
{
    public class Alert
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime LastAlertedTime { get; set; } = DateTime.Now;
        public int Often { get; set; } = 1;
        public TimeFormat TimeFormat { get; set; } = TimeFormat.Minutes;
        public int TimesPerLevel { get; set; } = 3;
        public List<GroupLevel> Levels { get; set; }

        public override string ToString() => GetMessage();

        #region Private Methods
        string GetMessage()
        {
            string msg = "";

            if (!string.IsNullOrEmpty(Message))
                msg = $"*Mensagem:* _{Message}_";
            
            return msg;
        }
        #endregion
    }
}
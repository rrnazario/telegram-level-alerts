using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramLevelAlerts.API.Models;

namespace TelegramLevelAlerts.API.Data
{
    public class AlertData
    {
        public static List<Alert> alerts = new();

        internal Task InsertAlertAsync(Alert alert)
        {
            var foundAlert = alerts.Find(f => f.Id == alert.Id);

            if (foundAlert != null)
                throw new ArgumentException("Alerta já inserido.");

            alerts.Add(alert);

            return Task.CompletedTask;
        }

        internal Task UpdateAsync(string id, Alert alert)
        {
            var foundAlert = alerts.Find(f => f.Id.ToString() == id);

            if (foundAlert == null)
                throw new ArgumentNullException("Alerta não inserido.");

            alerts.Remove(foundAlert);
            alerts.Add(alert);

            return Task.CompletedTask;
        }

        internal Task<IEnumerable<Alert>> GetAllAlertsAsync()
        {
            return Task.FromResult<IEnumerable<Alert>>(alerts);
        }
    }
}

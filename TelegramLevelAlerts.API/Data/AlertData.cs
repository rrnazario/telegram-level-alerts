using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramLevelAlerts.API.Models;

namespace TelegramLevelAlerts.API.Data
{
    public class AlertData
    {
        public static List<Alert> alerts = new();

        internal Task<Guid> InsertAlertAsync(Alert alert)
        {
            if (alert.Id == default)
                alert.Id = Guid.NewGuid();

            alerts.Add(alert);

            return Task.FromResult(alert.Id);
        }

        internal async Task UpdateAsync(string id, Alert alert) => await UpdateEntity(id, alert);

        internal async Task<Alert> GetById(string id) => await Task.FromResult(alerts.Find(f => f.Id.ToString() == id));

        private void UpdateEntity(Alert beforeAlert, Alert nowAlert)
        {
            alerts.Remove(beforeAlert);
            alerts.Add(nowAlert);
        }

        private async Task UpdateEntity(string id, Alert nowAlert)
        {
            var foundAlert = await GetById(id);
            UpdateEntity(foundAlert, nowAlert);
        }

        internal IEnumerable<Alert> GetAlertsToNotify() => alerts.Where(w => w.Alerting);

        internal Task<IEnumerable<Alert>> GetAllAlertsAsync()
        {
            return Task.FromResult<IEnumerable<Alert>>(alerts);
        }
    }
}

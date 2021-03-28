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
            var foundAlert = alerts.Find(f => f.Id == alert.Id);

            if (alert.Id != default && foundAlert != null)
                throw new ArgumentException("Alert already inserted.");

            if (alert.Id == default)
                alert.Id = Guid.NewGuid();

            alerts.Add(alert);

            return Task.FromResult(alert.Id);
        }

        internal Task UpdateAsync(string id, Alert alert)
        {
            var foundAlert = alerts.Find(f => f.Id.ToString() == id);

            if (foundAlert == null)
                throw new KeyNotFoundException("Alert not found.");

            UpdateEntity(foundAlert, alert);

            return Task.CompletedTask;
        }

        internal async Task<Alert> GetById(string id) => await Task.FromResult(alerts.Find(f => f.Id.ToString() == id));

        private void UpdateEntity(Alert beforeAlert, Alert nowAlert)
        {
            alerts.Remove(beforeAlert);
            alerts.Add(nowAlert);
        }

        internal Task StopAsync(string id)
        {
            var foundAlert = alerts.Find(f => f.Id.ToString() == id);

            if (foundAlert == null)
                throw new KeyNotFoundException("Alert not found.");

            var newAlert = new Alert(foundAlert);

            newAlert.Alerting = false;

            UpdateEntity(foundAlert, newAlert);

            return Task.CompletedTask;
        }

        internal IEnumerable<Alert> GetAlertsToNotify() => alerts.Where(w => w.Alerting);

        internal Task<IEnumerable<Alert>> GetAllAlertsAsync()
        {
            return Task.FromResult<IEnumerable<Alert>>(alerts);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramLevelAlerts.API.Data;
using TelegramLevelAlerts.API.Models;

namespace TelegramLevelAlerts.API.Services
{
    public class AlertService
    {
        private readonly AlertData _alertData;

        public AlertService(AlertData alertData)
        {
            _alertData = alertData;
        }

        internal async Task<Guid> RegisterAlertAsync(Alert alert) => await _alertData.InsertAlertAsync(alert);

        internal async Task UpdateAsync(string id, Alert alert) => await _alertData.UpdateAsync(id, alert);

        internal async Task<IEnumerable<Alert>> GetAllAlertsAsync() => await _alertData.GetAllAlertsAsync();

        internal IEnumerable<Alert> GetAlertsToNotify() => _alertData.GetAlertsToNotify();

        internal async Task<Alert> GetById(string id) => await _alertData.GetById(id);

        internal async Task StopAsync(string id) => await _alertData.StopAsync(id);
    }
}

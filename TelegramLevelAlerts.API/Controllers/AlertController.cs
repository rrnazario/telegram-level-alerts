using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TelegramLevelAlerts.API.Models;
using TelegramLevelAlerts.API.Services;

namespace TelegramLevelAlerts.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly AlertService _alertService;

        public AlertController(AlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var all = await _alertService.GetAllAlertsAsync();
            return Ok(all);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Alert alert)
        {
            await _alertService.RegisterAlertAsync(alert);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Alert alert)
        {
            await _alertService.UpdateAsync(id, alert);

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;

namespace SWD.API.Controllers
{
    [Route("api/alerts")]
    [ApiController]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>
        /// Get Alerts - With optional filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAlertsAsync(
            [FromQuery] string? status = "All",
            [FromQuery] string? search = null)
        {
            var alerts = await _alertService.GetAlertsWithFiltersAsync(status, search);

            var alertDtos = alerts.Select(h => new
            {
                id = h.HistoryId,
                time = h.TriggeredAt,
                sensor_name = h.Sensor?.Name ?? "Unknown",
                severity = h.Severity ?? "Warning",
                status = h.ResolvedAt == null ? "Active" : "Resolved"
            }).ToList();

            return Ok(alertDtos);
        }

        /// <summary>
        /// Resolve Alert
        /// </summary>
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveAlertAsync(int id)
        {
            var alert = await _alertService.GetAlertByIdAsync(id);
            if (alert == null)
                return NotFound(new { message = "Alert not found" });

            if (alert.ResolvedAt != null)
                return BadRequest(new { message = "Alert is already resolved" });

            await _alertService.ResolveAlertAsync(id);

            return Ok(new
            {
                message = "Alert resolved successfully",
                id = id,
                resolvedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Delete Alert Log
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> DeleteAlertAsync(int id)
        {
            var alert = await _alertService.GetAlertByIdAsync(id);
            if (alert == null)
                return NotFound(new { message = "Alert not found" });

            await _alertService.DeleteAlertAsync(id);

            return Ok(new { message = "Alert deleted successfully", id = id });
        }

        /// <summary>
        /// Get Alert Rules - For configuration
        /// </summary>
        [HttpGet("rules")]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> GetAllRulesAsync()
        {
            var rules = await _alertService.GetAllRulesAsync();

            var ruleDtos = rules.Select(r => new AlertRuleDto
            {
                RuleId = r.RuleId,
                SensorId = r.SensorId,
                SensorName = r.Sensor?.Name,
                Name = r.Name,
                ConditionType = r.ConditionType,
                MinVal = r.MinVal,
                MaxVal = r.MaxVal,
                NotificationMethod = r.NotificationMethod,
                Priority = r.Priority,
                IsActive = r.IsActive
            }).ToList();

            return Ok(ruleDtos);
        }

        /// <summary>
        /// Create Alert Rule
        /// </summary>
        [HttpPost("rules")]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> CreateRuleAsync([FromBody] CreateAlertRuleDto request)
        {
            var rule = new AlertRule
            {
                SensorId = request.SensorId,
                Name = request.Name,
                ConditionType = request.ConditionType,
                MinVal = request.MinVal,
                MaxVal = request.MaxVal,
                NotificationMethod = request.NotificationMethod,
                Priority = request.Priority,
                IsActive = true
            };

            await _alertService.CreateRuleAsync(rule);

            return Ok(new
            {
                message = "Alert rule created successfully",
                ruleId = rule.RuleId,
                sensorId = rule.SensorId
            });
        }

        /// <summary>
        /// Get Alert History - For historical analysis
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] int? sensorId = null)
        {
            var history = await _alertService.GetAlertHistoryAsync(sensorId);

            var historyDtos = history.Select(h => new AlertHistoryDto
            {
                HistoryId = h.HistoryId,
                RuleId = h.RuleId,
                RuleName = h.Rule?.Name,
                SensorId = h.SensorId,
                SensorName = h.Sensor?.Name,
                TriggeredAt = h.TriggeredAt,
                ResolvedAt = h.ResolvedAt,
                ValueAtTrigger = h.ValueAtTrigger,
                Severity = h.Severity,
                Message = h.Message
            }).ToList();

            return Ok(historyDtos);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;

namespace SWD.API.Controllers
{
    [Route("api/sensors")]  // ← ĐỔI từ "api/sensor"
    [ApiController]
    [Authorize]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        /// <summary>
        /// Get All Sensors - With optional filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSensorsAsync(
            [FromQuery] int? hub_id = null,
            [FromQuery] int? type = null)
        {
            List<Sensor> sensors;

            if (hub_id.HasValue)
            {
                sensors = await _sensorService.GetSensorsByHubIdAsync(hub_id.Value);
            }
            else if (type.HasValue)
            {
                sensors = await _sensorService.GetSensorsByTypeIdAsync(type.Value);
            }
            else
            {
                sensors = await _sensorService.GetAllSensorsAsync();
            }

            var sensorDtos = sensors.Select(s => new SensorDto
            {
                SensorId = s.SensorId,
                HubId = s.HubId,
                HubName = s.Hub?.Name,
                TypeId = s.TypeId,
                TypeName = s.Type?.TypeName,
                SensorName = s.Name,
                CurrentValue = s.CurrentValue,
                LastUpdate = s.LastUpdate,
                Status = s.Status
            }).ToList();

            return Ok(sensorDtos);
        }

        /// <summary>
        /// Register Sensor
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> RegisterSensorAsync([FromBody] RegisterSensorDto request)
        {
            var sensor = new Sensor
            {
                HubId = request.HubId,
                TypeId = request.TypeId,
                Name = request.Name,
                Status = "Active",
                CurrentValue = 0,
                LastUpdate = DateTime.UtcNow
            };

            await _sensorService.RegisterSensorAsync(sensor);

            return Ok(new
            {
                message = "Sensor registered successfully",
                sensor = new SensorDto
                {
                    SensorId = sensor.SensorId,
                    HubId = sensor.HubId,
                    HubName = null,
                    TypeId = sensor.TypeId,
                    TypeName = null,
                    SensorName = sensor.Name,
                    CurrentValue = sensor.CurrentValue,
                    LastUpdate = sensor.LastUpdate,
                    Status = sensor.Status
                }
            });
        }

        /// <summary>
        /// Get Sensor Readings - For chart display
        /// </summary>
        [HttpGet("{id}/readings")]
        public async Task<IActionResult> GetReadingsAsync(
            int id,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            try
            {
                DateTime fromDate;
                DateTime toDate;

                if (from.HasValue && to.HasValue)
                {
                    fromDate = from.Value.Date;
                    toDate = to.Value.Date.AddDays(1).AddTicks(-1);
                }
                else
                {
                    fromDate = DateTime.MinValue;
                    toDate = DateTime.MaxValue;
                }

                var readings = await _sensorService.GetSensorReadingsAsync(id, fromDate, toDate);

                var readingDtos = readings.Select(r => new ReadingDto
                {
                    ReadingId = r.ReadingId,
                    SensorId = r.SensorId,
                    SensorName = r.Sensor?.Name,
                    SensorTypeName = r.Sensor?.Type?.TypeName,
                    Value = r.Value,
                    RecordedAt = r.RecordedAt
                }).ToList();

                return Ok(readingDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to get readings", error = ex.Message });
            }
        }

        /// <summary>
        /// Receive Telemetry - IoT Gateway sends data here
        /// </summary>
        [HttpPost("telemetry")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveTelemetry(
            [FromQuery] int sensorId,
            [FromQuery] float value)
        {
            try
            {
                await _sensorService.ProcessReadingAsync(sensorId, value);
                return Ok(new
                {
                    message = "Telemetry received successfully",
                    sensorId = sensorId,
                    value = value,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to process telemetry",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get All Sensor Types - For dropdown
        /// </summary>
        [HttpGet("types")]
        public async Task<IActionResult> GetAllTypesAsync()
        {
            var types = await _sensorService.GetAllSensorTypesAsync();
            return Ok(types);
        }
    }
}
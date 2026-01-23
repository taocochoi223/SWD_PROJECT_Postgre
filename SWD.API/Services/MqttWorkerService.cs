using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;
using System.Text;
using System.Text.Json;

namespace SWD.API.Services
{
    public class MqttWorkerService : BackgroundService
    {
        private readonly ILogger<MqttWorkerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IMqttClient _mqttClient = null!;
        private MqttClientOptions _mqttOptions = null!;

        // Configurations 
        private const string Broker = "mqtt1.eoh.io";
        private const int Port = 1883;
        private const string GatewayToken = "fe6fa4a8-2c25-4e37-83f1-51c4990eb55e";
        private const string DeviceId = "18100709";
        private readonly string Topic = $"eoh/chip/{GatewayToken}/third_party/{DeviceId}/data";

        public MqttWorkerService(ILogger<MqttWorkerService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(Broker, Port)
                .WithCredentials(GatewayToken, GatewayToken)
                .WithCleanSession()
                .Build();

            _mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            _mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            _mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

            await ConnectToMqttAsync();

            await base.StartAsync(cancellationToken);
        }

        private async Task ConnectToMqttAsync()
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MQTT Broker. Retrying in 5 seconds...");
                await Task.Delay(5000);
                await ConnectToMqttAsync();
            }
        }

        private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            _logger.LogInformation("Connected to MQTT Broker.");
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(Topic))
                .Build();
            await _mqttClient.SubscribeAsync(subscribeOptions);
            _logger.LogInformation($"Subscribed to topic: {Topic}");
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            _logger.LogWarning("Disconnected from MQTT Broker. Attempting to reconnect...");
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                await ConnectToMqttAsync();
            });
            return Task.CompletedTask;
        }

        private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            // Fix for v5: Use PayloadSegment instead of Payload
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray()); 
            _logger.LogInformation($"Received Message: {payload}");

            using (var scope = _scopeFactory.CreateScope())
            {
                var sensorService = scope.ServiceProvider.GetRequiredService<ISensorService>();
                var hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
                var systemLogService = scope.ServiceProvider.GetRequiredService<ISystemLogService>();

                try
                {
                    var data = JsonSerializer.Deserialize<EohWebhookDto>(payload);
                    if (data == null) return;

                    await systemLogService.LogOptionAsync("MQTT-Listener", payload);

                    string targetMac = "AA:BB:CC:11:22"; 
                    var hub = await hubService.GetHubByMacAsync(targetMac);

                    if (hub != null)
                    {
                        hub.IsOnline = true;
                        hub.LastHandshake = DateTime.Now;

                        // Update Site Address if v5 or v6 is present
                        if (!string.IsNullOrEmpty(data.v5) || !string.IsNullOrEmpty(data.v6))
                        {
                            if (hub.Site != null)
                            {
                                string newAddress = $"{data.v6}, {data.v5}".Trim(',', ' ');
                                if (!string.IsNullOrEmpty(newAddress))
                                {
                                     hub.Site.Address = newAddress;
                                }
                            }
                        }

                        await hubService.UpdateHubAsync(hub);

                        var sensors = await sensorService.GetSensorsByHubIdAsync(hub.HubId);

                        await ProcessSensorReading(sensorService, sensors, "Temperature", data.v1);
                        await ProcessSensorReading(sensorService, sensors, "Humidity", data.v2);
                        await ProcessSensorReading(sensorService, sensors, "Pressure", data.v3);
                    }
                    else
                    {
                         _logger.LogWarning($"Hub with Mac {targetMac} not found in DB.");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing MQTT message");
                    await systemLogService.LogOptionAsync("MQTT-Listener", payload, ex.Message);
                }
            }
        }

        private async Task ProcessSensorReading(ISensorService sensorService, List<Sensor> sensors, string typeName, double value)
        {
            var sensor = sensors.FirstOrDefault(s => s.Type != null && s.Type.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            if (sensor != null)
            {
                await sensorService.ProcessReadingAsync(sensor.SensorId, (float)value);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null)
            {
                var disconnectOptions = new MqttClientDisconnectOptionsBuilder().Build();
                await _mqttClient.DisconnectAsync(disconnectOptions);
                _mqttClient.Dispose();
            }
            await base.StopAsync(cancellationToken);
        }
    }
}

namespace SWD.API.Dtos
{
    public class EohWebhookDto
    {
        public double v1 { get; set; } // Temperature
        public double v2 { get; set; } // Humidity
        public double v3 { get; set; } // Pressure
        public string? v4 { get; set; } // Time
        public string? v5 { get; set; } // Province
        public string? v6 { get; set; } // City
        public string? deviceId { get; set; } // Optional: in case it's in body
    }
}

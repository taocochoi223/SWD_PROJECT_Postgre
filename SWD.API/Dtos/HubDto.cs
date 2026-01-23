namespace SWD.API.Dtos
{
    public class HubDto
    {
        public int HubId { get; set; }
        public int SiteId { get; set; }
        public string? SiteName { get; set; }
        public string? Name { get; set; }
        public string MacAddress { get; set; } = null!;
        public bool? IsOnline { get; set; }
        public DateTime? LastHandshake { get; set; }
        public int SensorCount { get; set; }
    }
}

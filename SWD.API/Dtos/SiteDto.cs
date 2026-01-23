namespace SWD.API.Dtos
{
    public class SiteDto
    {
        public int SiteId { get; set; }
        public int OrgId { get; set; }
        public string? OrgName { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? GeoLocation { get; set; }
        public int HubCount { get; set; }
    }
}

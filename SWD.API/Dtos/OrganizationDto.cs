
namespace SWD.API.Dtos
{
    public class OrganizationDto
    {
        public int OrgId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int SiteCount { get; set; }
    }

    public class CreateOrganizationDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class UpdateOrganizationDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}

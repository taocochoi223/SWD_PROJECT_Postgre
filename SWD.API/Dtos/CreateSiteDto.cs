using System.ComponentModel.DataAnnotations;

namespace SWD.API.Dtos
{
    public class CreateSiteDto
    {
        [Required]
        public int OrgId { get; set; } // Tạm thời hardcode OrgId=1 ở Frontend cũng được

        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        public string GeoLocation { get; set; }
    }
}
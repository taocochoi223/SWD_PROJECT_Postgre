using System.ComponentModel.DataAnnotations;

namespace SWD.API.Dtos
{
    public class RegisterHubDto
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string MacAddress { get; set; }
    }
}
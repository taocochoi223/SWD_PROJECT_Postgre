using System.ComponentModel.DataAnnotations;

namespace SWD.API.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        public int OrgId { get; set; }

        public int? SiteId { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
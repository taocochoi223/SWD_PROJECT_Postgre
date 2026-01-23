using System.ComponentModel.DataAnnotations;

namespace SWD.API.Dtos
{
    public class CheckTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}

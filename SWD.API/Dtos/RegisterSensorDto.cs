using System.ComponentModel.DataAnnotations;

public class RegisterSensorDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int TypeId { get; set; }
    [Required]
    public int HubId { get; set; }
}
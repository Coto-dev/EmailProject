using System.ComponentModel.DataAnnotations;

namespace EmailProject.WebApp.DTOs;

public class VerifyRequestDto {
    [Required]
    [Range(1000, 9999)]
    public string Code { get; set; }
    [Required]
    [MinLength(3)]
    public string Email { get; set; }
}
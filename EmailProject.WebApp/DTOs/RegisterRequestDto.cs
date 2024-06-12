using System.ComponentModel.DataAnnotations;

namespace EmailProject.WebApp.DTOs;

public class RegisterRequestDto {
    [Required]
    [MinLength(3)]
    public string Email { get; set; }
}
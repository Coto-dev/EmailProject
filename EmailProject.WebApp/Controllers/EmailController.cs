using System.Collections.Concurrent;
using EmailProject.WebApp.DTOs;
using EmailProject.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailProject.WebApp.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthController : ControllerBase
{
    private readonly ICodeSenderService _codeSenderService;


    private static readonly ConcurrentDictionary<string, string> EmailCodes = new();

    public AuthController(ICodeSenderService codeSenderService) 
    {
        _codeSenderService = codeSenderService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return BadRequest("Email is required");
        }

        var code = new Random().Next(1000, 9999).ToString();
        _codeSenderService.SendCode(new RabbitMqMessageDto() 
        {
            Code = code,
            Email = request.Email
        });
        EmailCodes[request.Email] = code;
        return Ok(new { message = "Verification code sent to your email" });
    }
    [HttpPost]
    [Route("verify")]
    public IActionResult Get(VerifyRequestDto request) {  
        
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
        {
            return BadRequest("Email and code are required");
        }

        if (EmailCodes.TryGetValue(request.Email, out var correctCode) && request.Code == correctCode)
        {
            return Ok(new { message = "Verification successful" });
        }
        else
        {
            return BadRequest(new { message = "Invalid verification code" });
        }
        
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FAQDemo.API.DTOs.Auth;
using FAQDemo.API.Helpers;
using FAQDemo.API.Services.Interfaces;
using FluentValidation;

namespace FAQDemo.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        IAuthService auth,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _auth = auth;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _auth.RegisterAsync(dto);

        var result = new
        {
            user.Id,
            user.Email
        };

        return Ok(ApiResponse<object>.SuccessResponse(result, "Registration successful"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _auth.LoginAsync(dto);

        return Ok(ApiResponse<string>.SuccessResponse(token, "Login successful"));
    }
}
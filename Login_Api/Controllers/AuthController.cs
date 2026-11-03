using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IConfiguration _configuration;

    public AuthController(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (existingUser)
        {
            return Conflict(new
            {
                message = "Email is already registered."
            });
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Registration successful.",
            user = new
            {
                user.Id,
                user.Name,
                user.Email
            }
        });
    }

[HttpPost("login")]
public async Task<IActionResult> Login(LoginRequest request)
{
    var email = request.Email.Trim().ToLowerInvariant();

    var user = await _context.Users
        .SingleOrDefaultAsync(u => u.Email == email);

    if (user == null)
    {
        return Unauthorized(new
        {
            message = "Invalid email or password."
        });
    }

    var result = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        request.Password);

    if (result == PasswordVerificationResult.Failed)
    {
        return Unauthorized(new
        {
            message = "Invalid email or password."
        });
    }

    var token = GenerateToken(user);

return Ok(new
{
    message = "Login successful.",
    accessToken = token,
    tokenType = "Bearer",
    expiresIn = 1800,
    user = new
    {
        user.Id,
        user.Name,
        user.Email
    }
});
}


[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile()
{
    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    var name = User.FindFirstValue(
        ClaimTypes.Name);

    var email = User.FindFirstValue(
        ClaimTypes.Email);

    return Ok(new
    {
        message = "Profile retrieved successfully.",
        user = new
        {
            id = userId,
            name = name,
            email = email
        }
    });
}


private String GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),
            new Claim(
                ClaimTypes.Name,
                user.Name
            ),
            new Claim(
                ClaimTypes.Email,
                user.Email
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
            _configuration["JWT:Key"]!
        ));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

}
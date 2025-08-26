using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ToDoDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ToDoDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    [Authorize(Roles = "admin,Admin")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
            return BadRequest("User already exists");

        model.Role = model.Role ?? "user";

        model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User
        {
            UserName = model.UserName,
            Password = model.Password,
            Role = model.Role
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    return Ok(new { message = "User created" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            return Unauthorized("Invalid credentials");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new
        {
            token = tokenHandler.WriteToken(token),
            role = user.Role,
            username = user.UserName
        });
    }
}

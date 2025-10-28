using System.Security.Claims;
using DogSocietyApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogSocietyApi.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DogSocietyDbContext _context;

    public UserController(DogSocietyDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<User>> Register([FromBody] User user)
    {
        await _context.Users.AddAsync(user);

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Login(string email, string password)
    {
        // Source: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-9.0
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email && user.Password == password);

        if (user == null)
        {
            return Forbid("Invalid credentials!");
        }

        var claims = new List<Claim>
        {
            new Claim("Id", user.Id),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, user.Type.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            IsPersistent = false,
            IssuedUtc = DateTime.Now
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        return Ok(user);
    }
}
using System.Security.Claims;
using DogSocietyApi.DataTransferObjects;
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

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register([FromForm] RegisterDto registration)
    {
        var userType = _context.UserTypes.First(x => x.Name == "User");
        var user = new User
        {
            FullName = registration.FullName,
            Email = registration.Email,
            Password = registration.Password,
            PhoneNumber = registration.PhoneNumber,
            Type = userType,
            TypeId = userType.TypeId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromForm] LoginDto login)
    {
        // Source: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-9.0
        var user = await _context.Users.Include(x => x.Type).FirstOrDefaultAsync(user => user.Email == login.Email && user.Password == login.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        var claims = new List<Claim>
        {
            new Claim("UserId", user.UserId.ToString()),
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
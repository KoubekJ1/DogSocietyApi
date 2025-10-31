using DogSocietyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogSocietyApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly DogSocietyDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public AddressController(DogSocietyDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Deletes a specific TodoItem.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
    {
        return await _context.Addresses.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> GetAddress(long id)
    {
        var address = await _context.Addresses.FindAsync(id);

        if (address == null) return NotFound();

        return Ok(address);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Address>> CreateAddress([FromForm] Address address)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(address);
    }
}
using DogSocietyApi.DataTransferObjects;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
    {
        return await _context.Addresses.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> GetAdress(long id)
    {
        var address = await _context.Addresses.FindAsync(id);

        if (address == null) return NotFound();

        return Ok(address);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Association>> CreateAddress([FromForm] Address address)
    {   
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(address);
    }
}
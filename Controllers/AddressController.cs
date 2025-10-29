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
    public async Task<ActionResult<IEnumerable<Association>>> GetAddresses()
    {
        return await _context.Associations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Association>> GetAdress(long id)
    {
        var association = await _context.Associations.FindAsync(id);

        if (association == null) return NotFound();

        return Ok(association);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Association>> CreateAddress([FromForm] Address address)
    {   
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(address);
    }
}
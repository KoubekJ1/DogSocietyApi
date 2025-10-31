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
    /// Retrieves a list of all addresses.
    /// </summary>
    /// <remarks>
    /// This endpoint returns all address records stored in the system.
    /// </remarks>
    /// <returns>
    /// A list of address objects.
    /// </returns>
    /// <response code="200">Returns the list of addresses.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
    {
        return await _context.Addresses.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific address by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the address to retrieve.</param>
    /// <returns>
    /// The address with the specified ID.
    /// </returns>
    /// <response code="200">Returns the address if found.</response>
    /// <response code="404">If the address was not found.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> GetAddress(long id)
    {
        var address = await _context.Addresses.FindAsync(id);

        if (address == null) return NotFound();

        return Ok(address);
    }

    /// <summary>
    /// Creates a new address entry.
    /// </summary>
    /// <param name="address">The address object to create.</param>
    /// <remarks>
    /// The name, city and street name must be between 3 and 100 characters.
    /// The Postal code must be a series of exactly 5 digits.
    /// The street number must be a series of at most 4 digits.
    /// </remarks>
    /// <returns>
    /// The created address object.
    /// </returns>
    /// <response code="200">Returns the created address.</response>
    /// <response code="400">If the provided data is invalid.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpPost("create")]
    public async Task<ActionResult<Address>> CreateAddress([FromForm] Address address)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(address);
    }
}
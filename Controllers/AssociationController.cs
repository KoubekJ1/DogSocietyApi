using DogSocietyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogSocietyApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class AssociationController : ControllerBase
{
    private readonly DogSocietyDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public AssociationController(DogSocietyDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Association>>> GetAssociations()
    {
        return await _context.Associations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Association>> GetAssociation(long id)
    {
        var association = await _context.Associations.FindAsync(id);

        if (association == null) return NotFound();

        return Ok(association);
    }

    [HttpGet("/mine")]
    public async Task<ActionResult<IEnumerable<Association>>> GetOwnedAssociations()
    {
        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "Id").Value);
        var result = _context.Associations.Where(association => association.PresidentId == userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Association>> CreateAssociation([FromBody] Association association)
    {
        await _context.Associations.AddAsync(association);
        await _context.SaveChangesAsync();

        return Ok(association);
    }
}
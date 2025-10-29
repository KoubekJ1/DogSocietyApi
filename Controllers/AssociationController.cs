using DogSocietyApi.DataTransferObjects;
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

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<Association>>> GetOwnedAssociations()
    {
        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
        var result = _context.Associations.Where(association => association.PresidentId == userId);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Association>> CreateAssociation([FromForm] AssociationDto formData)
    {
        var association = new Association
        {
            Name = formData.Name,
            CreationDate = DateOnly.FromDateTime(DateTime.Now),
            Notes = formData.Notes,
            PresidentId = formData.PresidentId != null ? (long)formData.PresidentId : -1,
            AddressId = formData.AddressId,
        };

        if (association.PresidentId == -1)
        {
            long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
            association.PresidentId = userId;
            association.President = _context.Users.Find(userId);
        }
        
        _context.Associations.Add(association);
        await _context.SaveChangesAsync();

        return Ok(association);
    }
}
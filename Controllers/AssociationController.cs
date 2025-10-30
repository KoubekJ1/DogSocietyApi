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

    [HttpPost("changestatute")]
    public async Task<ActionResult> ChangeStatute([FromForm] StatuteDto formData)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);

        var currentStatute = _context.Statutes.FirstOrDefault(statute => statute.ValidFrom <= now
            && (statute.ValidUntil == null || statute.ValidUntil >= now));

        DateOnly validFrom;
        if (currentStatute == null)
        {
            validFrom = now;
        }
        else
        {
            if (currentStatute.ValidUntil == null)
            {
                validFrom = now;
                currentStatute.ValidUntil = now;
            }
            else
            {
                validFrom = (DateOnly)currentStatute.ValidUntil;
            }
        }

        if (formData.ValidFrom == null) formData.ValidFrom = validFrom;

        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);

        var newStatute = new Statute
        {
            ValidFrom = (DateOnly)formData.ValidFrom,
            ValidUntil = formData.ValidUntil,
            Text = formData.Text,
            AuthorId = userId
        };

        _context.Statutes.Add(newStatute);

        await _context.SaveChangesAsync();

        var log = new AuditLog
        {
            Entity = "Statute",
            RowId = newStatute.StatuteId,
            Date = DateTime.Now,
            Comment = formData.Comment,
            UserId = userId,
            TypeId = _context.LogTypes.FirstOrDefault(x => x.Name == "Statute").TypeId
        };

        _context.AuditLog.Add(log);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
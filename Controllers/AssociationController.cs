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

    [HttpGet("statutes/{associationId}")]
    public async Task<ActionResult<Statute>> GetStatutes(int associationId)
    {
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        var currentStatute = _context.Statutes.FirstOrDefault(
            statute => statute.AssociationId == associationId
            && statute.ValidFrom <= now
            && (statute.ValidUntil == null || statute.ValidUntil >= now));

        /*return Ok(new StatuteDto
        {
            AssociationId = associationId,
            ValidFrom = currentStatute.ValidFrom,
            ValidUntil = currentStatute.ValidUntil,
            Text = currentStatute.Text
        });*/

        return Ok(currentStatute);
    }

    [HttpPost("changestatute")]
    public async Task<ActionResult> ChangeStatute([FromForm] StatuteDto formData)
    {
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        var currentStatute = _context.Statutes.FirstOrDefault(
            statute => statute.AssociationId == formData.AssociationId
            && statute.ValidFrom <= now
            && (statute.ValidUntil == null || statute.ValidUntil >= now));

        DateTime validFrom;
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
                validFrom = (DateTime)currentStatute.ValidUntil;
            }
        }

        if (formData.ValidFrom == null) formData.ValidFrom = validFrom;

        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);

        var newStatute = new Statute
        {
            AssociationId = formData.AssociationId,
            ValidFrom = DateTime.SpecifyKind((DateTime)formData.ValidFrom, DateTimeKind.Utc),
            ValidUntil = formData.ValidUntil != null ? DateTime.SpecifyKind((DateTime)formData.ValidUntil, DateTimeKind.Utc) : null,
            Text = formData.Text,
            AuthorId = userId
        };

        _context.Statutes.Add(newStatute);

        await _context.SaveChangesAsync();

        var log = new AuditLog
        {
            Entity = "Statute",
            RowId = newStatute.StatuteId,
            Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            Comment = formData.Comment,
            UserId = userId,
            TypeId = _context.LogTypes.FirstOrDefault(x => x.Name == "Statute").TypeId
        };

        _context.AuditLog.Add(log);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
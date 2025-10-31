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

    /// <summary>
    /// Retrieves a list of all associations.
    /// </summary>
    /// <remarks>
    /// This endpoint returns all associations in the database.  
    /// </remarks>
    /// <returns>A collection of association objects.</returns>
    /// <response code="200">Returns the list of associations.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Association>>> GetAssociations()
    {
        return await _context.Associations.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific association by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the association.</param>
    /// <returns>The Association with the specified ID.</returns>
    /// <response code="200">Returns the requested association.</response>
    /// <response code="404">If the association was not found.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Association>> GetAssociation(long id)
    {
        var association = await _context.Associations.FindAsync(id);

        if (association == null) return NotFound();

        return Ok(association);
    }

    /// <summary>
    /// Retrieves all associations owned by the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// The ownership is determined by comparing the authenticated user's ID with the PresidentId property of each association.
    /// </remarks>
    /// <returns>A collection of association objects owned by the user.</returns>
    /// <response code="200">Returns the list of owned associations.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<Association>>> GetOwnedAssociations()
    {
        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
        var result = _context.Associations.Where(association => association.PresidentId == userId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new dog association.
    /// </summary>
    /// <param name="formData">The data transfer object containing association creation details.</param>
    /// <remarks>
    /// If no PresidentId is provided, the currently authenticated user will be assigned as the president.
    /// </remarks>
    /// <returns>The created association object.</returns>
    /// <response code="200">Returns the newly created association.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="401">If the user is not logged in.</response>
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

    /// <summary>
    /// Retrieves the currently valid statute for a given association.
    /// </summary>
    /// <param name="associationId">The unique identifier of the association.</param>
    /// <remarks>
    /// Returns the statute that is currently in effect,  
    /// determined by comparing the current date with ValidFrom and ValidUntil values.
    /// </remarks>
    /// <returns>The active statute for the specified association.</returns>
    /// <response code="200">Returns the current statute.</response>
    /// <response code="404">If no active statute was found.</response>
    /// <response code="401">If the user is not logged in.</response>
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

    /// <summary>
    /// Creates or updates the statute for a given association.
    /// </summary>
    /// <param name="formData">The data transfer object containing statute information and comments.</param>
    /// <remarks>
    /// The logged in user must own the association. 
    /// If a current statute exists, it will be marked as expired, and a new one will be created starting from the current date. An audit log entry will also be recorded for traceability.
    /// </remarks>
    /// <returns>An HTTP 200 OK response if successful.</returns>
    /// <response code="200">Statute updated successfully.</response>
    /// <response code="400">If the provided data is invalid.</response>
    /// <response code="403">If the user is not the president of the association.</response>
    /// <response code="404">If the association does not exist.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpPost("changestatute")]
    public async Task<ActionResult> ChangeStatute([FromForm] StatuteDto formData)
    {
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
        var association = _context.Associations.Find(formData.AssociationId);

        if (association == null)
        {
            return NotFound("Association not found!");
        }

        if (association.PresidentId != userId)
        {
            return Forbid();
        }

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
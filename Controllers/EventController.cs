using DogSocietyApi.DataTransferObjects;
using DogSocietyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogSocietyApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class EventController : ControllerBase
{
    private readonly DogSocietyDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public EventController(DogSocietyDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        return await _context.Events.ToListAsync();
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<Event>>> GetUpcomingEvents()
    {
        return await _context.Events.Where(x => x.From > DateTime.Now).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(long id)
    {
        var result = await _context.Events.FindAsync(id);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("participation")]
    public async Task<ActionResult<IEnumerable<Event>>> GetJoinedEvents(long associationId)
    {
        var events = _context.Events.Include(x => x.Associations);
        var result = events.Where(x => x.Associations.Any(y => y.AssociationId == associationId));
        return Ok(result);
    }

    [HttpGet("upcomingparticipation")]
    public async Task<ActionResult<IEnumerable<Association>>> GetUpcomingJoinedEvents(long associationId)
    {
        var events = _context.Events.Include(x => x.Associations);
        var result = events.Where(x => x.Associations.Any(y => y.AssociationId == associationId))
            .Where(z => z.From > DateTime.Now);
        return Ok(result);
    }

    [HttpPost("joinevent")]
    public async Task<ActionResult> JoinEvent(long eventId, long associationId)
    {
        var @event = _context.Events.Find(eventId);
        if (@event == null)
        {
            return NotFound($"Event {eventId} not found!");
        }

        var association = _context.Associations.Find(associationId);
        if (association == null)
        {
            return NotFound($"Association {association} not found!");
        }

        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
        if (association.PresidentId != userId)
        {
            return Forbid();
        }

        var log = new AuditLog
        {
            Entity = "Participation",
            RowId = associationId,
            Date = DateTime.Now,
            Comment = $"Association {association.Name}({associationId}) has signed up for event {@event.Name}({eventId})",
            UserId = userId,
            TypeId = _context.LogTypes.FirstOrDefault(x => x.Name == "Participation").TypeId
        };

        @event.Associations.Add(association);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("create")]
    public async Task<ActionResult<Event>> CreateEvent([FromForm] EventDto formData)
    {
        var @event = new Event
        {
            Name = formData.Name,
            From = DateTime.SpecifyKind(formData.From, DateTimeKind.Utc),
            Until = DateTime.SpecifyKind(formData.Until, DateTimeKind.Utc),
            AddressId = formData.AddressId,
            TypeId = formData.TypeId
        };
        
        _context.Events.Add(@event);
        await _context.SaveChangesAsync();

        return Ok(@event);
    }
}
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

    [HttpPost("create")]
    public async Task<ActionResult<Event>> CreateEvent([FromForm] EventDto formData)
    {
        var @event = new Event
        {
            Name = formData.Name,
            From = formData.From,
            Until = formData.Until,
            AddressId = formData.AddressId,
            TypeId = formData.TypeId
        };
        
        _context.Events.Add(@event);
        await _context.SaveChangesAsync();

        return Ok(@event);
    }
}
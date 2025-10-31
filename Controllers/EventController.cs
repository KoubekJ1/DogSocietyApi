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

    /// <summary>
    /// Retrieves a list of all events.
    /// </summary>
    /// <remarks>
    /// This endpoint returns all registered events, regardless of their dates.
    /// </remarks>
    /// <returns>A list of event objects.</returns>
    /// <response code="200">Returns the list of events.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        return await _context.Events.ToListAsync();
    }

    /// <summary>
    /// Retrieves a list of upcoming events
    /// </summary>
    /// <returns>A list of upcoming event objects.</returns>
    /// <response code="200">Returns the list of upcoming events.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<Event>>> GetUpcomingEvents()
    {
        return await _context.Events.Where(x => x.From > DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)).ToListAsync();
    }

    /// <summary>
    /// Retrieves the details of a specific event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <returns>The event matching the specified ID.</returns>
    /// <response code="200">Returns the event data.</response>
    /// <response code="404">If no event with the given ID exists.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(long id)
    {
        var result = await _context.Events.FindAsync(id);

        if (result == null) return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of events in which the specified association is participating.
    /// </summary>
    /// <param name="associationId">The ID of the association.</param>
    /// <returns>A collection of event objects where the given association is participating.</returns>
    /// <response code="200">Returns the list of events the association is participating in.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpGet("participation/{associationId}")]
    public async Task<ActionResult<IEnumerable<Event>>> GetJoinedEvents(long associationId)
    {
        var events = _context.Events;
        var result = events.Where(x => x.Associations.Any(y => y.AssociationId == associationId));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of upcoming events where a specific association is participating.
    /// </summary>
    /// <param name="associationId">The ID of the association.</param>
    /// <returns>A list of future <see cref="Event"/> objects joined by the association.</returns>
    /// <response code="200">Returns the upcoming joined events.</response>
    [HttpGet("upcomingparticipation/{associationId}")]
    public async Task<ActionResult<IEnumerable<Association>>> GetUpcomingJoinedEvents(long associationId)
    {
        var events = _context.Events;
        var result = events.Where(x => x.Associations.Any(y => y.AssociationId == associationId))
            .Where(z => z.From > DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
        return Ok(result);
    }

    /// <summary>
    /// Allows an association to join an event.
    /// </summary>
    /// <param name="formData">The JoinEventDto containing the event ID and association ID.</param>
    /// <remarks>
    /// Only the president of the association can register it for an event.  
    /// If the event has already started, it cannot be joined.
    /// </remarks>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="200">Association successfully joined the event or was already participating.</response>
    /// <response code="400">Invalid event data or event is in the past.</response>
    /// <response code="401">If the user is not authorized as the association president.</response>
    /// <response code="404">If the event or association was not found.</response>
    [HttpPost("joinevent")]
    public async Task<ActionResult> JoinEvent([FromForm] JoinEventDto formData)
    {
        var @event = _context.Events.Include(x => x.Associations).FirstOrDefault(x => x.EventId == formData.eventId);
        if (@event == null)
        {
            return NotFound($"Event {formData.eventId} not found!");
        }

        var association = _context.Associations.Find(formData.associationId);
        if (association == null)
        {
            return NotFound($"Association {association} not found!");
        }

        long userId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
        if (association.PresidentId != userId)
        {
            return Unauthorized();
        }

        if (@event.From < DateTime.Now)
        {
            return BadRequest("Event is no longer available!");
        }

        if (@event.Associations.FirstOrDefault(x => x.AssociationId == formData.associationId) != null)
        {
            return Ok("Association already participating!");
        }

        var log = new AuditLog
        {
            Entity = "Participation",
            RowId = formData.associationId,
            Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            Comment = $"Association {association.Name}({formData.associationId}) has signed up for event {@event.Name}({formData.eventId})",
            UserId = userId,
            TypeId = _context.LogTypes.FirstOrDefault(x => x.Name == "Participation").TypeId
        };

        _context.AuditLog.Add(log);
        await _context.SaveChangesAsync();

        @event.Associations.Add(association);
        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="formData">The EventDto containing event details.</param>
    /// <remarks>
    /// The event From time must be later than the current moment.
    /// The event Until time must be later than the From time.
    /// The expected time zone for the time fields is UTC.
    /// </remarks>
    /// <returns>The created event object.</returns>
    /// <response code="200">Returns the created event.</response>
    /// <response code="400">If the provided data is invalid.</response>
    /// <response code="401">If the user is not logged in.</response>
    [HttpPost("create")]
    public async Task<ActionResult<Event>> CreateEvent([FromForm] EventDto formData)
    {
        if (formData.From < DateTime.Now)
        {
            return BadRequest("Invalid event start time!");
        }
        if (formData.Until < formData.From)
        {
            return BadRequest("Invalid event length!");
        }
        if (await _context.EventTypes.FindAsync(formData.TypeId) == null)
        {
            return BadRequest("Invalid event type!");
        }
        if (await _context.Addresses.FindAsync(formData.AddressId) == null)
        {
            return BadRequest("Invalid address ID!");
        }

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
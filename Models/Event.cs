using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Event
{
    [Key]
    public long EventId { get; set; }
    public string Name { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public Address Address { get; set; }
    public long AddressId { get; set; }
    public EventType Type { get; set; }
    public long TypeId { get; set; }

    public List<Association> Associations { get; set; }
}
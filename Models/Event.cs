using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Event
{
    [Key]
    public long EventId { get; set; }
    public string Name { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
    public Address Address { get; set; }
    public long AddressId { get; set; }
    public EventType Type { get; set; }
    public long TypeId { get; set; }

    public List<Association> Associations { get; set; } = new List<Association>();
}
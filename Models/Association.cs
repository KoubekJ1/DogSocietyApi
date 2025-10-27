using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Association
{
    [Key]
    public long AssociationId { get; set; }
    public string Name { get; set; }
    public DateOnly CreationDate { get; set; }
    public string Notes { get; set; }

    public long PresidentId { get; set; }
    public User President { get; set; }
    public long AddressId { get; set; }
    public Address Address { get; set; }

    public List<Event> Events { get; set; }
}
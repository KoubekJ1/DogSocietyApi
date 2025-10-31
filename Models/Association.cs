using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Association
{
    [Key]
    public long AssociationId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    public DateOnly CreationDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public long PresidentId { get; set; }
    public User President { get; set; }
    public long AddressId { get; set; }
    public Address Address { get; set; }

    public List<Event> Events { get; set; }
}
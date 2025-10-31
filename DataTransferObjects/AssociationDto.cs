using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.DataTransferObjects;

public class AssociationDto
{
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    //public DateOnly CreationDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public long? PresidentId { get; set; }
    public long AddressId { get; set; }
}
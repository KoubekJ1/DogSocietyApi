using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.DataTransferObjects;

public class StatuteDto
{
    public long AssociationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Text { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string Comment { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Statute
{
    [Key]
    public long StatuteId { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Text { get; set; }
    public User Author { get; set; }
    public long AuthorId { get; set; }

    public Association Association { get; set; }
    public long AssociationId { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Statute
{
    [Key]
    public long StatuteId { get; set; }

    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string Text { get; set; }
    public User Author { get; set; }
    public long AuthorId { get; set; }
}
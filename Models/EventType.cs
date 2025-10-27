using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class EventType
{
    [Key]
    public long TypeId { get; set; }
    public string Name { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class User
{
    [Key]
    public long UserId { get; set; }

    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public long TypeId { get; set; }
    public UserType Type { get; set; }
}
namespace DogSocietyApi.Models;

public class User
{
    public long UserId { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public long TypeId { get; set; }
    public UserType Type { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.DataTransferObjects;

public class RegisterDto
{
    [RegularExpression(@"^([Á-žA-z]-?\s?){2,}$")]
    public string FullName { get; set; }
    
    [RegularExpression(@"^([a-z]|\.)+@([a-z]|\.)+.([a-z]|\.)+$")]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }

    [RegularExpression(@"\d{9}")]
    public string PhoneNumber { get; set; }
}
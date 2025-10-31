using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Address
{
    [Key]
    public long AddressId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string? Name { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string City { get; set; }

    [Required]
    [RegularExpression(@"^\d{5}$")]
    public string PostalCode { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string StreetName { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}$")]
    public string StreetNumber { get; set; }
}
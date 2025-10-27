using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class Address
{
    [Key]
    public long AddressId { get; set; }

    public string? Name { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string StreetName { get; set; }
    public string StreetNumber { get; set; }
}
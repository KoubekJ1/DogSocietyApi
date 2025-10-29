namespace DogSocietyApi.DataTransferObjects;

public class AssociationDto
{
    public string Name { get; set; }
    //public DateOnly CreationDate { get; set; }
    public string? Notes { get; set; }

    public long? PresidentId { get; set; }
    public long AddressId { get; set; }
}
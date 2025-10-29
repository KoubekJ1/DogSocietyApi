namespace DogSocietyApi.DataTransferObjects;

public class EventDto
{
    public string Name { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
    public long AddressId { get; set; }
    public long TypeId { get; set; }
}
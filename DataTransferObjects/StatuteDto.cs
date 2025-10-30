namespace DogSocietyApi.DataTransferObjects;

public class StatuteDto
{
    public long AssociationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Text { get; set; }
    public string Comment { get; set; }
}
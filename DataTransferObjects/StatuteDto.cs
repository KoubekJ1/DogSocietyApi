namespace DogSocietyApi.DataTransferObjects;

public class StatuteDto
{
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string Text { get; set; }
    public string Comment { get; set; }
}
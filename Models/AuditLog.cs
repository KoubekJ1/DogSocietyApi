using System.ComponentModel.DataAnnotations;

namespace DogSocietyApi.Models;

public class AuditLog
{
    [Key]
    public long LogId { get; set; }
    public string Entity { get; set; }
    public long RowId { get; set; }
    public DateTime Date { get; set; }
    public string Comment { get; set; }

    public User User { get; set; }
    public long UserId { get; set; }

    public LogType Type { get; set; }
    public long TypeId { get; set; }
}
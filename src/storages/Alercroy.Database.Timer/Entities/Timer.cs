using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alercroy.Database.Timer.Entities;

public class Timer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public uint ChatId { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    [Required]
    public DateTimeOffset BeginDate { get; set; }
}
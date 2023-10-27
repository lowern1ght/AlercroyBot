using Microsoft.EntityFrameworkCore;

namespace Alercroy.Database.Timer;

public class TimerDbContext : DbContext
{
    public TimerDbContext(DbContextOptions<TimerDbContext> options)
        : base(options) { }

    public DbSet<Entities.Timer> Timers { get; set; } = null!;
}
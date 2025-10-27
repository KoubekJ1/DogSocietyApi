using Microsoft.EntityFrameworkCore;

namespace DogSocietyApi.Models;

public class DogSocietyDbContext : DbContext
{
	public DogSocietyDbContext(DbContextOptions<DogSocietyDbContext> options)
		: base(options)
	{
	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

	public DbSet<Address> Addresses { get; set; }
	public DbSet<Association> Associations { get; set; }
	public DbSet<AuditLog> AuditLog { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<EventType> EventTypes { get; set; }
	public DbSet<LogType> LogTypes { get; set; }
	public DbSet<Statute> Statutes { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<UserType> UserTypes { get; set; }
}
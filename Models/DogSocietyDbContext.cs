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
		modelBuilder.Entity<UserType>().HasData(
			[
				new UserType { TypeId = 1, Name = "User" },
			 	new UserType { TypeId = 2, Name = "Administrator" }
			]
		);

		modelBuilder.Entity<EventType>().HasData(
			[
				new EventType { TypeId = 1, Name = "Showcase" },
				new EventType { TypeId = 2, Name = "Race" },
				new EventType { TypeId = 3, Name = "Seminar" }
			]
		);

		modelBuilder.Entity<LogType>().HasData(
			[
				new EventType { TypeId = 1, Name = "Statute" },
				new EventType { TypeId = 2, Name = "Participation" }
			]
		);

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
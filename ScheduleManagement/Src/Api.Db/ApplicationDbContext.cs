using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
	: DbContext(options)
{
	public DbSet<Person> Persons { get; init; }
	public DbSet<Role> Roles { get; init; }
	public DbSet<RefreshToken> Tokens { get; init; }
	public DbSet<ResetPasswordOtp> ResetPasswordOpts { get; init; }
	public DbSet<Department> Departments { get; init; }
	public DbSet<LastOpenedSchedule> LastOpenedSchedules { get; init; }
	public DbSet<Cathedral> Cathedrals { get; init; }
	public DbSet<StudyType> StudyTypes { get; init; }
	public DbSet<StudySpecialization> StudySpecializations { get; init; }
	public DbSet<RoomType> RoomTypes { get; init; }
	public DbSet<StudyRoom> StudyRooms { get; init; }
	public DbSet<StudySubject> StudySubjects { get; init; }
	public DbSet<StudyDegree> StudyDegrees { get; init; }
	public DbSet<Semester> Semesters { get; init; }
	public DbSet<StudyGroup> StudyGroups { get; init; }
	public DbSet<Weekday> Weekdays { get; init; }
	public DbSet<WeekScheduleOccur> WeekdayScheduleOccurs { get; init; }
	public DbSet<ScheduleSubject> ScheduleSubjects { get; init; }
	public DbSet<ScheduleSubjectType> ScheduleSubjectTypes { get; init; }
	public DbSet<ContactFormIssueType> ContactFormIssueTypes { get; init; }
	public DbSet<ContactMessage> ContactMessages { get; init; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder
			.UseMySql(
				configuration.GetConnectionString("MySQL"),
				new MySqlServerVersion(ApiConfig.DbDriverVersion))
			.UseLoggerFactory(LoggerFactory.Create(factory => factory
				.AddConsole()
				.AddFilter(level => level >= LogLevel.Information)
			))
			.EnableSensitiveDataLogging()
			.EnableDetailedErrors();
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var d = DateTime.Now;
		var entitiesWithPrimaryKey = ChangeTracker.Entries()
			.Where(x => x is { Entity: AbstractEntity, State: EntityState.Added or EntityState.Modified });

		foreach (var entityEntry in entitiesWithPrimaryKey)
		{
			var formatedDateTime = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
			if (entityEntry.State == EntityState.Added)
			{
				((AbstractEntity)entityEntry.Entity).CreatedDate = formatedDateTime;
			}
			((AbstractEntity)entityEntry.Entity).UpdatedDate = formatedDateTime;
		}
		return await base.SaveChangesAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.BuildPersonRelations();
		modelBuilder.BuildScheduleSubjectRelations();
		modelBuilder.BuildContactMessageRelations();
	}
}

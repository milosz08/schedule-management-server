using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Network.Auth;
using ScheduleManagement.Api.Network.User;

namespace ScheduleManagement.Api.Db;

public class ApplicationDbSeeder(
	ApplicationDbContext dbContext,
	IAuthService authService,
	IHostEnvironment environment)
{
	public async Task Seed()
	{
		if (await dbContext.Database.CanConnectAsync())
		{
			await InsertAllRoles();
			await InsertAllSemesters();
			await InsertAllWeekdays();
			await InsertAllScheduleSubjectTypes();
			await InsertInitialDepartment();
			await InsertInitialCathedral();
			await InsertDefaultAdminData();
			await InsertDefaultStudyTypes();
			await InsertStudyRoomsTypes();
			await InsertStudyDegreesTypes();
			await InsertAllContactFromIssueTypes();
		}
	}

	private async Task InsertAllRoles()
	{
		if (!dbContext.Roles.Any())
		{
			await dbContext.Roles.AddRangeAsync(ConvertJsonToList<Role>("Roles"));
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertAllSemesters()
	{
		if (!dbContext.Semesters.Any())
		{
			await dbContext.Semesters.AddRangeAsync(ConvertJsonToList<Semester>("Semesters"));
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertAllWeekdays()
	{
		if (!dbContext.Weekdays.Any())
		{
			await dbContext.Weekdays.AddRangeAsync(ConvertJsonToList<Weekday>("Weekdays"));
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertAllScheduleSubjectTypes()
	{
		if (!dbContext.ScheduleSubjectTypes.Any())
		{
			await dbContext.ScheduleSubjectTypes.AddRangeAsync(
				ConvertJsonToList<ScheduleSubjectType>("ScheduleTypes"));
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertInitialDepartment()
	{
		if (!dbContext.Departments.Any())
		{
			var initialDepartments = ConvertJsonToList<Department>("Departments");
			initialDepartments[0].IsRemovable = false;
			await dbContext.Departments.AddAsync(initialDepartments[0]);
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertInitialCathedral()
	{
		if (!dbContext.Cathedrals.Any())
		{
			var initialCathedrals = ConvertJsonToList<Cathedral>("Cathedrals");
			initialCathedrals[0].DepartmentId = dbContext.Departments.First().Id;
			initialCathedrals[0].IfRemovable = false;
			await dbContext.Cathedrals.AddAsync(initialCathedrals[0]);
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertDefaultAdminData()
	{
		var account = ApiConfig.InitAccount;
		var findPerson = await dbContext.Persons
			.FirstOrDefaultAsync(p => p.Name == account.Name && p.Surname == account.Surname);
		var initialCathedrals = ConvertJsonToList<Cathedral>("Cathedrals");
		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.Name.Equals(initialCathedrals[0].Name, StringComparison.OrdinalIgnoreCase));
		if (findPerson == null)
		{
			await authService.Register(new RegisterUpdateUserRequestDto
			{
				Name = account.Name,
				Surname = account.Surname,
				Nationality = "Polska",
				City = "Gliwice",
				IfRemovable = false,
				Role = UserRole.Administrator,
				DepartmentName = findCathedral!.Department.Name,
				CathedralName = findCathedral.Name
			}, ApiConfig.InitAccount.Password);
		}
	}

	private async Task InsertDefaultStudyTypes()
	{
		if (!dbContext.StudyTypes.Any())
		{
			await dbContext.StudyTypes.AddRangeAsync(ConvertJsonToList<StudyType>("StudyTypes"));
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertStudyRoomsTypes()
	{
		if (!dbContext.RoomTypes.Any())
		{
			var test = ConvertJsonToList<RoomType>("StudyRoom");
			await dbContext.RoomTypes.AddRangeAsync(test);
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertStudyDegreesTypes()
	{
		if (!dbContext.StudyDegrees.Any())
		{
			await dbContext.StudyDegrees.AddRangeAsync(ConvertJsonToList<StudyDegree>("StudyDegrees"));
			await dbContext.SaveChangesAsync();
		}
	}

	private async Task InsertAllContactFromIssueTypes()
	{
		if (!dbContext.ContactFormIssueTypes.Any())
		{
			await dbContext.ContactFormIssueTypes.AddRangeAsync(
				ConvertJsonToList<ContactFormIssueType>("ContactFormIssueTypes"));
			await dbContext.SaveChangesAsync();
		}
	}

	private List<T> ConvertJsonToList<T>(string fileName)
	{
		var roomTypesPath = Path.Combine(environment.ContentRootPath, "SeedingData", $"{fileName}.json");
		var jsonString = File.ReadAllText(roomTypesPath);
		List<T>? deserialisedArray;
		try
		{
			deserialisedArray = JsonSerializer.Deserialize<List<T>>(jsonString);
			if (deserialisedArray == null)
			{
				throw new JsonException();
			}
		}
		catch (JsonException ex)
		{
			throw new RestApiException("Nieprawidłowy format pliku json! Stacktrace: " + ex.Message,
				HttpStatusCode.InternalServerError);
		}
		return deserialisedArray;
	}
}

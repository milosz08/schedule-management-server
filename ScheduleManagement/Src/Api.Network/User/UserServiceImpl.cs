using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Email;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Network.Auth;
using ScheduleManagement.Api.Pagination;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.User;

public class UserServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<UserServiceImpl> logger,
	IMailboxProxyService mailboxProxyService,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IUserService
{
	public PaginationResponseDto<UserResponseDto> GetAllUsers(SearchQueryRequestDto searchQuery)
	{
		var usersBaseQuery = dbContext.Persons
			.Include(p => p.Role)
			.Where(p => searchQuery.SearchPhrase == null ||
			            p.Surname.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Person, object>>>
			{
				{ nameof(Person.Id), p => p.Id },
				{ nameof(Person.Surname), p => p.Surname },
				{ nameof(Person.Login), p => p.Login },
				{ nameof(Person.Role), p => p.Role.Name }
			}, searchQuery, ref usersBaseQuery);

		var allUsers = mapper.Map<List<UserResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(usersBaseQuery, searchQuery));

		return new PaginationResponseDto<UserResponseDto>(
			allUsers, usersBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public async Task<RegisterUpdateUserResponseDto> UpdateUserDetails(RegisterUpdateUserRequestDto dto, long userId,
		bool isUpdateEmailPass)
	{
		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Subjects)
			.Include(p => p.Cathedral)
			.Include(p => p.Department)
			.Include(p => p.ScheduleSubjects)
			.ThenInclude(scheduleSubject => scheduleSubject.ScheduleTeachers)
			.Include(p => p.StudySpecializations)
			.FirstOrDefaultAsync(p => p.Id == userId);

		if (findPerson == null)
			throw new RestApiException("Nie znaleziono użytkownika z podanym id.", HttpStatusCode.NotFound);

		var findRole = await dbContext.Roles
			.FirstOrDefaultAsync(r => r.Name.Equals(dto.Role, StringComparison.OrdinalIgnoreCase));
		if (findRole == null)
			throw new RestApiException("Nie znaleziono roli z podaną nazwą.", HttpStatusCode.NotFound);

		var findDepartment = await dbContext.Departments
			.FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
		if (findDepartment == null)
			throw new RestApiException("Nie znaleziono wydziału z podaną nazwą.", HttpStatusCode.NotFound);

		if (!UserRole.Student.Equals(dto.Role))
		{
			var findCathedral = await dbContext.Cathedrals
				.FirstOrDefaultAsync(c => c.Name.Equals(dto.CathedralName, StringComparison.OrdinalIgnoreCase));
			if (findDepartment == null)
				throw new RestApiException("Nie znaleziono katedry z podaną nazwą.", HttpStatusCode.NotFound);

			findPerson.Cathedral!.Id = findCathedral!.Id;
		}
		else if (UserRole.Student.Equals(dto.Role))
		{
			findPerson.StudySpecializations.Clear();
			findPerson.StudySpecializations = dbContext.StudySpecializations
				.Where(s => dto.StudySpecsOrSubjects.Any(sid => sid == s.Id)).ToList();
		}

		if (UserRole.Teacher.Equals(dto.Role) || UserRole.Editor.Equals(dto.Role))
		{
			findPerson.Subjects.Clear();
			findPerson.Subjects = dbContext.StudySubjects
				.Where(s => dto.StudySpecsOrSubjects.Any(sid => sid == s.Id)).ToList();
		}

		if (((UserRole.Teacher.Equals(dto.Role) ||
		      UserRole.Administrator.Equals(dto.Role)) &&
		     (UserRole.Editor.Equals(findPerson.Role.Name) ||
		      UserRole.Teacher.Equals(findPerson.Role.Name))) ||
		    !dto.DepartmentName.Equals(findPerson.Department!.Name, StringComparison.OrdinalIgnoreCase))
		{
			var toRemove = findPerson.ScheduleSubjects.Where(s => s.ScheduleTeachers.Any(st => st.Id == userId))
				.ToList();
			findPerson.ScheduleSubjects.Clear();

			var toRemoveEntities = dbContext.ScheduleSubjects.Where(sb => toRemove.Contains(sb));
			dbContext.ScheduleSubjects.RemoveRange(toRemoveEntities);

			logger.LogInformation("Successfully removed: {} schedule subjects", toRemoveEntities.Count());
		}

		if (isUpdateEmailPass)
		{
			var generatedEmailPassword = RandomUtils.GenerateUserFirstPassword();
			mailboxProxyService.AddNewEmailAccount(findPerson.Email, generatedEmailPassword);
			findPerson.EmailPassword = generatedEmailPassword;
		}

		findPerson.City = dto.City;
		findPerson.Nationality = dto.Nationality;
		findPerson.Role = findRole;
		findPerson.Department!.Id = findDepartment.Id;

		await dbContext.SaveChangesAsync();
		var resDto = mapper.Map<RegisterUpdateUserResponseDto>(findPerson);

		logger.LogInformation("Successfully updated user account: {}", resDto);
		return resDto;
	}

	public async Task<List<NameIdElementDto>> GetAllEmployeersScheduleBaseCath(long deptId, long cathId)
	{
		var allUsersWithoutStudents = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.Include(p => p.Cathedral)
			.Where(p => p.Department!.Id == deptId && p.Cathedral!.Id == cathId &&
			            !p.Role.Name.Equals(UserRole.Student, StringComparison.OrdinalIgnoreCase))
			.ToListAsync();

		allUsersWithoutStudents
			.Sort((first, second) => string.Compare(first.Surname, second.Surname, StringComparison.Ordinal));

		return allUsersWithoutStudents
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<List<NameIdElementDto>> GetAllTeachersScheduleBaseDeptAndSpec(long deptId, string? subjectName)
	{
		var selectedUsersWithoutStudents = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.Include(p => p.Subjects)
			.Where(p => p.Department!.Id == deptId &&
			            p.Subjects.Any(s => s.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase))
			            && !p.Role.Name.Equals(UserRole.Student, StringComparison.OrdinalIgnoreCase))
			.ToListAsync();

		if (selectedUsersWithoutStudents.Count > 0)
		{
			selectedUsersWithoutStudents
				.Sort((first, second) => string.Compare(first.Surname, second.Surname, StringComparison.Ordinal));

			return selectedUsersWithoutStudents
				.Select(mapper.Map<NameIdElementDto>)
				.ToList();
		}

		return [];
	}

	public async Task<DashboardDetailsResDto> GetDashboardPanelData(ClaimsPrincipal claimsPrincipal)
	{
		var userIdentity = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name));

		if (userIdentity == null) throw new RestApiException("Dostęp do zasobu zabroniony.", HttpStatusCode.Forbidden);

		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Subjects)
			.Include(p => p.Cathedral)
			.Include(p => p.Department)
			.Include(p => p.StudySpecializations)
			.FirstOrDefaultAsync(p => p.Login == userIdentity.Value);

		if (findPerson == null)
			throw new RestApiException("Podany użytkownik nie istenieje w systemie.", HttpStatusCode.NotFound);

		var dashboardDetailsResDto = mapper.Map<DashboardDetailsResDto>(findPerson);
		if (findPerson.Cathedral != null)
			dashboardDetailsResDto.CathedralFullName = $"{findPerson.Cathedral.Name} ({findPerson.Cathedral.Alias})";

		if (UserRole.Student.Equals(findPerson.Role.Name))
			dashboardDetailsResDto.StudySpecializations =
				findPerson.StudySpecializations.Select(s => $"{s.Name} ({s.Alias})").ToList();

		switch (findPerson.Role.Name)
		{
			case UserRole.Teacher:
			case UserRole.Editor:
				dashboardDetailsResDto.StudySubjects =
					findPerson.Subjects.Select(s => $"{s.Name} ({s.Alias})").ToList();
				break;
			case UserRole.Administrator:
				dashboardDetailsResDto.DashboardElementsCount = new DashboardElementsCount(
					dbContext.Departments.Count(),
					dbContext.Cathedrals.Count(),
					dbContext.StudyRooms.Count(),
					dbContext.StudySpecializations.Count(),
					dbContext.StudySubjects.Count(),
					dbContext.StudyGroups.Count()
				);
				dashboardDetailsResDto.DashboardUserTypesCount = new DashboardUserTypesCount(
					dbContext.Persons.Include(r => r.Role).Count(p => UserRole.Student.Equals(p.Role.Name)),
					dbContext.Persons.Include(r => r.Role).Count(p => UserRole.Teacher.Equals(p.Role.Name)),
					dbContext.Persons.Include(r => r.Role).Count(p => UserRole.Editor.Equals(p.Role.Name)),
					dbContext.Persons.Include(r => r.Role).Count(p => UserRole.Administrator.Equals(p.Role.Name))
				);
				break;
		}

		return dashboardDetailsResDto;
	}

	public async Task<UserDetailsEditResDto> GetUserDetails(long userId)
	{
		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Subjects)
			.Include(p => p.Cathedral)
			.Include(p => p.Department)
			.Include(p => p.StudySpecializations)
			.FirstOrDefaultAsync(p => p.Id == userId);

		if (findPerson == null)
			throw new RestApiException("Nie znaleziono użytkownika z podanym numerem id.", HttpStatusCode.NotFound);

		var response = mapper.Map<UserDetailsEditResDto>(findPerson);

		switch (findPerson.Role.Name)
		{
			case UserRole.Teacher:
			case UserRole.Editor:
				response.StudySpecsOrSubjects = findPerson.Subjects.Select(s => s.Id).ToList();
				break;
			case UserRole.Student:
				response.StudySpecsOrSubjects = findPerson.StudySpecializations.Select(s => s.Id).ToList();
				break;
		}

		return response;
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);

		var personsNotRemoveAccounts = dbContext.Persons.Where(p => !p.IsRemovable).Select(p => p.Id);
		var filteredDeletedPersons = items.Ids.Where(id => !personsNotRemoveAccounts.Contains(id)).ToArray();

		var removeMessage = "Nie usunięto żadnego użytkownika.";
		if (filteredDeletedPersons.Length != 0)
		{
			var personsToRemove = await dbContext.Persons
				.Where(p => filteredDeletedPersons.Any(id => id == p.Id))
				.ToListAsync();
			foreach (var person in personsToRemove) mailboxProxyService.DeleteEmailAccount(person.Email);

			dbContext.Persons.RemoveRange(personsToRemove);
			await dbContext.SaveChangesAsync();

			removeMessage =
				$"Pomyślnie usunięto wybranych użytkowników. Liczba usuniętych użytkowników: {personsToRemove.Count}.";
			logger.LogInformation("Successfully removed: {} users", personsToRemove.Count);
		}

		return new MessageContentResDto
		{
			Message = removeMessage
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);

		var findAllRemovingPersons = dbContext.Persons.Where(p => p.IsRemovable);
		if (findAllRemovingPersons.Any())
		{
			foreach (var person in findAllRemovingPersons) mailboxProxyService.DeleteEmailAccount(person.Email);

			dbContext.Persons.RemoveRange(findAllRemovingPersons);
			await dbContext.SaveChangesAsync();

			logger.LogInformation("Successfully removed: {} users", findAllRemovingPersons.Count());
		}

		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkich użytkowników."
		};
	}
}

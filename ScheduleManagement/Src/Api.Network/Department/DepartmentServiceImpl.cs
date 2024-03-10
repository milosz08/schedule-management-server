using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.Department;

public class DepartmentServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<DepartmentServiceImpl> logger,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IDepartmentService
{
	public async Task<DepartmentRequestResponseDto> CreateDepartment(DepartmentRequestResponseDto dto)
	{
		var findDepartment = await dbContext.Departments
			.FirstOrDefaultAsync(d =>
				d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
				d.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase));

		if (findDepartment != null)
		{
			throw new RestApiException("Podany wydział istnieje już w systemie.", HttpStatusCode.ExpectationFailed);
		}
		var newDepartment = mapper.Map<Entity.Department>(dto);
		await dbContext.Departments.AddAsync(newDepartment);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully created new department: {}", dto);
		return dto;
	}

	public async Task<DepartmentRequestResponseDto> UpdateDepartment(DepartmentRequestResponseDto dto, long deptId)
	{
		var findDepartment = await dbContext.Departments.FirstOrDefaultAsync(d => d.Id == deptId);
		if (findDepartment == null)
		{
			throw new RestApiException("Nie znaleziono wydziału z podanym id.", HttpStatusCode.NotFound);
		}
		if (dto.Name.Equals(findDepartment.Name) && dto.Alias.Equals(findDepartment.Alias))
		{
			throw new RestApiException("Należy wprowadzić wartości różne od poprzednich.",
				HttpStatusCode.ExpectationFailed);
		}
		findDepartment.Name = dto.Name;
		findDepartment.Alias = dto.Alias;
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully updated department with ID: {}, {}", deptId, findDepartment);
		return dto;
	}

	public SearchQueryResponseDto GetAllDepartments(string? name)
	{
		if (string.IsNullOrEmpty(name))
		{
			var allDepartments = dbContext.Departments
				.Select(d => d.Name)
				.ToList();
			allDepartments.Sort();
			return new SearchQueryResponseDto(allDepartments);
		}
		var findAllDepartments = dbContext.Departments
			.Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
			.Select(d => d.Name)
			.ToList();
		findAllDepartments.Sort();

		return findAllDepartments.Count > 0
			? new SearchQueryResponseDto(findAllDepartments)
			: new SearchQueryResponseDto();
	}

	public PaginationResponseDto<DepartmentQueryResponseDto> GetPageableDepartments(SearchQueryRequestDto searchQuery)
	{
		var deparmentsBaseQuery = dbContext.Departments
			.Where(d => searchQuery.SearchPhrase == null ||
			            d.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Entity.Department, object>>>
			{
				{ nameof(Entity.Department.Id), d => d.Id },
				{ nameof(Entity.Department.Name), d => d.Name },
				{ nameof(Entity.Department.Alias), d => d.Alias }
			}, searchQuery, ref deparmentsBaseQuery);
		}
		var allDepts = mapper.Map<List<DepartmentQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(deparmentsBaseQuery, searchQuery));

		return new PaginationResponseDto<DepartmentQueryResponseDto>(
			allDepts, deparmentsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public async Task<List<NameIdElementDto>> GetAllDepartmentsSchedule()
	{
		var findAllDepartments = await dbContext.Departments.ToListAsync();

		findAllDepartments.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		return findAllDepartments
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<DepartmentEditResDto> GetDepartmentDetails(long deptId)
	{
		var findDepartment = await dbContext.Departments.FirstOrDefaultAsync(d => d.Id == deptId);
		if (findDepartment == null)
		{
			throw new RestApiException("Nie znaleziono szukanego wydziału.", HttpStatusCode.NotFound);
		}
		return mapper.Map<DepartmentEditResDto>(findDepartment);
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var message = "Nie usunięto żadnego wydziału.";

		var nonRemovableIds = dbContext.Departments
			.Where(d => !d.IsRemovable)
			.Select(d => d.Id);

		var removableIds = items.Ids.Where(e => !nonRemovableIds.Contains(e)).AsQueryable();
		var toRemoved = await dbContext.Departments
			.Where(d => removableIds.Any(id => id == d.Id))
			.ToListAsync();

		if (toRemoved.Count != 0)
		{
			message = $"Pomyślnie usunięto wybrane wydziały. Liczba usuniętych wydziałów: {toRemoved.Count}.";
		}
		dbContext.Departments.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} departments", toRemoved.Count);
		return new MessageContentResDto
		{
			Message = message
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var toRemoved = dbContext.Departments.Where(d => d.IsRemovable);

		dbContext.Departments.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed all removable departments: {}", toRemoved.Count());
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie wydziały możliwe do usunięcia."
		};
	}
}

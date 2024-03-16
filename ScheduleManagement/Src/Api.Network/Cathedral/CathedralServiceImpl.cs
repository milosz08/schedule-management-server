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

namespace ScheduleManagement.Api.Network.Cathedral;

public class CathedralServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	IMapper mapper,
	ILogger<CathedralServiceImpl> logger)
	: AbstractExtendedCrudService(dbContext, passwordHasher), ICathedralService
{
	public async Task<CathedralResponseDto> CreateCathedral(CathedralRequestDto dto)
	{
		var findDepartment = await dbContext.Departments
			.FirstOrDefaultAsync(d => dto.DepartmentName.Equals(d.Name, StringComparison.OrdinalIgnoreCase));
		if (findDepartment == null)
		{
			throw new RestApiException("Nie znaleziono wydziału z podanym id", HttpStatusCode.NotFound);
		}
		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c =>
				(c.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
				 c.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase)) &&
				c.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));

		if (findCathedral != null)
		{
			throw new RestApiException("Podana katedra istnieje już w wybranej jednostce.",
				HttpStatusCode.ExpectationFailed);
		}
		var cathedral = new Entity.Cathedral
		{
			Name = dto.Name,
			Alias = dto.Alias,
			DepartmentId = findDepartment.Id
		};
		await dbContext.AddAsync(cathedral);
		await dbContext.SaveChangesAsync();

		var resDto = mapper.Map<CathedralResponseDto>(cathedral);

		logger.LogInformation("Successfully created cathedra: {}", resDto);
		return resDto;
	}

	public async Task<CathedralResponseDto> UpdateCathedral(CathedralRequestDto dto, long cathId)
	{
		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.Id == cathId);
		if (findCathedral == null)
		{
			throw new RestApiException("Nie znaleziono katedry z podanym id", HttpStatusCode.NotFound);
		}
		if (findCathedral.Name == dto.Name && findCathedral.Alias == dto.Alias)
		{
			throw new RestApiException("Należy wprowadzić wartości różne od poprzednich.",
				HttpStatusCode.ExpectationFailed);
		}
		findCathedral.Name = dto.Name;
		findCathedral.Alias = dto.Alias;
		await dbContext.SaveChangesAsync();

		var resDto = mapper.Map<CathedralResponseDto>(findCathedral);

		logger.LogInformation("Successfully updated cathedra with ID: {} and data: {}", cathId, resDto);
		return resDto;
	}

	public SearchQueryResponseDto GetAllCathedralsBasedDepartmentName(string? deptName, string? cathName)
	{
		deptName ??= string.Empty;
		cathName ??= string.Empty;

		var findAllCathedralsNames = dbContext.Cathedrals
			.Include(c => c.Department)
			.Where(c => deptName.Equals(c.Department.Name, StringComparison.OrdinalIgnoreCase) &&
			            c.Name.Contains(cathName, StringComparison.OrdinalIgnoreCase))
			.Select(c => c.Name)
			.ToList();
		findAllCathedralsNames.Sort();

		if (findAllCathedralsNames.Count > 0)
		{
			return new SearchQueryResponseDto(findAllCathedralsNames);
		}
		var findAllCathedrals = dbContext.Cathedrals
			.Include(c => c.Department)
			.Where(c => deptName.Equals(c.Department.Name, StringComparison.OrdinalIgnoreCase))
			.Select(c => c.Name)
			.ToList();

		findAllCathedrals.Sort();
		return new SearchQueryResponseDto(findAllCathedrals);
	}

	public PaginationResponseDto<CathedralQueryResponseDto> GetAllCathedrals(SearchQueryRequestDto searchQuery)
	{
		var cathedralsBaseQuery = dbContext.Cathedrals
			.Include(c => c.Department)
			.Where(c => searchQuery.SearchPhrase == null ||
			            c.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Entity.Cathedral, object>>>
			{
				{ nameof(Entity.Cathedral.Id), c => c.Id },
				{ nameof(Entity.Cathedral.Name), c => c.Name },
				{ nameof(Entity.Cathedral.Alias), c => c.Alias },
				{ "DepartmentName", c => c.Department.Name },
				{ "DepartmentAlias", c => c.Department.Alias }
			}, searchQuery, ref cathedralsBaseQuery);
		}
		var allCathedrals = mapper.Map<List<CathedralQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(cathedralsBaseQuery, searchQuery));

		return new PaginationResponseDto<CathedralQueryResponseDto>(
			allCathedrals, cathedralsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public List<NameIdElementDto> GetAllCathedralsScheduleBaseDept(long deptId)
	{
		var cathedralsBaseDept = dbContext.Cathedrals
			.Include(s => s.Department)
			.Where(s => s.Department.Id == deptId)
			.ToList();
		cathedralsBaseDept.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		return cathedralsBaseDept
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<CathedralEditResDto> GetCathedralDetails(long cathId)
	{
		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.Id == cathId);
		if (findCathedral == null)
		{
			throw new RestApiException("Nie znaleziono katedry z podanym numerem id.", HttpStatusCode.NotFound);
		}
		return mapper.Map<CathedralEditResDto>(findCathedral);
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora",
				HttpStatusCode.Forbidden);
		}
		var nonRemovableCaths = dbContext.Cathedrals.Where(c => !c.IsRemovable).Select(c => c.Id);
		var filteredDeletedCathedrals = items.Ids.Where(id => !nonRemovableCaths.Contains(id)).ToArray();

		var removeMessage = "Nie usunięto żadnej katedry.";
		if (filteredDeletedCathedrals.Length != 0)
		{
			var toRemove = await dbContext.Cathedrals
				.Where(c => filteredDeletedCathedrals.Any(id => id == c.Id))
				.ToListAsync();
			dbContext.Cathedrals.RemoveRange(toRemove);
			await dbContext.SaveChangesAsync();

			removeMessage = $"Pomyślnie usunięto wybrane katedry. Liczba usuniętych katedr: {toRemove.Count}.";
			logger.LogInformation("Successfully deleted: {} cathedrals", toRemove.Count);
		}
		return new MessageContentResDto
		{
			Message = removeMessage
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora",
				HttpStatusCode.Forbidden);
		}
		var findAllRemovingCathedrals = dbContext.Cathedrals.Where(c => c.IsRemovable);
		if (findAllRemovingCathedrals.Any())
		{
			dbContext.Cathedrals.RemoveRange(findAllRemovingCathedrals);
			await dbContext.SaveChangesAsync();

			logger.LogInformation("Successfully deleted: {} cathedrals", findAllRemovingCathedrals.Count());
		}
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie katedry."
		};
	}
}

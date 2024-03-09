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

namespace ScheduleManagement.Api.Network.StudyRoom;

public class StudyRoomServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<StudyRoomServiceImpl> logger,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IStudyRoomService
{
	public async Task<StudyRoomResponseDto> CreateStudyRoom(StudyRoomRequestDto dto)
	{
		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.AsSingleQuery()
			.FirstOrDefaultAsync(c =>
				c.Name.Equals(dto.CathedralName, StringComparison.OrdinalIgnoreCase) &&
				c.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));

		if (findCathedral == null)
		{
			throw new RestApiException("Nie znaleziono wydziału/katedry z podaną nazwą.", HttpStatusCode.NotFound);
		}
		var findRoomType = await dbContext.RoomTypes
			.FirstOrDefaultAsync(r => string
				.Equals(r.Name + " (" + r.Alias + ")", dto.RoomTypeName, StringComparison.OrdinalIgnoreCase));
		if (findRoomType == null)
		{
			throw new RestApiException("Nie znaleziono typu sali z podanym aliasem.", HttpStatusCode.NotFound);
		}
		var findExistingRoom = await dbContext.StudyRooms
			.Include(r => r.Department)
			.Include(r => r.Cathedral)
			.Include(r => r.RoomType)
			.FirstOrDefaultAsync(r =>
				r.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) &&
				r.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase) &&
				r.Cathedral.Name.Equals(dto.CathedralName, StringComparison.OrdinalIgnoreCase));

		if (findExistingRoom != null)
		{
			throw new RestApiException("Podana sala istnieje już w wybranej jednostce",
				HttpStatusCode.ExpectationFailed);
		}
		var createStudyRoom = new Entity.StudyRoom
		{
			Name = dto.Name.ToUpper(),
			Description = dto.Description,
			Capacity = dto.Capacity,
			CathedralId = findCathedral.Id,
			DepartmentId = findCathedral.Department.Id,
			RoomTypeId = findRoomType.Id
		};
		await dbContext.AddAsync(createStudyRoom);
		await dbContext.SaveChangesAsync();

		var resDto = mapper.Map<StudyRoomResponseDto>(createStudyRoom);

		logger.LogInformation("Successfully created new study room: {}", resDto);
		return resDto;
	}

	public async Task<StudyRoomResponseDto> UpdateStudyRoom(StudyRoomRequestDto dto, long roomId)
	{
		var findStudyRoom = await dbContext.StudyRooms
			.Include(r => r.RoomType)
			.Include(r => r.Cathedral)
			.Include(r => r.Department)
			.FirstOrDefaultAsync(r => r.Id == roomId);

		if (findStudyRoom == null)
		{
			throw new RestApiException("Nie znaleziono szukanej sali zajęciowej.", HttpStatusCode.NotFound);
		}
		var roomTypeWithAlias = $"{findStudyRoom.RoomType.Name} ({findStudyRoom.RoomType.Alias})";

		if (dto.Name.Equals(findStudyRoom.Name) && dto.Description.Equals(findStudyRoom.Description) &&
		    dto.Capacity.Equals(findStudyRoom.Capacity) && dto.RoomTypeName.Equals(roomTypeWithAlias))
		{
			throw new RestApiException("Należy wprowadzić wartości różne od poprzednich.",
				HttpStatusCode.ExpectationFailed);
		}
		var findRoomType = await dbContext.RoomTypes
			.FirstOrDefaultAsync(t => string.Equals(t.Name + " (" + t.Alias + ")", dto.RoomTypeName));
		if (findRoomType == null)
		{
			throw new RestApiException("Nie znaleziono typu sali na podstawie nazwy.", HttpStatusCode.NotFound);
		}
		findStudyRoom.Name = dto.Name;
		findStudyRoom.Description = dto.Description;
		findStudyRoom.Capacity = dto.Capacity;
		findStudyRoom.RoomType.Id = findRoomType.Id;

		await dbContext.SaveChangesAsync();
		var resDto = mapper.Map<StudyRoomResponseDto>(findStudyRoom);

		logger.LogInformation("Successfully updated study room with ID: {}: {}", roomId, resDto);
		return resDto;
	}

	public PaginationResponseDto<StudyRoomQueryResponseDto> GetAllStudyRooms(SearchQueryRequestDto searchQuery)
	{
		var studyRoomsBaseQuery = dbContext.StudyRooms
			.Include(r => r.Department)
			.Include(r => r.Cathedral)
			.Include(r => r.RoomType)
			.Where(r => searchQuery.SearchPhrase == null ||
			            r.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Entity.StudyRoom, object>>>
			{
				{ nameof(Entity.StudyRoom.Id), r => r.Id },
				{ nameof(Entity.StudyRoom.Name), r => r.Name },
				{ nameof(Entity.StudyRoom.Capacity), r => r.Capacity },
				{ "DepartmentAlias", r => r.Department.Alias },
				{ "CathedralAlias", r => r.Cathedral.Alias },
				{ "RoomTypeAlias", r => r.RoomType.Alias }
			}, searchQuery, ref studyRoomsBaseQuery);
		}
		var allDepts = mapper.Map<List<StudyRoomQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(studyRoomsBaseQuery, searchQuery));

		return new PaginationResponseDto<StudyRoomQueryResponseDto>(
			allDepts, studyRoomsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public async Task<List<NameIdElementDto>> GetAllStudyRoomsScheduleBaseCath(long deptId, long cathId)
	{
		var studyRoomBaseDeptAndCath = await dbContext.StudyRooms
			.Include(r => r.Department)
			.Include(r => r.Cathedral)
			.Where(r => r.Department.Id == deptId && r.Cathedral.Id == cathId)
			.ToListAsync();

		studyRoomBaseDeptAndCath.Sort((first, second) =>
			string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		return studyRoomBaseDeptAndCath
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<List<NameIdElementDto>> GetAllStudyRoomsScheduleBaseDeptName(long deptId)
	{
		var studyRoomBaseDeptAndCath = await dbContext.StudyRooms
			.Include(r => r.Department)
			.Where(r => r.Department.Id == deptId)
			.ToListAsync();

		studyRoomBaseDeptAndCath.Sort((first, second) =>
			string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		return studyRoomBaseDeptAndCath
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<StudyRoomEditResDto> GetStudyRoomDetails(long roomId)
	{
		var findStudyRoom = await dbContext.StudyRooms
			.Include(r => r.RoomType)
			.Include(r => r.Cathedral)
			.Include(r => r.Department)
			.FirstOrDefaultAsync(r => r.Id == roomId);

		if (findStudyRoom == null)
		{
			throw new RestApiException("Nie znaleziono szunakej sali.", HttpStatusCode.NotFound);
		}
		return mapper.Map<StudyRoomEditResDto>(findStudyRoom);
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var message = "Nie usunięto żadnej sali.";
		var toRemoved = dbContext.StudyRooms.Where(s => items.Ids.Any(id => id == s.Id));
		if (toRemoved.Any())
		{
			message = $"Pomyślnie usunięto wybrane sale. Liczba usuniętych sal: {toRemoved.Count()}.";
		}
		dbContext.StudyRooms.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study rooms", toRemoved.Count());
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
		var count = dbContext.StudyRooms.Count();

		dbContext.StudyRooms.RemoveRange();
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study rooms", count);
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie sale."
		};
	}
}

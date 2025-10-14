using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.S3;

namespace ScheduleManagement.Api.Network.MemoryStorage;

public class MemoryStorageServiceImpl(ApplicationDbContext dbContext, IMapper mapper) : IMemoryStorageService
{
	public async Task<List<SavedAccountDetailsResponseDto>> CheckSavedAccounts(SavedAccountsRequestDto reqDto)
	{
		var existingUserAccounts = await dbContext.Persons
			.Include(p => p.Role)
			.Where(p => reqDto.SavedAccountIds.Any(a => a == p.Id))
			.ToListAsync();

		return existingUserAccounts
			.Select(p =>
			{
				var mappedPerson = mapper.Map<SavedAccountDetailsResponseDto>(p);
				if (p.ProfileImageUuid != null)
					mappedPerson.ProfileImageUrl = $"{ApiConfig.S3.Url}/{S3Bucket.Profiles}/{p.ProfileImageUuid}.jpg";

				return mappedPerson;
			})
			.ToList();
	}
}

namespace ScheduleManagement.Api.Network.MemoryStorage;

public interface IMemoryStorageService
{
	Task<List<SavedAccountDetailsResponseDto>> CheckSavedAccounts(SavedAccountsRequestDto reqDto);
}

namespace ScheduleManagement.Api.Network.MemoryStorage;

public sealed class SavedAccountsRequestDto
{
	public List<long> SavedAccountIds { get; set; }
}

public sealed class SavedAccountDetailsResponseDto
{
	public long Id { get; set; }
	public string Login { get; set; }
	public string NameWithSurname { get; set; }
	public string Email { get; set; }
	public string Role { get; set; }
	public string? ProfileImageUrl { get; set; }
}

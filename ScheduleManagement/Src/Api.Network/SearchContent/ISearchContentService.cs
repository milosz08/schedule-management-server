namespace ScheduleManagement.Api.Network.SearchContent;

public interface ISearchContentService
{
	Task<List<SearchMassiveQueryResDto>> GetAllItemsFromServerQuery(SearchMassiveQueryReqDto query);
}

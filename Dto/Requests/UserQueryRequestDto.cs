using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Dto.Requests
{
    public class UserQueryRequestDto
    {
        public string SearchPhrase { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = ApplicationUtils._allowedPageSizes[0];
        public string SortBy { get; set; } = nameof(Person.Id);
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
    }

    public enum SortDirection
    {
        ASC, DES
    }
}
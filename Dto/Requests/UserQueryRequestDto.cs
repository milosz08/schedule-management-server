namespace asp_net_po_schedule_management_server.Dto.Requests
{
    public class UserQueryRequestDto
    {
        public string SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
    }

    public enum SortDirection
    {
        ASC, DES
    }
}
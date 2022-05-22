using System.Collections.Generic;
using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class SearchQueryRequestDto
    {
        public string SearchPhrase { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = ApplicationUtils._allowedPageSizes[0];
        public string SortBy { get; set; } = "Id";
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class SearchQueryResponseDto
    {
        public List<string> DataElements { get; set; }

        public SearchQueryResponseDto()
        {
            DataElements = new List<string>();
        }
        
        public SearchQueryResponseDto(List<string> dataElements)
        {
            DataElements = dataElements;
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public enum SortDirection
    {
        ASC, DES
    }
}
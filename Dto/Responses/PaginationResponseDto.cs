using System;
using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    // klasa odpowiedzialna za paginację (listowanie) rezultatów zapytań
    public class PaginationResponseDto<T>
    {
        public List<T> Elements { get; set; }
        public int TotalPagesCount { get; set; }
        public int ElementsFrom { get; set; }
        public int ElementsTo { get; set; }
        public int TotalElementsCount { get; set; }

        public PaginationResponseDto(List<T> elements, int totalCount, int pageSize, int pageNumber)
        {
            Elements = elements;
            TotalElementsCount = totalCount;
            ElementsFrom = pageSize * (pageNumber - 1) + 1;
            ElementsTo = ElementsFrom + pageSize - 1;
            TotalPagesCount = (int) Math.Ceiling(totalCount / (double) pageSize);
        }
    }
}
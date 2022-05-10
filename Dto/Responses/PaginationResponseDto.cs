using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    // klasa odpowiedzialna za paginację (listowanie) rezultatów zapytań
    public class PaginationResponseDto<T>
    {
        [JsonIgnore]
        private readonly int _maxPagesPlaceholder = 3;
        
        public List<T> Elements { get; set; }
        public int TotalPagesCount { get; set; }
        public int ElementsFrom { get; set; }
        public int ElementsTo { get; set; }
        public int TotalElementsCount { get; set; }
        public CurrentActivePages CurrentActivePages { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        public PaginationResponseDto(List<T> elements, int totalCount, int pageSize, int pageNumber)
        {
            Elements = elements;
            TotalElementsCount = totalCount;
            ElementsFrom = pageSize * (pageNumber - 1) + 1;
            ElementsTo = ElementsFrom + pageSize - 1;
            int totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
            TotalPagesCount = totalPages;
            CurrentActivePages = ComputedCurrentPageRange(pageNumber, totalPages);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        // metoda preparująca i zwracająca aktualnie wyświetlane przyciski wyboru stron w paginacji
        private CurrentActivePages ComputedCurrentPageRange(int currentPage, int maxPagesCount)
        {
            int[] pages = new int[_maxPagesPlaceholder];
            if (maxPagesCount < _maxPagesPlaceholder) {
                return new CurrentActivePages()
                {
                    ActivePages = new int[maxPagesCount].Select((_, i) => i + 1).ToArray(),
                };
            }
            if (currentPage >= 1 && currentPage < pages.Length) { // pierwsze 4
                return new CurrentActivePages()
                {
                    ActivePages = pages.Select((_, i) => i + 1).Append(4).ToArray(),
                    MaxEnabled = true,
                };
            }
            if (currentPage > maxPagesCount - (pages.Length - 1) && currentPage <= maxPagesCount) { // ostatnie 4
                return new CurrentActivePages()
                {
                    ActivePages = pages.Select((_, i) => maxPagesCount + (i - (pages.Length - 1))).ToArray(),
                    MinEnabled = true,
                };
            }
            return new CurrentActivePages() // cała reszta
            {
                ActivePages = new[] { currentPage - 1, currentPage, currentPage + 1 },
                MinEnabled = true,
                MaxEnabled = true,
            };
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public class CurrentActivePages
    {
        public int[] ActivePages { get; set; }
        public bool MinEnabled { get; set; }
        public bool MaxEnabled { get; set; }
    }
}
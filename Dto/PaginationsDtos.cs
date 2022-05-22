using System;

using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class CurrentActivePages
    {
        public int[] ActivePages { get; set; }
        public bool MinEnabled { get; set; }
        public bool MaxEnabled { get; set; }

        public CurrentActivePages(int[] activePages, bool minEnabled = false, bool maxEnabled = false)
        {
            ActivePages = activePages;
            MinEnabled = minEnabled;
            MaxEnabled = maxEnabled;
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class PaginationResponseDto<T>
    {
        [JsonIgnore] private readonly int _maxPagesPlaceholder = 3;

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

        /// <summary>
        /// Metoda preparująca i zwracająca aktualnie wyświetlane przyciski wyboru stron w paginacji.
        /// </summary>
        /// <param name="currentPage">aktualna strona</param>
        /// <param name="maxPagesCount">maksymalna ilość stron</param>
        /// <returns>aktywne strony</returns>
        private CurrentActivePages ComputedCurrentPageRange(int currentPage, int maxPagesCount)
        {
            int[] pages = new int[_maxPagesPlaceholder];
            if (maxPagesCount < _maxPagesPlaceholder) {
                return new CurrentActivePages(new int[maxPagesCount].Select((_, i) => i + 1).ToArray());
            }

            if (currentPage >= 1 && currentPage < pages.Length) {
                // pierwsze 4
                return new CurrentActivePages(pages.Select((_, i) => i + 1).Append(4).ToArray(), false, true);
            }

            if (currentPage > maxPagesCount - (pages.Length - 1) && currentPage <= maxPagesCount) {
                // ostatnie 4
                return new CurrentActivePages(pages
                    .Select((_, i) => maxPagesCount + (i - (pages.Length - 1))).ToArray(), true);
            }

            // cała reszta
            return new CurrentActivePages(new[] {currentPage - 1, currentPage, currentPage + 1}, true, true);
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class AvailablePaginationSizes
    {
        public List<int> AvailablePaginations { get; set; }

        public AvailablePaginationSizes(List<int> availablePaginations)
        {
            AvailablePaginations = availablePaginations;
        }
    }
}
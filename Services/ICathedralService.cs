using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface ICathedralService
    {
        Task<CreatedCathedralResponseDto> CreateCathedral(CreateCathedralRequestDto dto);
        Task<SearchQueryResponseDto> GetAllCathedralsBasedDepartmentName(string cathName, string deptName);
    }
}
using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface ICathedralService
    {
        Task<CathedralResponseDto> CreateCathedral(CathedralRequestDto dto);
        SearchQueryResponseDto GetAllCathedralsBasedDepartmentName(string cathName, string deptName);
        PaginationResponseDto<CathedralQueryResponseDto> GetAllCathedrals(SearchQueryRequestDto searchQuery);
        List<NameWithDbIdElement> GetAllCathedralsScheduleBaseDept(long deptId);
        Task<CathedralEditResDto> GetCathedralBaseDbId(long cathId);
        Task DeleteMassiveCathedrals(MassiveDeleteRequestDto cathedrals, UserCredentialsHeaderDto credentials);
        Task DeleteAllCathedrals(UserCredentialsHeaderDto credentials);
    }
}
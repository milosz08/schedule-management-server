using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IDepartmentsService
    {
        Task<DepartmentRequestResponseDto> CreateDepartment(DepartmentRequestResponseDto dto);
        SearchQueryResponseDto GetAllDepartmentsList(string deptQueryName);
        PaginationResponseDto<DepartmentQueryResponseDto> GetAllDepartments(SearchQueryRequestDto searchQuery);
        Task<List<NameWithDbIdElement>> GetAllDepartmentsSchedule();
        Task DeleteMassiveDepartments(MassiveDeleteRequestDto departments, UserCredentialsHeaderDto credentials);
        Task DeleteAllDepartments(UserCredentialsHeaderDto credentials);
    }
}
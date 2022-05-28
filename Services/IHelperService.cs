using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IHelperService
    {
        AvailablePaginationSizes GetAvailablePaginationTypes();
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudyTypes();
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudyDegreeTypes();
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableSemesters();
        Task<List<NameWithDbIdElement>> GetAvailableStudyDegreeBaseAllSpecs(long deptId);
        Task<List<NameWithDbIdElement>> GetAvailableSemBaseStudyGroups(long deptId, long studySpecId);
        Task<ConvertToNameWithIdResponseDto> ConvertNamesToIds(ConvertNamesToIdsRequestDto dto);
        Task<ConvertToNameWithIdResponseDto> ConvertIdsToNames(ConvertIdsToNamesRequestDto dto);
        Task<AvailableDataResponseDto<string>> GetAvailableSubjectTypes(string subjTypeName);
        Task<AvailableDataResponseDto<string>> GetAvailableRoomTypes();
        Task<AvailableDataResponseDto<string>> GetAvailableRoles();
    }
}
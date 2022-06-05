using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IStudySpecService
    {
        Task<IEnumerable<StudySpecResponseDto>> AddNewStudySpecialization(StudySpecRequestDto dto);
        Task<List<StudySpecResponseDto>> UpdateStudySpecialization(StudySpecRequestDto dto, long specId);
        SearchQueryResponseDto GetAllStudySpecializationsInDepartment(string specName, string deptName);
        PaginationResponseDto<StudySpecQueryResponseDto> GetAllStudySpecializations(SearchQueryRequestDto searchQuery);
        Task<List<NameWithDbIdElement>> GetAllStudySpecsScheduleBaseDept(long deptId, long degreeId);
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudySpecsBaseDept(string deptName);
        Task<StudySpecializationEditResDto> GetStudySpecializationBaseDbId(long specId);
        Task DeleteMassiveStudySpecs(MassiveDeleteRequestDto studySpecs, UserCredentialsHeaderDto credentials);
        Task DeleteAllStudySpecs(UserCredentialsHeaderDto credentials);
    }
}
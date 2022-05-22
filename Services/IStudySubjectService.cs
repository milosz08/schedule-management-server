using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IStudySubjectService
    {
        Task<CreateStudySubjectResponseDto> AddNewStudySubject(CreateStudySubjectRequestDto dto);
        PaginationResponseDto<StudySubjectQueryResponseDto> GetAllStudySubjects(SearchQueryRequestDto searchQuery);
        SearchQueryResponseDto GetAllStudySubjectsBaseDept(string subjcQuery, string deptQuery);
        Task DeleteMassiveSubjects(MassiveDeleteRequestDto subjects, UserCredentialsHeaderDto credentials);
        Task DeleteAllSubjects(UserCredentialsHeaderDto credentials);
    }
}
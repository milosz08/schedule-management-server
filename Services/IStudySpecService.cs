using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IStudySpecService
    {
        Task<IEnumerable<StudySpecResponseDto>> AddNewStudySpecialization(StudySpecRequestDto dto);
    }
}
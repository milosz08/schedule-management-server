using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    public sealed class AvailableStudyTypesResponseDto
    {
        public List<StudySingleTypeDto> StudyTypes { get; set; }
    }

    public sealed class StudySingleTypeDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
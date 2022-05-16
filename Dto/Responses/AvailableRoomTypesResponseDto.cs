using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    public sealed class AvailableRoomTypesResponseDto
    {
        public List<SingleRoomTypeDto> StudyRoomTypes { get; set; }
    }

    public sealed class SingleRoomTypeDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Services
{
    public interface ITimeManagementService
    {
        List<string> GetAllWeeksNameWithWeekNumberInCurrentYear();
    }
}
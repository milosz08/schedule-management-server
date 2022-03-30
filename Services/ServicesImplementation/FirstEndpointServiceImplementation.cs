using asp_net_po_schedule_management_server.DbConfig;

namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class FirstEndpointServiceImplementation : IFirstEndpointService
    {
        private readonly ApplicationDbContext _dbContext;

        public FirstEndpointServiceImplementation(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public string AdditionService(int first, int second)
        {
            return $"Your number is: {first + second}";
        }
    }
}
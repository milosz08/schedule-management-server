namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class FirstEndpointServiceImplementation : IFirstEndpointService
    {
        public string AdditionService(int first, int second)
        {
            return $"Your number is: {first + second}";
        }
    }
}
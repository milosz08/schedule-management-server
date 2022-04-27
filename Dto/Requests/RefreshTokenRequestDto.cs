namespace asp_net_po_schedule_management_server.Dto.Requests
{
    public sealed class RefreshTokenRequestDto
    {
        public string BearerToken { get; set; }
        
        public string RefreshBearerToken { get; set; }
    }
}
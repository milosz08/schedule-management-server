using System;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class RefreshTokenRequestDto
    {
        public string BearerToken { get; set; }
        public string RefreshBearerToken { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class RefreshTokenResponseDto
    {
        public string BearerToken { get; set; }
        public string RefreshBearerToken { get; set; }
        public DateTime TokenExpirationDate { get; set; }
        public double tokenRefreshInSeconds { get; set; }
    }
}
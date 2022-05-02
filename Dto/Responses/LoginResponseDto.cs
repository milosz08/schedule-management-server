using System;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    public sealed class LoginResponseDto
    {
        public string DictionaryHash { get; set; }
        
        public string BearerToken { get; set; }
        
        public string RefreshBearerToken { get; set; }
        
        public string Role { get; set; }
        
        public string NameWithSurname { get; set; }
        
        public string Login { get; set; }
        
        public string Email { get; set; }
        
        public DateTime TokenExpirationDate { get; set; }
        
        public double tokenRefreshInSeconds { get; set; }
        
        public bool FirstAccess { get; set; }
        
        public bool HasPicture { get; set; }
    }
}
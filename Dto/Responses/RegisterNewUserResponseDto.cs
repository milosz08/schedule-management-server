using asp_net_po_schedule_management_server.Dto.Shared;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    public sealed class RegisterNewUserResponseDto : DictionaryHashInjectionFragment
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string Nationality { get; set; }
        
        public string City { get; set; }
        
        public string Role { get; set; }
        
        public string Login { get; set; }
        
        public string Password { get; set; }
        
        public string EmailPassword { get; set; }
        
        public string Shortcut { get; set; }
        
        public string Email { get; set; }
        
        public bool FirstAccess { get; set; }
    }
}
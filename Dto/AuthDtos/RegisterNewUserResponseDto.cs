using System.Text.Json.Serialization;

using asp_net_po_schedule_management_server.Dto.Shared;


namespace asp_net_po_schedule_management_server.Dto.AuthDtos
{
    public sealed class RegisterNewUserResponseDto : ResponseDictionaryHashInjection
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("surname")]
        public string Surname { get; set; }
        
        [JsonPropertyName("nationality")]
        public string Nationality { get; set; }
        
        [JsonPropertyName("city")]
        public string City { get; set; }
        
        [JsonPropertyName("role")]
        public string Role { get; set; }
        
        [JsonPropertyName("login")]
        public string Login { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }
        
        [JsonPropertyName("shortcut")]
        public string Shortcut { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("firstAccess")]
        public bool FirstAccess { get; set; }
    }
}
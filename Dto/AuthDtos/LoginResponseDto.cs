using System.Text.Json.Serialization;


namespace asp_net_po_schedule_management_server.Dto.AuthDtos
{
    public sealed class LoginResponseDto
    {
        [JsonPropertyName("bearerToken")]
        public string BearerToken { get; set; }
        
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
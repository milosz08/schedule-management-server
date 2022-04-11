using System.Text.Json.Serialization;


namespace asp_net_po_schedule_management_server.Dto
{
    public class UserResponseDto : UserRequestDto
    {
        [JsonPropertyName("bearerToken")]
        public string BearerToken { get; set; }
    }
}
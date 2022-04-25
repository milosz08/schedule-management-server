using System.Text.Json.Serialization;


namespace asp_net_po_schedule_management_server.Dto.Responses
{
    public sealed class PseudoNoContentResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
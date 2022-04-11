using System.Text.Json.Serialization;

using System.Text.Json;


namespace asp_net_po_schedule_management_server.Dto
{
    public class ServerExceptionResponseDto
    {
        [JsonPropertyName("message")]
        public string ExceptionMessage { get; set; }
        
        [JsonPropertyName("errorCode")]
        public int ExceptionErrorCode { get; set; }
        
        [JsonPropertyName("serverTimestamp")]
        public string ServerTimestamp { get; set; }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
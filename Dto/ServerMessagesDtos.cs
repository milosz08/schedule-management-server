using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class PseudoNoContentResponseDto
    {
        public string Message { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class ServerExceptionResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string ServerTimestamp { get; set; }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
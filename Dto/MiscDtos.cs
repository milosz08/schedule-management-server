using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class MassiveDeleteRequestDto
    {
        public long[] ElementsIds { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class UserCredentialsHeaderDto
    {
        public string Login { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class AvailableDataResponseDto<T>
    {
        public List<T> DataElements { get; set; }
        
        public AvailableDataResponseDto(List<T> dataElements)
        {
            DataElements = dataElements;
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class NameWithDbIdElement
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public NameWithDbIdElement(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
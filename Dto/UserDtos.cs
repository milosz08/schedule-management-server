namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class UserResponseDto
    {
        public long Id { get; set; }
        public string NameWithSurname { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public bool IfRemovable { get; set; }
    }
}
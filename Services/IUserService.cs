using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IUserService
    {
        UserResponseDto AuthenticateUser(UserRequestDto user);
    }
}
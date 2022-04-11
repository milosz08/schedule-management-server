using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.DbConfig;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class UserServiceImplementation : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtAuthenticationManager _manager;
        
        public UserServiceImplementation(ApplicationDbContext dbContext, IJwtAuthenticationManager manager) {
            _dbContext = dbContext;
            _manager = manager;
        }
        
        public UserResponseDto AuthenticateUser(UserRequestDto user)
        {
            string token = _manager.BearerHandlingService(user.Login);
            
            UserResponseDto response = new UserResponseDto() {
                Login = user.Login,
                Password = user.Password,
                BearerToken = token
            };
            
            return response;
        }
    }
}
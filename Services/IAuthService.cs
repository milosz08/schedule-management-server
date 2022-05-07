﻿using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> UserLogin(LoginRequestDto user);
        Task<RefreshTokenResponseDto> UserRefreshToken(RefreshTokenRequestDto dto);
        Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user, string customPassword = "",
            AvailableRoles defRole = AvailableRoles.TEACHER);
    }
}
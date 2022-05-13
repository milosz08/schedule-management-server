using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.CustomDecorators;

using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IResetPasswordService _resetPasswordService;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public AuthController(IAuthService service, IResetPasswordService resetPasswordService)
        {
            _service = service;
            _resetPasswordService = resetPasswordService;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpPost(ApiEndpoints.LOGIN)]
        public async Task<ActionResult<LoginResponseDto>> UserLogin([FromBody] LoginRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UserLogin(user));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.REGISTER)]
        [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
        public async Task<ActionResult<RegisterNewUserResponseDto>> UserRegister([FromBody] RegisterNewUserRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.UserRegister(user, String.Empty));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpPost(ApiEndpoints.REFRESH_TOKEN)]
        public async Task<ActionResult<RefreshTokenResponseDto>> UserRefreshToken(RefreshTokenRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UserRefreshToken(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.CHANGE_PASSWORD)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> UserChangePassword(
            [FromQuery] string userId,
            [FromBody] ChangePasswordRequestDto form)
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.OK,
                await _resetPasswordService.UserChangePassword(form, userId, userLogin));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.RESET_PASSWORD)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> UserResetPassword(
            [FromBody] SetResetPasswordRequestDto dto)
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            Claim resetToken = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Rsa);
            return StatusCode((int) HttpStatusCode.OK,
                await _resetPasswordService.UserResetPassword(dto, resetToken, userLogin));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpPost(ApiEndpoints.SEND_RESET_PASSWORD_TOKEN)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> SendPasswordResetTokenViaEmail(
            [FromQuery] string userEmail)
        {
            return StatusCode((int) HttpStatusCode.OK,
                await _resetPasswordService.SendPasswordResetTokenViaEmail(userEmail));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpPost(ApiEndpoints.CONFIRM_RESET_TOKEN)]
        public async Task<ActionResult<SetNewPasswordViaEmailResponse>> ResetPasswordViaEmailToken(
            [FromQuery] string emailToken)
        {
            return StatusCode((int) HttpStatusCode.OK,
                await _resetPasswordService.ResetPasswordViaEmailToken(emailToken));
        }
    }
}
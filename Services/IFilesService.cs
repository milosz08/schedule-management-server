using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IFilesService
    {
        Task<(byte[], string)> UserGetCustomAvatar(string userId, Claim userLogin);
        Task<PseudoNoContentResponseDto> UserAddCustomAvatar(IFormFile image, Claim userLogin);
    }
}
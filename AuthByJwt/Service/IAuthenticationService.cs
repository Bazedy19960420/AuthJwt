using Microsoft.AspNetCore.Identity;
using static AuthByJwt.Models.Dtos;

namespace AuthByJwt.Service
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegisterDto user);
        Task<bool> ValidateUser(UserForAuthenticationDto user);
        Task<TokenDto> CreateToken(bool populateExp);

    }
}

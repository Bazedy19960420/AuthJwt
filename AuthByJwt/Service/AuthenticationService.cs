using AuthByJwt.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static AuthByJwt.Models.Dtos;

namespace AuthByJwt.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private User? _user;

        public AuthenticationService(IMapper mapper, UserManager<User> userManager, IConfiguration configuration)

        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }



        public async Task<IdentityResult> RegisterUser(Dtos.UserForRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userDto.Roles);

            }
            return result;
        }

        public async Task<bool> ValidateUser(Dtos.UserForAuthenticationDto user)
        {
            _user = await _userManager.FindByNameAsync(user.UserName);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, user.Password));
            return result;
        }
        public async Task<TokenDto> CreateToken(bool populateExp)
        {

            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOption = GenerateTokenOption(signingCredentials, claims);
            var refreshToken = generateRefreshToken();
            _user.RefreshToken = refreshToken;
            if (populateExp)
            {
                _user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(20);

            }
            await _userManager.UpdateAsync(_user);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOption);
            return new TokenDto(accessToken, refreshToken);
        }
        private string generateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpireToken(string token)
        {
            var jwtsettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsettings["Secret"])),
                ValidateLifetime = true,
                ValidIssuer = jwtsettings["ValidIssuer"],
                ValidAudience = jwtsettings["ValidAudience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken != null)
            {
                throw new SecurityTokenException("Token Is Invalid");
            }
            return principal;
        }

        private JwtSecurityToken GenerateTokenOption(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettigns = _configuration.GetSection("JwtSettings");
            var tokenOption = new JwtSecurityToken
            (
                issuer: jwtSettigns["ValidIssuer"],
                audience: jwtSettigns["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(3),
                signingCredentials: signingCredentials
            );
            return tokenOption;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,_user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;

        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        }


    }
}

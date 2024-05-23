using AuthByJwt.Service;
using Microsoft.AspNetCore.Mvc;
using static AuthByJwt.Models.Dtos;

namespace AuthByJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegisterDto registerDto)
        {

            var result = await _authenticationService.RegisterUser(registerDto);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.TryAddModelError(err.Code, err.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _authenticationService.ValidateUser(user))
                return Unauthorized();
            var tokenDto = _authenticationService.CreateToken(true);
            return Ok(tokenDto);

        }


    }
}


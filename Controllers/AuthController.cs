using BlazorAuthAPI.Models;
using BlazorAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuthAPI.Controllers
{
    [ApiController]
    [Route("api/")]
    public class AuthController(CryptographyService cryptographyService) : ControllerBase
    {
        readonly CryptographyService _cryptographyService = cryptographyService;

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (IsUserValid(request))
            {
                return Redirect($"/auth-callback/{_cryptographyService.Encrypt(new UserState()
                {
                    IsAuthenticated = true,
                    SessionToken = Guid.NewGuid().ToString(),
                    Username = request.Username
                        
                })}");
            }
            else
            {
                return Unauthorized(request);
            }
        }

        private static bool IsUserValid(AuthRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Username) &&
                   !string.IsNullOrWhiteSpace(request.Password) &&
                   request is { Username: "admin", Password: "123456" };
        }
    }
}

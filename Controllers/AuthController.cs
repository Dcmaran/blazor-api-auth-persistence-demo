using BlazorAuthAPI.Auth;
using BlazorAuthAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserState userState, CustomAuthStateProvider customAuthStateProvider) : ControllerBase
    {
        private readonly UserState _userState = userState;
        private readonly CustomAuthStateProvider _customAuthStateProvider = customAuthStateProvider;

        [HttpPost("login")]
        public IActionResult PerformAction([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (IsUserValid(request))
            {
                SetUserState(request.Username!);
                _customAuthStateProvider.GetAuthenticationStateAsync().Wait();
                _customAuthStateProvider.AuthenticateUser(request.Username!);
                return Redirect("/authorized");
            }
            else
            {
                ClearUserState();
                return Unauthorized(_userState);
            }
        }

        private static bool IsUserValid(AuthRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Username) &&
                   !string.IsNullOrWhiteSpace(request.Password) &&
                   request.Username == "admin" &&
                   request.Password == "123456";
        }

        private void SetUserState(string username)
        {
            _userState.Username = username;
            _userState.SessionID = Guid.NewGuid().ToString();
            _userState.IsAutenticated = true;
        }

        private void ClearUserState()
        {
            _userState.Username = null;
            _userState.SessionID = null;
            _userState.IsAutenticated = false;
        }
    }
}

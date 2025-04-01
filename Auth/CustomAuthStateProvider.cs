using BlazorAuthAPI.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAuthAPI.Auth
{
    public class CustomAuthStateProvider(UserState userState) : AuthenticationStateProvider
    {
        private readonly UserState _userState = userState;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity = new();

            if (_userState.IsAutenticated && _userState.Username is not null)
            {
                identity = new([new(ClaimTypes.Name, _userState.Username),], "apiauth_type");
            }

            ClaimsPrincipal user = new(identity);

            return Task.FromResult(new AuthenticationState(user));
        }

        public void AuthenticateUser(string userName)
        {
            _userState.Username = userName;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
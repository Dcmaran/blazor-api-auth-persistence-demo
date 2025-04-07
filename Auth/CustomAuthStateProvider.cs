using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAuthAPI.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private string? username;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user;

            if (string.IsNullOrWhiteSpace(username))
            {
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }
            else
            {
                ClaimsIdentity identity = new([new Claim(ClaimTypes.Name, username)], authenticationType: "apiauth_type");
                user = new ClaimsPrincipal(identity);
            }

            return Task.FromResult(new AuthenticationState(user));
        }

        public void AuthenticateUser(string userName)
        {
            username = userName;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}

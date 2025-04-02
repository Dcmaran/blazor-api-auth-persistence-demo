using BlazorAuthAPI.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAuthAPI.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private string? username;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity = new (
                [new Claim(ClaimTypes.Name, username ?? string.Empty)],
                "apiauth_type"
            );

            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        }

        public void AuthenticateUser(string userName)
        {
            username = userName;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
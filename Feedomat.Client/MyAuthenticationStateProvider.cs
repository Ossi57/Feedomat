using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Feedomat.Shared.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace Feedomat.Client
{
    public class MyAuthenticationStateProvider : AuthenticationStateProvider
    {

        private HttpClient httpClient;

        public MyAuthenticationStateProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public void StateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            UserModel currentUser = await httpClient.GetFromJsonAsync<UserModel>("user/getcurrentuser");
            AuthenticationState authenticationState;
            if (currentUser != null && currentUser.Username != null)
            {
                var claim = new Claim(ClaimTypes.Name, currentUser.Username);
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claim }, "serverAuth");
                //create claimsPrincipal               
                authenticationState = new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
            }
            else
            {
                authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            return authenticationState;
        }
    }
}

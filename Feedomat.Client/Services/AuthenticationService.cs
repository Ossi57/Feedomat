using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Feedomat.Shared.Model;
using Microsoft.Extensions.Configuration;

namespace Feedomat.Client.Services
{
    public interface IAuthenticationService
    {
        UserModel User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService HttpService { get; set; }
        private ILocalStorageService LocalStorageService { get; set; }
        private IConfiguration Configuration { get; set; }
        private HttpClient HttpClient { get; set; }
        private MyAuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AuthenticationService(IHttpService httpService, ILocalStorageService localStorageService, IConfiguration configuration, HttpClient httpClient, MyAuthenticationStateProvider authenticationStateProvider)
        {
            HttpService = httpService;
            LocalStorageService = localStorageService;
            Configuration = configuration;
            HttpClient = httpClient;
            AuthenticationStateProvider = authenticationStateProvider;

            Console.WriteLine("Authentication service started");
        }

        public UserModel User { get; private set; }

        public async Task Initialize()
        {
            User = await LocalStorageService.GetItem<UserModel>("user");
        }

        public async Task Login(string username, string password)
        {

            if (Configuration.GetValue<bool>("FakeBackend"))
            {
                User = await HttpService.Post<UserModel>("/users/authenticate", new { username, password });
                await LocalStorageService.SetItem("user", User);
            }
            else
            {
                var user = new UserModel { Username = username, Password = password };
                var response = await HttpClient.PostAsJsonAsync("user/authenticate", user);
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK && response.IsSuccessStatusCode)
                {
                    User = await response.Content.ReadFromJsonAsync<UserModel>();
                    AuthenticationStateProvider.StateChanged();
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task Logout()
        {
            User = null;
            await HttpClient.GetAsync("user/logout");
        }
    }
}

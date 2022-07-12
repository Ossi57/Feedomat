using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Feedomat.Client.Services;
using System.Net.Http;
using Feedomat.Client.Helper;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace Feedomat.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();

            builder.Services.AddMudServices();
            if (builder.Configuration.GetValue<bool>("FakeBackend"))
            {
                builder.Services.AddScoped(sp => new HttpClient(new FakeBackendHandler())
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                });
            }
            else
            {
                builder.Services.AddScoped(sp => new HttpClient()
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                });
            }
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<IFeedService, FeedService>();
            builder.Services.AddScoped<MyAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<MyAuthenticationStateProvider>());

            builder.Services.AddAuthorizationCore();

            var host = builder.Build();
            await host.Services.GetRequiredService<IAuthenticationService>().Initialize();

            await host.RunAsync();
        }
    }
}

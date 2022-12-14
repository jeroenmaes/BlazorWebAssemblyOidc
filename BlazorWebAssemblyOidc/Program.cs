using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorWebAssemblyOidc
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            string backendApi = builder.Configuration["BackendApi"];

            builder.Services.AddScoped<CustomAuthenticationMessageHandler>();
                        
            builder.Services.AddHttpClient("secured-api", opt => opt.BaseAddress = new Uri(backendApi))
                .AddHttpMessageHandler<CustomAuthenticationMessageHandler>();
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("secured-api"));

            builder.Services.AddHttpClient("api", opt => opt.BaseAddress = new Uri(backendApi));
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

            builder.Services.AddOidcAuthentication(opt =>
            {                
                opt.ProviderOptions.Authority = builder.Configuration["security:authority"];
                opt.ProviderOptions.ClientId = builder.Configuration["security:clientid"];
                opt.ProviderOptions.ResponseType = "code";
                opt.ProviderOptions.DefaultScopes.Add("api");
                opt.ProviderOptions.DefaultScopes.Add("email");
                opt.ProviderOptions.DefaultScopes.Add("profile");
            });

            await builder.Build().RunAsync();
        }
    }

    public class CustomAuthenticationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthenticationMessageHandler(IAccessTokenProvider provider, NavigationManager nav, IConfiguration config) : base(provider, nav)
        {
            string backendApi = config["BackendApi"];
            ConfigureHandler(new string[] { backendApi });
        }
    }
}

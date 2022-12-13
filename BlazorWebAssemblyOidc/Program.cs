using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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

            builder.Services.AddScoped<CustomAuthenticationMessageHandler>();

            string authorityUrl = builder.Configuration["security:authority"];

            builder.Services.AddHttpClient("secured-api", opt => opt.BaseAddress = new Uri(authorityUrl))
                .AddHttpMessageHandler<CustomAuthenticationMessageHandler>();

            builder.Services.AddHttpClient("api");

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
        public CustomAuthenticationMessageHandler(IAccessTokenProvider provider, NavigationManager nav) : base(provider, nav)
        {
            ConfigureHandler(new string[] { "http://localhost:8080/realms/master/", "http://localhost:5128/" });
        }
    }
}

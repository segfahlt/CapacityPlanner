using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http;
using CapacityPlanner.Client.Services;

namespace CapacityPlanner.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddMudServices();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<LookupService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<SkillService>();
            builder.Services.AddScoped<PlatformApiService>();

            await builder.Build().RunAsync();
        }
    }
}

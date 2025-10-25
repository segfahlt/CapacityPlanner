using MudBlazor.Services;
using CapacityPlanner.Client.Pages;
using CapacityPlanner.Components;
using CapacityPlanner.Api;

namespace CapacityPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMudServices();
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

			// API host services only
			builder.Services.AddCapacityPlannerServer(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            var app = builder.Build();

            // Pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAntiforgery();
			app.MapStaticAssets();
			app.MapRazorComponents<App>()
				.AddInteractiveWebAssemblyRenderMode()
				.AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

			app.MapCapacityPlannerApi();

            app.Run();
        }
    }
}

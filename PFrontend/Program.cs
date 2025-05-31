using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace PFrontend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7185/") });
            builder.Services.AddScoped<ExpedienteService>();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-GT");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-GT");



            await builder.Build().RunAsync();
        }
    }
}

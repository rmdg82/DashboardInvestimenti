using Blazored.SessionStorage;
using DashboardInvestimenti.Contracts;
using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DashboardInvestimenti
{
    public class Program
    {
        private static async Task DebugDelayAsync()
        {
#if DEBUG
            await Task.Delay(5000);
#endif
        }

        public static async Task Main(string[] args)
        {
            await DebugDelayAsync();
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            builder.Services.AddMudServices();

            builder.Services.AddBlazoredSessionStorage();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<IExcelReader<ExcelModel>, ExcelReader>();

            await builder.Build().RunAsync();
        }
    }
}
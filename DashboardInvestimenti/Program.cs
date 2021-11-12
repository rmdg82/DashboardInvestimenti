using Blazored.SessionStorage;
using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services.Implementations;
using DashboardInvestimenti.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace DashboardInvestimenti
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            builder.Services.AddMudServices();

            builder.Services.AddBlazoredSessionStorage();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<IExcelReader<ExcelModel>, ExcelReader>();
            builder.Services.AddTransient<IFinancialCalculator, FinancialCalculator>();

            await builder.Build().RunAsync();
        }
    }
}
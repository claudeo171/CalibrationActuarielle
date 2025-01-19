using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlineCalibrator.Client;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using OnlineCalibrator.SharedPages;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebWorkers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddSingleton<DonneeContainer>();
builder.Services.AddSingleton<IMLService, MLService>();
//for multithreading
builder.Services.AddBlazorJSRuntime();
builder.Services.AddWebWorkerService();
await builder.Build().RunAsync();

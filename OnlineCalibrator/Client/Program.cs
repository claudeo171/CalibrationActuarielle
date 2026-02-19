using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using OnlineCalibrator.SharedPages;
using OnlineCalibrator.SharedPages.Component;
using pax.BlazorChartJs;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebWorkers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Tavenem.DataStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
EnvironementCalibration.EstMAUI = false;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<DonneeContainer>();
var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
options.TypeInfoResolverChain.Add(ItemContext.Default.WithAddedModifier(static typeInfo =>
{
    if (typeInfo.Type == typeof(IIdItem))
    {
        typeInfo.PolymorphismOptions ??= new JsonPolymorphismOptions
        {
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        };
        typeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(SavedData)));
    }
}));
builder.Services.AddIndexedDbService();
builder.Services.AddIndexedDb(
    "saveData",
    ["valueStore"],
    2,
    options, key: "id");
builder.Services.AddSingleton<IMLService, MLService>();
/*BlazorWebWorker*/
builder.Services.AddBlazorJSRuntime();
builder.Services.AddWebWorkerService(WebWorkerService =>
{
    WebWorkerService.TaskPool.MaxPoolSize = WebWorkerService.MaxWorkerCount;
});

// Other misc. services
builder.Services.AddSingleton<IGrosCalculService, GrosCalculService>();
//SUite
builder.Services.AddChartJs(options =>
{
    // default
    options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
    options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
});
//for multithreading
await builder.Build().BlazorJSRunAsync();

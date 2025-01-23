using Blazored.SessionStorage;
using Microsoft.Extensions.Logging;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using SpawnDev.BlazorJS.WebWorkers;
using SpawnDev.BlazorJS;
using pax.BlazorChartJs;
using OnlineCalibrator.SharedPages;

namespace OnlineCalibrator.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            EnvironementCalibration.EstMAUI = true;
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddSingleton<DonneeContainer>();
            builder.Services.AddSingleton<IMLService, MLService>();
            //for multithreading
            builder.Services.AddBlazorJSRuntime();
            builder.Services.AddWebWorkerService();
            builder.Services.AddChartJs(options =>
            {
                // default
                options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
                options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
                options.ChartJsPluginAnnotation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-annotation";
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

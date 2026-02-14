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
            /*BlazorWebWorker*/
            builder.Services.AddBlazorJSRuntime();
            builder.Services.AddWebWorkerService(webWorkerService =>
            {
                /*
                // Optionally configure the WebWorkerService service before it is used
                // Default WebWorkerService.TaskPool settings: PoolSize = 0, MaxPoolSize = 1, AutoGrow = true
                // Below sets TaskPool max size to 2. By default the TaskPool size will grow as needed up to the max pool size.
                // Setting max pool size to -1 will set it to the value of navigator.hardwareConcurrency
                webWorkerService.TaskPool.MaxPoolSize = 2;
                // Below is telling the WebWorkerService TaskPool to set the initial size to 2 if running in a Window scope and 0 otherwise
                // This starts up 2 WebWorkers to handle TaskPool tasks as needed
                webWorkerService.TaskPool.PoolSize = webWorkerService.GlobalScope == GlobalScope.Window ? 2 : 0;
                */
            });

            // Other misc. services
            builder.Services.AddSingleton<IGrosCalculService, GrosCalculService>();
            builder.Services.AddChartJs(options =>
            {
                // default
                options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
                options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
                options.ChartJsPluginAnnotation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-annotation";
                options.ChartJsMatrixPlugin = "https://cdn.jsdelivr.net/npm/chartjs-chart-matrix@2";
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

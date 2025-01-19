using Blazored.SessionStorage;
using Microsoft.Extensions.Logging;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using SpawnDev.BlazorJS.WebWorkers;
using SpawnDev.BlazorJS;

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

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddSingleton<DonneeContainer>();
            builder.Services.AddSingleton<IMLService, MLService>();
            //for multithreading
            builder.Services.AddBlazorJSRuntime();
            builder.Services.AddWebWorkerService();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

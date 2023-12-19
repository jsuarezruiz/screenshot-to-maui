using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ScreenshotToMaui.Services;
using ScreenshotToMaui.ViewModels;
using ScreenshotToMaui.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace ScreenshotToMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<AlertService>();
            builder.Services.AddSingleton<OpenAIVisionService>();
            builder.Services.AddSingleton<OpenAIDalle3Service>();
            builder.Services.AddSingleton<MainView>();
            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddTransientPopup<AboutView, AboutViewModel>();
            builder.Services.AddTransientPopup<FaqView, FaqViewModel>();
            builder.Services.AddTransientPopup<SettingsView, SettingsViewModel>();

            return builder.Build();
        }
    }
}

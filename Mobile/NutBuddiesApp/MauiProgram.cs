using Microsoft.Extensions.Logging;

namespace NutBuddiesApp;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialicons.ttf", "MaterialIcons");
            });
        builder.Services.AddSingleton<NutBuddiesApp.Services.ApiClient>();
        builder.Services.AddSingleton<NutBuddiesApp.Services.CartService>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        NutBuddiesApp.Helpers.ServiceHelper.Services = app.Services;
        return app;
    }
}
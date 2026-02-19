namespace NutBuddiesApp.Helpers;

public static class ServiceHelper
{
    public static IServiceProvider Services { get; set; } = default!;

    public static T Get<T>() where T : notnull =>
        Services.GetRequiredService<T>();
}
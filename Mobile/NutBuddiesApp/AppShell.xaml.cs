using NutBuddiesApp.Views;

namespace NutBuddiesApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        _ = GoToAsync("//start");
        Routing.RegisterRoute(nameof(ProductDetailsPage), typeof(ProductDetailsPage));
        Routing.RegisterRoute(nameof(AddReviewPage), typeof(AddReviewPage));

    }
}
namespace NutBuddiesApp.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        // merge direct pe tab-ul Products
        await Shell.Current.GoToAsync("//products");
    }
}
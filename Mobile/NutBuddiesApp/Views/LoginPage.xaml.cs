using NutBuddiesApp.Services;

namespace NutBuddiesApp.Views;

public partial class LoginPage
{
    private readonly ApiClient _api = new();

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await UpdateUiAsync();
    }

    private async Task UpdateUiAsync()
    {
        var token = await SecureStorage.GetAsync("jwt");
        var email = await SecureStorage.GetAsync("email");

        if (!string.IsNullOrWhiteSpace(token))
        {
            LoggedOutView.IsVisible = false;
            LoggedInView.IsVisible = true;

            HelloLabel.Text = $"Hello, {email ?? "user"}! 👋";
        }
        else
        {
            LoggedOutView.IsVisible = true;
            LoggedInView.IsVisible = false;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = "";

        var email = EmailEntry.Text?.Trim() ?? "";
        var pass = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
        {
            ErrorLabel.Text = "Email and password required.";
            return;
        }

        try
        {
            var token = await _api.LoginAsync(email, pass);

            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorLabel.Text = "Invalid credentials.";
                return;
            }

            await SecureStorage.SetAsync("jwt", token);
            await SecureStorage.SetAsync("email", email);

            EmailEntry.Text = "";
            PasswordEntry.Text = "";

            await UpdateUiAsync();
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        SecureStorage.Remove("jwt");
        SecureStorage.Remove("email");

        await UpdateUiAsync();
    }
}
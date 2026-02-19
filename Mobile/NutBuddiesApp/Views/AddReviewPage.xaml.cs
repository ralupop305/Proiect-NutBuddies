using NutBuddiesApp.Helpers;
using NutBuddiesApp.Services;

namespace NutBuddiesApp.Views;

[QueryProperty(nameof(ProductId), "productId")]
public partial class AddReviewPage
{
    private readonly ApiClient _api;
    private int _productId;

    public string ProductId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _productId = id;
                ProductLabel.Text = $"Product #{_productId}";
            }
        }
    }

    public AddReviewPage()
    {
        InitializeComponent();
        _api = ServiceHelper.Get<ApiClient>();

        RatingStepper.Value = 5;
        RatingLabel.Text = "Rating: 5";
    }

    private void OnRatingChanged(object sender, ValueChangedEventArgs e)
    {
        RatingLabel.Text = $"Rating: {(int)e.NewValue}";
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = "";

        var rating = (int)RatingStepper.Value;
        var comment = CommentEditor.Text?.Trim() ?? "";

        if (_productId <= 0)
        {
            ErrorLabel.Text = "Invalid product.";
            return;
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            ErrorLabel.Text = "Please write a comment.";
            return;
        }

        // dacă endpoint-ul e [Authorize], trebuie token
        var token = await SecureStorage.GetAsync("jwt");
        if (string.IsNullOrWhiteSpace(token))
        {
            await DisplayAlert("Login required", "Please login to add a review.", "OK");
            return;
        }

        _api.SetBearer(token);

        var ok = await _api.AddReviewAsync(_productId, rating, comment);

        if (!ok)
        {
            await DisplayAlert("Error", "Could not save review.", "OK");
            return;
        }

        await DisplayAlert("Thanks! ✅", "Review saved.", "OK");
        await Shell.Current.GoToAsync(".."); // back
    }
}
using NutBuddiesApp.Helpers;
using NutBuddiesApp.Models;
using NutBuddiesApp.Services;

namespace NutBuddiesApp.Views;

public partial class ProductsPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly CartService _cart;

    public ProductsPage()
    {
        InitializeComponent();
        _api = new ApiClient();
        _cart = ServiceHelper.Get<CartService>();
    }

    private async void OnLoadClicked(object sender, EventArgs e)
    {
        try
        {
            ErrorLabel.Text = "";
            var items = await _api.GetProductsAsync();
            ProductsList.ItemsSource = items;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
        }
    }
    
    private async void OnDetailsClicked(object sender, EventArgs e)
    {
        // The list binds to ProductDto items, so navigate using that type
        if ((sender as Button)?.BindingContext is ProductDto p)
            await Shell.Current.GoToAsync($"{nameof(ProductDetailsPage)}?id={p.Id}");
    }
    
    private void OnAddToCartClicked(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is ProductDto p)
        {
            _cart.AddProduct(p);
            DisplayAlert("Cart", $"{p.Name} added ✅", "OK");
        }
    }
}
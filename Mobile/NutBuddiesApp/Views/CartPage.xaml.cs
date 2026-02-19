using NutBuddiesApp.Helpers;
using NutBuddiesApp.Models;
using NutBuddiesApp.Services;

namespace NutBuddiesApp.Views;

public partial class CartPage 
{
    private readonly CartService _cart;

    public CartPage()
    {
        InitializeComponent();
        _cart = ServiceHelper.Get<CartService>();

        CartList.ItemsSource = _cart.Items;
        // _cart.CartChanged += Refresh;
        // Refresh();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _cart.CartChanged += Refresh;
        Refresh();
    }


    private void Refresh()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TotalLabel.Text = $"Total: {_cart.Total} RON ({_cart.Count} items)";
            EmptyLabel.IsVisible = _cart.Items.Count == 0;
        });
    }

    private void OnPlusClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is CartItem item)
            _cart.Increase(item);
    }

    private void OnMinusClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is CartItem item)
            _cart.Decrease(item);
    }

    private void OnRemoveClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is CartItem item)
            _cart.Remove(item);
    }

    private void OnClearClicked(object sender, EventArgs e) => _cart.Clear();

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cart.CartChanged -= Refresh;
    }
    
    private async void OnPlaceOrderClicked(object sender, EventArgs e)
    {
        if (_cart.Items.Count == 0)
        {
            await DisplayAlert("Order", "Cart is empty.", "OK");
            return;
        }

        var name = NameEntry.Text?.Trim() ?? "";
        var phone = PhoneEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
        {
            await DisplayAlert("Missing info", "Please enter name and phone.", "OK");
            return;
        }

        var token = await SecureStorage.GetAsync("jwt");
        if (string.IsNullOrWhiteSpace(token))
        {
            await DisplayAlert("Login required", "Please login to place an order.", "OK");
            return;
        }

        var api = new ApiClient();
        api.SetBearer(token);

        var items = _cart.Items
            .Select(i => new ApiClient.PlaceOrderItemRequest(i.ProductId, i.Quantity))
            .ToList();

        var orderId = await api.PlaceOrderAsync(name, phone, items);

        if (orderId == null)
        {
            await DisplayAlert("Order failed", "Could not place order.", "OK");
            return;
        }

        _cart.Clear();
        NameEntry.Text = "";
        PhoneEntry.Text = "";

        await DisplayAlert("Success ✅", $"Order #{orderId} placed!", "OK");
    }

}
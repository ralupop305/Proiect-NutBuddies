using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NutBuddiesApp.Helpers;
using NutBuddiesApp.Models;
using NutBuddiesApp.Services;

namespace NutBuddiesApp.Views;

[QueryProperty(nameof(Id), "id")]
public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiClient _api;
    private int _id;

    public string Id
    {
        set
        {
            if (int.TryParse(value, out var parsed))
            {
                _id = parsed;
                _ = LoadAsync();
            }
        }
    }

    public ProductDetailsPage()
    {
        InitializeComponent();
        _api = ServiceHelper.Get<ApiClient>();
    }

    private async Task LoadAsync()
    {
        try
        {
            var p = await _api.GetProductByIdAsync(_id);
            if (p == null) return;

            NameLabel.Text = p.Name;
            CategoryLabel.Text = p.CategoryName ?? "";
            PriceLabel.Text = $"Price: {p.Price} RON";
            StockLabel.Text = $"Stock: {p.StockQty}";
            DescriptionLabel.Text = string.IsNullOrWhiteSpace(p.Description) ? "No description." : p.Description;

            SugarFreeLabel.Text = $"Sugar free: {(p.IsSugarFree ? "Yes" : "No")}";
            ProteinLabel.Text = $"Protein / 100g: {p.ProteinPer100g}";
            WeightLabel.Text = $"Weight: {p.WeightGrams} g";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
    private async void OnAddReviewClicked(object sender, EventArgs e)
    {
        // aici presupunem că ai id-ul produsului în _id (din query)
        await Shell.Current.GoToAsync($"{nameof(AddReviewPage)}?productId={_id}");
    }

}
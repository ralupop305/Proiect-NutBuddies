using System.Collections.ObjectModel;
using NutBuddiesApp.Models;
using NutBuddiesApp.Services;

namespace NutBuddiesApp.Services;

public class CartService
{
    public ObservableCollection<CartItem> Items { get; } = new();

    public event Action? CartChanged;

    public void AddProduct(ProductDto p)
    {
        var existing = Items.FirstOrDefault(x => x.ProductId == p.Id);
        if (existing != null)
            existing.Quantity++;
        else
            Items.Add(new CartItem
            {
                ProductId = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = 1
            });

        CartChanged?.Invoke();
    }

    public void Increase(CartItem item)
    {
        item.Quantity++;
        CartChanged?.Invoke();
    }

    public void Decrease(CartItem item)
    {
        if (item.Quantity > 1)
            item.Quantity--;
        else
            Items.Remove(item);

        CartChanged?.Invoke();
    }

    public void Remove(CartItem item)
    {
        Items.Remove(item);
        CartChanged?.Invoke();
    }

    public void Clear()
    {
        Items.Clear();
        CartChanged?.Invoke();
    }

    public decimal Total => Items.Sum(i => i.LineTotal);
    public int Count => Items.Sum(i => i.Quantity);
}
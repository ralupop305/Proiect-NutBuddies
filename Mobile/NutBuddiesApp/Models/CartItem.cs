using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace NutBuddiesApp.Models;

public class CartItem
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }


    public decimal LineTotal => Price * Quantity;
    
    private void OnPropertyChanged([CallerMemberName] string? name = null) 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
namespace NutBuddiesApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQty { get; set; }

    public bool IsSugarFree { get; set; }
    public double ProteinPer100g { get; set; }
    public int WeightGrams { get; set; }

    public int CategoryId { get; set; }
    public string? CategoryName { get; set; } // opțional (dacă îl trimiți din API)
}
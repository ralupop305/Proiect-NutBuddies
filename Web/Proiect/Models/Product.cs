using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Numele produsului este obligatoriu.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 caractere.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrierea poate avea maxim 500 caractere.")]
    public string? Description { get; set; }

    [Range(0.01, 9999.99, ErrorMessage = "Prețul trebuie să fie mai mare decât 0.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "Stocul nu poate fi negativ.")]
    public int StockQty { get; set; }

    public bool IsSugarFree { get; set; }

    [Range(0, 100, ErrorMessage = "Proteina per 100g trebuie să fie între 0 și 100.")]
    public double ProteinPer100g { get; set; }

    [Range(1, 2000, ErrorMessage = "Greutatea trebuie să fie între 1 și 2000g.")]
    public int WeightGrams { get; set; }

    // FK + navigare
    [Display(Name = "Categorie")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}
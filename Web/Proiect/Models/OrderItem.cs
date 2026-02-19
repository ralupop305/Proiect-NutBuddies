using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models;

public class OrderItem
{
    public int Id { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Range(0.01, 999999.99)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
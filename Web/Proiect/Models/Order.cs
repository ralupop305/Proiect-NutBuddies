using System.ComponentModel.DataAnnotations;

namespace Proiect.Models;

public class Order
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, Phone]
    public string Phone { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending/Paid/Shipped/Delivered/Cancelled

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
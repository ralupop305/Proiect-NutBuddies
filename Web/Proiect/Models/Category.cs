using System.ComponentModel.DataAnnotations;

namespace Proiect.Models;

public class Category
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Numele categoriei este obligatoriu.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 50 caractere.")]
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

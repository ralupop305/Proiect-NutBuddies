using System.ComponentModel.DataAnnotations;

namespace Proiect.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^[A-Z]+[a-zA-Z\s-]*$",
        ErrorMessage = "Prenumele trebuie sa inceapa cu majuscula (ex. Ana, Ana Maria sau Ana-Maria)")]
    [StringLength(30, MinimumLength = 3)]
    public string? FirstName { get; set; }

    [RegularExpression(@"^[A-Z]+[a-z\s]*$")]
    [StringLength(30, MinimumLength = 3)]
    public string? LastName { get; set; }

    [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$",
        ErrorMessage = "Telefonul trebuie sa fie de forma '0722-123-123' sau '0722.123.123' sau '0722 123 123'")]
    public string? Phone { get; set; }
}
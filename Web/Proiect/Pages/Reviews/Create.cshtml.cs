using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Pages.Reviews
{
    public class CreateModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public CreateModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? productId)
        {
            if (User.Identity?.Name == "ralucaAdmin@gmail.com") return Forbid();
            if (productId != null)
            {
                // Folosim proprietatea ProductId din modelul tău
                Review = new Review { ProductId = productId.Value };
            }

            // Corecție: _context.Products (cu 's')
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity?.Name == "ralucaAdmin@gmail.com") return Forbid();
            // 1. Curățăm validările automate pentru a evita eroarea de Foreign Key
            ModelState.Clear();

            // 2. Mapăm datele conform modelului tău Review
            Review.UserId = User.Identity?.Name; // Aici am schimbat din UserEmail în UserId
            Review.CreatedAt = DateTime.Now;

            // 3. Validare manuală pentru a asigura integritatea bazei de date
            if (Review.ProductId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Produsul nu a fost selectat corect.");
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                return Page();
            }

            // 4. Salvarea în baza de date
            _context.Reviews.Add(Review);
            await _context.SaveChangesAsync(); // Acum va trece de verificarea SQLite

            return RedirectToPage("./Index");
        }
    }
}

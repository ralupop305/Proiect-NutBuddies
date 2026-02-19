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
        [BindProperty]
        public Review NewReview { get; set; } = default!; // Am schimbat numele  pt a rezolva eroarea cu review
        public IActionResult OnGet(int? productId)
        {
            if (productId != null)
            {
                NewReview = new Review { ProductId = productId.Value };
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();

            // Folosim NewReview peste tot
            NewReview.UserId = User.Identity?.Name;
            NewReview.CreatedAt = DateTime.Now;

            if (NewReview.ProductId == 0)
            {
                ModelState.AddModelError("", "Eroare: Produsul nu a fost identificat.");
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                return Page();
            }

            _context.Reviews.Add(NewReview);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

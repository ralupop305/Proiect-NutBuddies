using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Pages.Reviews
{
    public class DeleteModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public DeleteModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Review Review { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(m => m.Id == id);

            if (review == null)
            {
                return NotFound();
            }
            else
            {
                Review = review;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                Review = review;
                _context.Reviews.Remove(Review);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

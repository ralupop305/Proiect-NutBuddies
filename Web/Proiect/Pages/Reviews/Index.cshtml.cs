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
    public class IndexModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public IndexModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        public IList<Review> Review { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Review = await _context.Reviews
                .Include(r => r.Product).ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Pages.OrderItems
{
    public class IndexModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public IndexModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        public IList<OrderItem> OrderItem { get;set; } = default!;

        public async Task OnGetAsync()
        {
            OrderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product).ToListAsync();
        }
    }
}

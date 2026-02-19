using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public DetailsModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Modificarea de aici aduce și produsele (OrderItems) și detaliile lor (Product)
            var order = await _context.Orders
                    .Include(o => o.OrderItems)      // Include lista de produse a comenzii
                        .ThenInclude(oi => oi.Product) // Pentru fiecare produs, include numele și prețul din tabelul Products
                    .FirstOrDefaultAsync(m => m.Id == id); 
            
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                Order = order;
            }
            return Page();
        }
    }
}

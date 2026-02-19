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
    public class IndexModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public IndexModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var currentUserEmail = User.Identity?.Name;

            // email-ul de admin
            if (currentUserEmail == "ralucaAdmin@gmail.com")
            {
                // Adminul vede absolut tot
                Order = await _context.Orders.ToListAsync();
            }
            else
            {
                // Clientul vede doar comenzile care îi aparțin (identificate prin email-ul din nume)
                Order = await _context.Orders
                    .Where(o => o.CustomerName.Contains(currentUserEmail))
                    .ToListAsync();
            }
        }
    }
}

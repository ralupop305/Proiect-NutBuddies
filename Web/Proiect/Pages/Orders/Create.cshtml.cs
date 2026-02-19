using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly Proiect.Data.NutBuddiesContext _context;

        public CreateModel(Proiect.Data.NutBuddiesContext context)
        {
            _context = context;
        }

        public SelectList ProductList { get; set; } = default!;

        [BindProperty]
        public Order Order { get; set; } = default!;

        // 1. listă pentru a putea adauga mai multe produse deodată
        [BindProperty]
        public List<OrderItem> SelectedItems { get; set; } = new List<OrderItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            string userEmail = User.Identity?.Name ?? "";
            string autoName = "";

            // Doar extragem numele din email pentru CustomerName
            if (!string.IsNullOrEmpty(userEmail) && userEmail.Contains("@"))
            {
                autoName = userEmail.Split('@')[0];
            }

            Order = new Order
            {
                CustomerName = autoName,
                Phone = "", // Lăsăm gol pentru ca utilizatorul să introducă telefonul manual
                Status = "Pending"
            };

            ProductList = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Curățăm erorile automate care pot bloca salvarea (cum ar fi cele de validare pentru relații)
            ModelState.Clear();

            // Dacă utilizatorul este logat, "lipim" email-ul de nume pentru a-l identifica ulterior
            if (User.Identity.IsAuthenticated)
            {
                Order.CustomerName = $"{Order.CustomerName} ({User.Identity.Name})";
            }

            // 2. Validare manuală minimă
            if (string.IsNullOrEmpty(Order.CustomerName))
            {
                ModelState.AddModelError("Order.CustomerName", "Numele clientului este obligatoriu.");
            }

            if (SelectedItems == null || !SelectedItems.Any(i => i.ProductId > 0))
            {
                ModelState.AddModelError(string.Empty, "Trebuie să adăugați cel puțin un produs în listă.");
            }

            if (!ModelState.IsValid)
            {
                // Reîncărcăm lista de produse dacă ne întoarcem la pagină din cauza unei erori
                ProductList = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
                return Page();
            }

            // 3. Pregătim obiectul pentru salvare
            Order.OrderItems = new List<OrderItem>();
            Order.CreatedAt = DateTime.Now; // Setăm data curentă automat

            foreach (var item in SelectedItems.Where(i => i.ProductId > 0))
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    // Verificăm stocul (Cerința 5)
                    if (product.StockQty < item.Quantity)
                    {
                        ModelState.AddModelError(string.Empty, $"Stoc insuficient pentru {product.Name}. Disponibil: {product.StockQty}");
                        ProductList = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
                        return Page();
                    }

                    // Scădem stocul și adăugăm produsul în comandă
                    product.StockQty -= item.Quantity;
                    Order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }
            }

            // 4. Salvarea efectivă în baza de date
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Dtos;
using Proiect.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace Proiect.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // trebuie login ca să plasezi comanda
public class OrdersController : ControllerBase
{
    private readonly NutBuddiesContext _db;

    public OrdersController(NutBuddiesContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName))
            return BadRequest("CustomerName is required.");

        if (string.IsNullOrWhiteSpace(dto.Phone))
            return BadRequest("Phone is required.");

        if (dto.Items == null || dto.Items.Count == 0)
            return BadRequest("Cart is empty.");

        // produse existente
        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != productIds.Count)
            return BadRequest("One or more products do not exist.");

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                return BadRequest("Quantity must be > 0.");
        }

        var order = new Order
        {
            CustomerName = dto.CustomerName.Trim(),
            Phone = dto.Phone.Trim(),
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        foreach (var item in dto.Items)
        {
            var p = products.First(x => x.Id == item.ProductId);

            // 1. VERIFICARE STOC: Dacă nu sunt destule produse, oprim totul
            if (p.StockQty < item.Quantity)
            {
                return BadRequest($"Stoc insuficient pentru {p.Name}. Disponibil: {p.StockQty}");
            }

            // 2. SCĂDERE STOC: Actualizăm cifra din tabelul Products
            p.StockQty -= item.Quantity;

            // 3. ADĂUGARE ÎN COMANDĂ: Folosim numele corect "OrderItems" din modelul tău
            order.OrderItems.Add(new OrderItem
            {
                ProductId = p.Id,
                Quantity = item.Quantity,
                UnitPrice = p.Price
            });
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return Ok(new { orderId = order.Id });
    }
}

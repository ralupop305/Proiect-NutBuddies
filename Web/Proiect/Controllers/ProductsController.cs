using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;

namespace Proiect.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly NutBuddiesContext _db;

    public ProductsController(NutBuddiesContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.StockQty,
                Category = p.Category != null ? p.Category.Name : null
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _db.Products
            .Include(x => x.Category)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.IsSugarFree,
                x.ProteinPer100g,
                x.WeightGrams,
                x.CategoryId,
                x.Price,
                x.StockQty,
                Category = x.Category != null ? x.Category.Name : null
            })
            .FirstOrDefaultAsync();

        return p == null ? NotFound() : Ok(p);
    }
}
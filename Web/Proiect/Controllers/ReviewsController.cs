using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Proiect.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly NutBuddiesContext _db;

    public ReviewsController(NutBuddiesContext db) => _db = db;

    public record ReviewDto(int ProductId, int Rating, string? Comment);

    // Public: toate review-urile pentru un produs
    [HttpGet("by-product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var items = await _db.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.ProductId,
                r.Rating,
                r.Comment,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    // User: creează review (doar logat)
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5.");

        if (dto.Comment?.Length > 300)
            return BadRequest("Comment max length is 300.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var review = new Review
        {
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            UserId = userId // dacă ai UserId în model
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return Ok(new { review.Id });
    }

    // User: update doar propriul review
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (review == null) return NotFound();

        // dacă ai UserId în Review, verificăm ownership
        if (!string.IsNullOrEmpty(review.UserId) && review.UserId != userId)
            return Forbid();

        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5.");

        if (dto.Comment?.Length > 300)
            return BadRequest("Comment max length is 300.");

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // User: delete doar propriul review
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (review == null) return NotFound();

        if (!string.IsNullOrEmpty(review.UserId) && review.UserId != userId)
            return Forbid();

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

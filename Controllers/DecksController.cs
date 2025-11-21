using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAnki.Data;
using SimpleAnki.Models;

namespace SimpleAnki.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class DecksController : ControllerBase
{
    private readonly AppDbContext _db;
    public DecksController(AppDbContext db) => _db = db;

    // GET /api/decks
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);
        var decks = await _db.Decks
            .Where(d => d.UserId == userId)
            .Include(d => d.Cards)
            .ToListAsync();
        return Ok(decks);
    }

    // GET /api/decks/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);
        var deck = await _db.Decks
            .Include(d => d.Cards)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        return deck == null ? NotFound() : Ok(deck);
    }

    // POST /api/decks
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Deck deck)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = Guid.Parse(User.FindFirst("id")!.Value);
        deck.Id = Guid.NewGuid();
        deck.UserId = userId; // associate deck with current user
        _db.Decks.Add(deck);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = deck.Id }, deck);
    }

    // PUT /api/decks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Deck input)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);
        var deck = await _db.Decks.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
        if (deck == null) return NotFound();

        deck.Title = input.Title;
        deck.Description = input.Description;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/decks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);
        var deck = await _db.Decks.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
        if (deck == null) return NotFound();

        _db.Decks.Remove(deck);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

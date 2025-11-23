using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAnki.Data;
using SimpleAnki.Models;
using SimpleAnki.DTOs;
using SimpleAnki.Requests;

namespace SimpleAnki.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
            .Include(d => d.Cards)
            .Where(d => d.UserId == userId)
            .Select(d => new DeckDto
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                CardsCount = d.Cards.Count,
            })
            .ToListAsync();

            var deckIds = decks.Select(d => d.Id).ToList();

            var cards = await _db.Cards
                .Where(c => deckIds.Contains(c.DeckId))
                .Select(c => new CardDto
                {
                    Id = c.Id,
                    Front = c.Front,
                    Back = c.Back,
                    Order = c.Order,
                    DeckId = c.DeckId,
                    DeckTitle = c.Deck!.Title
                })
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
            .Where(d => d.UserId == userId && d.Id == id)
            .Select(d => new DeckDto
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                CardsCount = d.Cards.Count,
            })
            .FirstOrDefaultAsync();
        
        if (deck == null)
            return NotFound();
        
        var cards = await _db.Cards
            .Where(c => c.DeckId == deck.Id)
            .Select(c => new CardDto
            {
                Id = c.Id,
                Front = c.Front,
                Back = c.Back,
                Order = c.Order,
                DeckId = c.DeckId,
                DeckTitle = deck.Title
            })
            .ToListAsync();

        deck.Cards = cards;

        return deck == null ? NotFound() : Ok(deck);
    }

    // POST /api/decks
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeckRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var deck = new Deck
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            UserId = userId
        };

        _db.Decks.Add(deck);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = deck.Id }, new { deck.Id });
    }

    // PUT /api/decks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeckRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var deck = await _db.Decks.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
        if (deck == null) return NotFound();

        deck.Title = request.Title;
        deck.Description = request.Description;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/decks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var deck = await _db.Decks.Include(d => d.Cards)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (deck == null) return NotFound();

        _db.Decks.Remove(deck);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

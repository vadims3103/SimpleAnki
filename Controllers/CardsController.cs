using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAnki.Data;
using SimpleAnki.Models;

namespace SimpleAnki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CardsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cards = await _db.Cards.Include(c => c.Deck).ToListAsync();
        return Ok(cards);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var card = await _db.Cards.Include(c => c.Deck).FirstOrDefaultAsync(c => c.Id == id);
        return card == null ? NotFound() : Ok(card);
    }

    [HttpGet("by-deck/{deckId:guid}")]
    public async Task<IActionResult> GetByDeck(Guid deckId)
    {
        var cards = await _db.Cards.Where(c => c.DeckId == deckId).ToListAsync();
        return Ok(cards);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Card card)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Ensure deck exists
        var deck = await _db.Decks.FindAsync(card.DeckId);
        if (deck == null) return BadRequest($"Deck {card.DeckId} not found.");

        card.Id = Guid.NewGuid();
        _db.Cards.Add(card);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = card.Id }, card);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Card input)
    {
        var card = await _db.Cards.FindAsync(id);
        if (card == null) return NotFound();
        card.Front = input.Front;
        card.Back = input.Back;
        card.Order = input.Order;
        // not allowing deck change here — we'd add separate endpoint if needed
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var card = await _db.Cards.FindAsync(id);
        if (card == null) return NotFound();
        _db.Cards.Remove(card);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
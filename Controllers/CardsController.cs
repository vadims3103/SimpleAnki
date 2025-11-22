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
public class CardsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CardsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var cards = await _db.Cards
            .Include(c => c.Deck)
            .Where(c => c.Deck != null && c.Deck.UserId == userId)
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

        return Ok(cards);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var card = await _db.Cards
            .Include(c => c.Deck)
            .Where(c => c.Deck != null && c.Deck.UserId == userId)
            .Select(c => new CardDto
            {
                Id = c.Id,
                Front = c.Front,
                Back = c.Back,
                Order = c.Order,
                DeckId = c.DeckId,
                DeckTitle = c.Deck!.Title
            })
            .FirstOrDefaultAsync(c => c.Id == id);

        return card == null ? NotFound() : Ok(card);
    }

    [HttpGet("by-deck/{deckId:guid}")]
    public async Task<IActionResult> GetByDeck(Guid deckId)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var cards = await _db.Cards
            .Include(c => c.Deck)
            .Where(c => c.Deck != null && c.Deck.UserId == userId && c.DeckId == deckId)
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

        return Ok(cards);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var deck = await _db.Decks.FirstOrDefaultAsync(d => d.Id == request.DeckId && d.UserId == userId);
        if (deck == null) return BadRequest("Deck not found or does not belong to you");

        var card = new Card
        {
            Id = Guid.NewGuid(),
            DeckId = request.DeckId,
            Front = request.Front,
            Back = request.Back,
            Order = request.Order
        };

        _db.Cards.Add(card);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = card.Id }, new { card.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCardRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var card = await _db.Cards.Include(c => c.Deck)
            .FirstOrDefaultAsync(c => c.Id == id && c.Deck != null && c.Deck.UserId == userId);

        if (card == null) return NotFound();

        card.Front = request.Front;
        card.Back = request.Back;
        card.Order = request.Order;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);

        var card = await _db.Cards.Include(c => c.Deck)
            .FirstOrDefaultAsync(c => c.Id == id && c.Deck != null && c.Deck.UserId == userId);

        if (card == null) return NotFound();

        _db.Cards.Remove(card);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
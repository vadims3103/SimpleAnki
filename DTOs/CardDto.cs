namespace SimpleAnki.DTOs;
public class CardDto
{
    public Guid Id { get; set; }
    public required string Front { get; set; }
    public required string Back { get; set; }
    public int Order { get; set; }
    public Guid DeckId { get; set; }
    public required string DeckTitle { get; set; }

    public List<ExampleDto> Examples { get; set; } = new();
}
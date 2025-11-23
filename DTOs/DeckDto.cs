namespace SimpleAnki.DTOs
{
    public class DeckDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int CardsCount { get; set; }
        public List<CardDto> Cards { get; set; } = new();
    }
}
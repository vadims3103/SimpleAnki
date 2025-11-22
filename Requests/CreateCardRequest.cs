namespace SimpleAnki.Requests
{
    public class CreateCardRequest
    {
        public Guid DeckId { get; set; }
        public string Front { get; set; } = null!;
        public string Back { get; set; } = null!;
        public int Order { get; set; }
    }
}
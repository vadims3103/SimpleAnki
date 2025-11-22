namespace SimpleAnki.Requests
{
    public class UpdateCardRequest
    {
        public string Front { get; set; } = null!;
        public string Back { get; set; } = null!;
        public int Order { get; set; }
    }
}
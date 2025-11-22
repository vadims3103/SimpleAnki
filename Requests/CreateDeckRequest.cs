namespace SimpleAnki.Requests
{
    public class CreateDeckRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
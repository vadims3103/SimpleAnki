namespace SimpleAnki.Requests
{
    public class UpdateDeckRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
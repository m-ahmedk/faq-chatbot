namespace FAQDemo.API.DTOs.Faq
{
    public class UpdateFaqDto
    {
        public int Id { get; set; }  // required for update
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
}

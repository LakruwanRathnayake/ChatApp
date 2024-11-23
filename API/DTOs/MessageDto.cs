// DTOs/MessageDto.cs
namespace API.DTOs
{
    public class MessageDto
    {
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
    }
}

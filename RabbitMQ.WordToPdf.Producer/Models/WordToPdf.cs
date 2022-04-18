using Microsoft.AspNetCore.Http;

namespace RabbitMQ.WordToPdf.Producer.Models
{
    public class WordToPdf
    {
        public string Email { get; set; }
        public IFormFile WordFile { get; set; }
    }
}

namespace RabbitMQ.WordToPdf.Consumer
{
    public class MessageWordToPdf
    {
        public byte[] WordByte { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }

    }
}

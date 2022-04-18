using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spire.Doc;
using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace RabbitMQ.WordToPdf.Consumer
{
    internal class Program
    {

        public static bool EmailSend(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                memoryStream.Position = 0;

                System.Net.Mime.ContentType ct = new ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);

                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(memoryStream, ct);

                attach.ContentDisposition.FileName = $"{fileName}.pdf";

                MailMessage mailMessage = new MailMessage();

                SmtpClient smtpClient = new SmtpClient();

                mailMessage.From = new MailAddress("kulakberkay15@gmail.com");

                mailMessage.To.Add(email);

                mailMessage.Subject = "Pdf Dosyası | bernersoft.com";

                mailMessage.Body = "pdf dosyanız ektedir";

                mailMessage.IsBodyHtml = true;

                mailMessage.Attachments.Add(attach);

                smtpClient.Host = "smtp.gmail.com";

                smtpClient.Port = 587;

                smtpClient.EnableSsl = true;

                smtpClient.Credentials = new System.Net.NetworkCredential("kulakberkay15@gmail.com", "43795164825Rt*");

                Console.WriteLine($"Sonuç:{email} adresine gönderilmiştir");

                memoryStream.Close();

                memoryStream.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mail gönderim sırasında bir hata meydana geldi: {ex.InnerException}");
                return false;
            }

        }

        private static void Main(string[] args)
        {
            bool result = false;

            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://jihwdhzu:Di2vfztYFMOPXtyiq7D-liqRM462dsVX@tiger.rmq.cloudamqp.com/jihwdhzu");

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("convert-exchange", ExchangeType.Direct, true, false, null);

                    channel.QueueBind(queue: "File", exchange: "convert-exchange", "WordToPdf");

                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("File", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            Console.WriteLine("Kuyruktan bir mesaj alındı ve işleniyor");

                            Document document = new Document();

                            string message = Encoding.UTF8.GetString(ea.Body);

                            MessageWordToPdf messageWordToPdf = JsonConvert.DeserializeObject<MessageWordToPdf>(message);

                            document.LoadFromStream(new MemoryStream(messageWordToPdf.WordByte), FileFormat.Docx2013);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                document.SaveToStream(ms, FileFormat.PDF);

                                result = EmailSend(messageWordToPdf.Email, ms, messageWordToPdf.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Hata meydana geldi:" + ex.Message);
                        }

                        if (result)
                        {
                            Console.WriteLine("Kuyruktan Mesaj başarıla işlendi...");

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    };

                    Console.WriteLine("çıkmak için tıklayınız");

                    Console.ReadLine();
                }
            }
        }
    }
}

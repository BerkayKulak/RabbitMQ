using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}

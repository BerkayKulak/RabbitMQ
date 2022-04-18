using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.WordToPdf.Producer.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RabbitMQ.WordToPdf.Producer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WordToPdfPage()
        {

            return View();
        }

        [HttpPost]
        public IActionResult WordToPdfPage(Models.WordToPdf wordToPdf)
        {

            var factory = new ConnectionFactory();

            factory.Uri = new Uri(_configuration["ConnectionStrings:RabbitMQ"]);

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("convert-exchange", ExchangeType.Direct, true, false, null);

                    channel.QueueDeclare(queue: "File", true, exclusive: false, autoDelete: false, arguments: null);

                    channel.QueueBind("File", "convert-exchange", "WordToPdf");

                    MessageWordToPdf messageWordToPdf = new MessageWordToPdf();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        wordToPdf.WordFile.CopyTo(ms);

                        messageWordToPdf.WordByte = ms.ToArray();
                    }

                    messageWordToPdf.Email = wordToPdf.Email;

                    messageWordToPdf.FileName = Path.GetFileNameWithoutExtension(wordToPdf.WordFile.FileName);

                    string serializeMessage = JsonConvert.SerializeObject(messageWordToPdf);

                    byte[] ByteMessage = Encoding.UTF8.GetBytes(serializeMessage);

                    var properties = channel.CreateBasicProperties();

                    properties.Persistent = true;

                    channel.BasicPublish("convert-exchange", routingKey: "WordToPdf", basicProperties: properties, body: ByteMessage);

                    ViewBag.result =
                        "Word dosyanız Pdf dosyasına dönüştürüldükten sonra size email olarak gönderilecektir.";

                    return View();

                }
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RabbitMQ.subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://jihwdhzu:Di2vfztYFMOPXtyiq7D-liqRM462dsVX@tiger.rmq.cloudamqp.com/jihwdhzu");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // channel.QueueDeclare("hello-queue", true, false, false);

            // rabbit mq mesajları kaç kaç göndericek onu belirttik. 1 1 göndericek.

            // var randomQueueName = channel.QueueDeclare().QueueName;
            //var randomQueueName = "log-database-save-queue";

            //channel.QueueDeclare(randomQueueName, true, false, false);

            // channel.QueueBind(randomQueueName, "logs-fanout", "", null);


            channel.BasicQos(0, 1, false);
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all");

            channel.QueueBind(queueName, "header-exchange", string.Empty, headers);

            channel.BasicConsume(queueName, false, consumer);



            Console.WriteLine("Logları dinliyorum.");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Product product = JsonSerializer.Deserialize<Product>(message);

                Thread.Sleep(1500);

                Console.WriteLine($"Gelen Mesaj: {product.Id} -{product.Name}-{product.Price}-{product.Stok}");



                channel.BasicAck(e.DeliveryTag, false);
            };


            Console.ReadLine();
        }


    }
}

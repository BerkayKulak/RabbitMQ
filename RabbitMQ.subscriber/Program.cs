using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", true,consumer);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine("Gelen Mesaj: "+message);
            };       


            Console.ReadLine();
        }

        
    }
}

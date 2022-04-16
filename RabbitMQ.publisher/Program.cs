﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4,
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://jihwdhzu:Di2vfztYFMOPXtyiq7D-liqRM462dsVX@tiger.rmq.cloudamqp.com/jihwdhzu");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // channel.QueueDeclare("hello-queue", true, false, false);

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            var properties = channel.CreateBasicProperties();

            properties.Headers = headers;

            channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));

            Console.WriteLine("header mesajı gönderildi");

            Console.ReadLine();

        }
    }
}

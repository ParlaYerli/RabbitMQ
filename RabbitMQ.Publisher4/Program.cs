using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Publisher4
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

                    var properties = channel.CreateBasicProperties();
                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format","pdf");
                    headers.Add("shape", "a4");
                    properties.Headers = headers;
                    Console.WriteLine("mesaj gönderildi");
                    channel.BasicPublish("header-exchange",string.Empty,properties, Encoding.UTF8.GetBytes("header mesajım"));
                }
            }
            Console.ReadLine(); 
        }
    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer4
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
                    channel.QueueDeclare("kuyruk1", false, false, false, null);
                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");
                    headers.Add("x-match", "all");
                    channel.QueueBind("kuyruk1", "header-exchange", string.Empty, headers);
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("kuyruk1", false, consumer);
                    consumer.Received += (model, e) =>
                    {
                        byte[] body = e.Body.ToArray();
                        var mesaj = Encoding.UTF8.GetString(body);
                        Console.WriteLine("mesaj alındı" + mesaj);
                        channel.BasicAck(e.DeliveryTag, multiple: false);// consumer, mesajı işledikten sonra broker queueden silebilir.   
                    };
                    Console.WriteLine("Cıkıs yapmak için tıklayın"); 
                }
                Console.ReadLine();
            }
        }
    }
}

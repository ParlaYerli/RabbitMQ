
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.example.fanout.RabbitMQProducer1
{
    //exchange fanout için örnek: queuedeki mesajlar bütün consumerlara iletilir.
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            using (var connection = factory.CreateConnection()) // bağlantıyı açıyorum
            {
                using (var channel = connection.CreateModel()) // kanalı açıyorum
                {
                    channel.ExchangeDeclare("logs",durable:true,type: ExchangeType.Fanout);
                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish("logs", routingKey: "", properties, body: bodyByte); //exchange kullanılmayacaksa ilk parametre bos bırakılır ve default exchange kabul edilir.
                        Console.WriteLine($"Mesajınız gönderilmiştir: {message}-{i}");
                    }

                }
            }
        }
        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}

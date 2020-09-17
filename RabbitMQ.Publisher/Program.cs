using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher
{
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
                    channel.QueueDeclare("task_queue", durable:true,false,false,null); // durable kuyruğun yaşam süresiyle alakalı.
                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish("", routingKey:"task_queue", properties, body: bodyByte); //exchange kullanılmayacaksa ilk parametre bos bıralkılır ve default exchange kabul edilir.
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

//bir mesajın sağlam olmasını garanti altına almak için kuyruk ve mesajda ayar yapılması gerek.(durable:true,persistence:true)
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Consumer
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
                    channel.QueueDeclare("task_queue", durable: true, false, false, null); // durable kuyruğun yaşam süresiyle alakalı.
                    //consumerlar arası eşit dağılım
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);//consumera 1 tane mesaj gelsin . true olursa tüm consumer için , false olursa sadece tek bir consumer için geçerlidir.
                    Console.WriteLine("mesajları bekliyorum ... ");

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("task_queue", autoAck: false, consumer);//autoAck: false -> consumer mesajı işledikten sonra queueye bilgi gönderecek ve mesaj silinecek.
                    consumer.Received += (model, e) =>
                    {
                        byte[] body = e.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("bir mesaj alındı" + message);
                        channel.BasicAck(e.DeliveryTag, multiple: false);// consumer, mesajı işledikten sonra broker queueden silebilir.   
                        Console.WriteLine("mesaj işlendi...");
                    };
                    Console.ReadLine();
                }
            }
        }
    }
}

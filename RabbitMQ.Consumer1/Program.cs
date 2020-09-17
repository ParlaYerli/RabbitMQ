using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Consumer1
{
    //exchange : fanout
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
                    channel.ExchangeDeclare("logs", durable: true, type: ExchangeType.Fanout);
                    //consumer için her instance oluştuğunda , exchange'e bağlı bir kuyruk olusacak.  oluşan her bir kuyruğun isminin birbirinden farklı olmasını sağlar.
                    // bu yüzden random olarak belirlenmesini sağlıyorum.
                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");


                    //consumerlar arası eşit dağılım
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);//consumera 1 tane mesaj gelsin . true olursa tüm consumer için , false olursa sadece tek bir consumer için geçerlidir.
                    Console.WriteLine("logları bekliyorum ... ");

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queueName, autoAck: false, consumer);//autoAck: false -> consumer mesajı işledikten sonra queueye bilgi gönderecek ve mesaj silinecek.
                    consumer.Received += (model, e) =>
                    {
                        byte[] body = e.Body.ToArray();
                        var log = Encoding.UTF8.GetString(body);
                        Console.WriteLine("log alındı" + log);
                        channel.BasicAck(e.DeliveryTag, multiple: false);// consumer, mesajı işledikten sonra broker queueden silebilir.   
                        Console.WriteLine("log bitti...");
                    };
                    Console.ReadLine();
                }

            }
        }
    }
}
 
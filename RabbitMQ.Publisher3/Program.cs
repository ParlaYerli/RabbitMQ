﻿using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher3
{
    //Critical.Error.Info, Warning.Info.Error
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }

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
                    channel.ExchangeDeclare("topic-exchange", durable: true, type: ExchangeType.Topic);
                    Array log_name_array = Enum.GetValues(typeof(LogNames));
                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();
                        LogNames log1= (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length));
                        LogNames log2= (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length));
                        LogNames log3 = (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length));
                        string RoutingKey = $"{log1}.{log2}.{log3}";

                        var bodyByte = Encoding.UTF8.GetBytes($"log={log1.ToString()}-{log2.ToString()}-{log3.ToString()}");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish("topic-exchange", routingKey:RoutingKey, properties, body: bodyByte);
                        Console.WriteLine($"log mesajı gönderilmiştir: mesaj ---> {RoutingKey.ToString()}");
                    }
                    Console.ReadLine();
                }
            }
        }

        public static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}

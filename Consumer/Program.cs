using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

class Program
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }

    static void Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            UserName = "user",
            Password = "password",
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Deserialize JSON back to Message object
            var messageObject = JsonSerializer.Deserialize<Message>(message);

            Console.WriteLine(" [x] Received: {0}, Timestamp: {1}", messageObject.Text, messageObject.Timestamp);
        };
        channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}

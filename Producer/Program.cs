using RabbitMQ.Client;
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

        var messageObject = new Message
        {
            Text = "Hello World!",
            Timestamp = DateTime.Now
        };

        string message = JsonSerializer.Serialize(messageObject);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);

        Console.WriteLine(" [x] Sent {0}", message);
    }
}

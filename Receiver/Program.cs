using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Receiver
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				UserName = "guest",
				Password = "guest",
				VirtualHost = "/"
			};

			var connection = factory.CreateConnection();
			var channel = connection.CreateModel();
			channel.QueueDeclare("book_queue");

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, eventArgs) =>
			{
				var body = eventArgs.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine(message);
			};

			channel.BasicConsume(queue: "booking", autoAck: true, consumer: consumer);
		}
	}
}
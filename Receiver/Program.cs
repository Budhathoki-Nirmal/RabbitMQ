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

			using (var connection = factory.CreateConnection())


			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(queue: "book_queue", durable: true, exclusive: false, autoDelete: false);
				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, eventArgs) =>
				{
					var body = eventArgs.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);
					Console.WriteLine(message);
				};

				channel.BasicConsume(queue: "book_queue", autoAck: true, consumer: consumer);
			}
		}
	}
}
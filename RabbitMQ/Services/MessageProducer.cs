using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Models;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace RabbitMQ.Services
{
	public class MessageProducer : IMessageProducer
	{
		private readonly ConnectionFactory _connectionFactory;
		private readonly string _exchangeName;
		private readonly string _routingKey;
		
		public void SendMessage<T>(T message)
		{
			

			//using (var connection = factory.CreateConnection())
			//using (var channel = connection.CreateModel())
			//{
			//	//connection.ConnectionShutdown += ConnectionShutdownHandler;

			//	// Rest of your publishing logic goes here
			//	channel.QueueDeclare("booking", durable: true, exclusive: true);

			//	var json = JsonSerializer.Serialize(message);
			//	var body = Encoding.UTF8.GetBytes(json);

			//	channel.BasicPublish(exchange: "", routingKey: "booking", body: body);

			//	//Console.WriteLine("Press any key to exit.");
			//	//Console.ReadKey();
			//}

			//var connection = factory.CreateConnection();
			//var channel = connection.CreateModel();
			//channel.ExchangeDeclare(exchange: "Book", type: "Direct", durable: true);
			//channel.QueueDeclare(queue: "Booking", durable: true, exclusive: false, autoDelete: false);
			//channel.QueueBind(queue: "Booking", exchange: "Book", routingKey: "Booking", arguments: null);

			//var json = JsonSerializer.Serialize(message);
			//var body = Encoding.UTF8.GetBytes(json);

			//channel.BasicPublish(exchange: "Book", routingKey: "booking", body: body);

			//var consumer = new EventingBasicConsumer(channel);
			//consumer.Received += (model, eventArgs) =>
			//{
			//	var body = eventArgs.Body.ToArray();
			//	var message = Encoding.UTF8.GetString(body);
			//	Console.WriteLine(message);
			//};

			//channel.BasicConsume(queue: "booking", autoAck: true, consumer: consumer);
			//Console.ReadKey();
		}

		public bool Connect()
		{
			return true;
			//policy.Execute(()=>{

			//})

			//var policy = Policy.Handle
			//var factory = new ConnectionFactory
			//{
			//	HostName = "localhost",
			//	UserName = "guest",
			//	Password = "guest",
			//	VirtualHost = "Coding"

		}

		public void Dispose()
		{
			Console.WriteLine("Dispose");
		}
	}
}

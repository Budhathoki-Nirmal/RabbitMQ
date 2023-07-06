using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Models;
using RabbitMQ.Services;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly RabbitMQService _rabbitMQService;
		private readonly IMessageProducer _messageProducer;
		private readonly ILogger<BookingController> _logger;
		//public List<Booking> _bookings = new List<Booking>();
		//public List<string> _booking1 = new List<string>();
		//private readonly ConnectionFactory _connectionFactory;
		//private readonly string _exchangeName;
		//private readonly string _routingKey;
		//private readonly string _queueName;

		public BookingController(IMessageProducer messageProducer, ILogger<BookingController> logger, IConfiguration configuration, RabbitMQService rabbitMQService)
		{
			_rabbitMQService = rabbitMQService;
			_messageProducer = messageProducer;
			_logger = logger;
			//var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();
			//_connectionFactory = new ConnectionFactory
			//{
			//	HostName = rabbitMQConfig.Hostname,
			//	//Port = rabbitMQConfig.Port,
			//	VirtualHost = rabbitMQConfig.VirtualHost,
			//	UserName = rabbitMQConfig.Username,
			//	Password = rabbitMQConfig.Password
			//};
			//_exchangeName = rabbitMQConfig.ExchangeName;
			//_routingKey = rabbitMQConfig.RoutingKey;
			//_queueName = rabbitMQConfig.QueueName;
			
			//for (int i = 0; i < 50; i++)
			//{
			//	_bookings.Add(new Booking { Id = i+1, PassengerName = "Ram" });

			//}
		}

		[HttpPost]
		public IActionResult PublishMessage(string batchSize)
		{

			try
			{
				_rabbitMQService.PublishMessage(batchSize);
				//_rabbitMQService.PublishMessagesInBatch(_bookings, batchSize);

				return Ok("Message published successfully.");
			}
			catch (Exception ex)
			{
				// Handle the exception
				return StatusCode(500, $"Failed to publish message: {ex.Message}");
			}
		}

		//	[HttpPost]
		//	public IActionResult CreateBooking(Booking newBooking)
		//	{
		//		if (!ModelState.IsValid) return BadRequest();

		//		//MessagePublisherService messagePublisherService;
		//		//_bookings.Add(newBooking);
		//		Console.WriteLine(DateTime.Now);

		//		using (var connection = _connectionFactory.CreateConnection())


		//		using (var channel = connection.CreateModel())
		//		{
		//			foreach (var booking in _bookings)
		//			{

		//					var json = JsonSerializer.Serialize(booking);
		//					var body = Encoding.UTF8.GetBytes(json);
		//				try
		//				{
		//					throw new Exception()
		//					//channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: null, body: body);
		//				}
		//				catch (Exception ex)
		//				{
		//					Console.WriteLine(ex.Message.ToString());
		//					Console.WriteLine(booking);
		//				}


		//				//	channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);

		//				//messagePublisherService





		//				//}
		//				//using (var channel = connection.CreateModel())
		//				//{
		//				//channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
		//				//var consumer = new EventingBasicConsumer(channel);
		//				//consumer.Received += (model, eventArgs) =>
		//				//{
		//				//	var body = eventArgs.Body.ToArray();
		//				//	var message = Encoding.UTF8.GetString(body);
		//				//	_booking1.Add(message);

		//				//};
		//				////Task.Delay(10000).Wait();
		//				//channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);


		//			}


		//		}
		//		Console.WriteLine(DateTime.Now);
		//		Console.WriteLine(_bookings.Count);
		//		//_booking1.Clear();

		//		return Ok();
		//	}
	}
}

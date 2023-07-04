using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Models;
using System;
namespace RabbitMQ.Services
{
	public class RabbitMQService : IDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private readonly IConnection _connection;
		private readonly IModel _channel;
		private readonly string _exchangeName;
		private readonly string _routingKey;
		private readonly string _queueName;
		private TaskCompletionSource<ulong> _messageAckTaskCompletionSource;

		public RabbitMQService(IConfiguration configuration)
		{
			var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();
			_connectionFactory = new ConnectionFactory
			{
				HostName = rabbitMQConfig.Hostname,
				//Port = rabbitMQConfig.Port,
				VirtualHost = rabbitMQConfig.VirtualHost,
				UserName = rabbitMQConfig.Username,
				Password = rabbitMQConfig.Password
			};
			_exchangeName = rabbitMQConfig.ExchangeName;
			_routingKey = rabbitMQConfig.RoutingKey;
			_queueName = rabbitMQConfig.QueueName;
			const int maxRetries = 3;
			int retryCount = 0;
			while (retryCount < maxRetries)
			{
				try
				{
					_connection = _connectionFactory.CreateConnection();
					_channel = _connection.CreateModel();

					_channel.ConfirmSelect();
					_channel.BasicAcks += HandleBasicAck;
					_channel.BasicNacks += HandleBasicNack;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"An error occurred while publishing the message: {ex.Message}");
					retryCount++;
					Console.WriteLine($"Retrying... Attempt {retryCount} of {maxRetries}");
					Thread.Sleep(1000);
				}
			}

			if (retryCount >= maxRetries)
			{
				Console.WriteLine($"Failed to publish the message after {maxRetries} attempts.");
			}


		}

		public async Task PublishMessage(string message)
		{
			for (int i = 0; i < 50000; i++)
			{
				try
				{
					
					var body = Encoding.UTF8.GetBytes(message);
					_messageAckTaskCompletionSource = new TaskCompletionSource<ulong>();
					if(_channel!=null)
					{
						_channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: null, body: body);
						var deliveryTag = await _messageAckTaskCompletionSource.Task;
						if (deliveryTag > 0)
						{
							// Handle successful acknowledgment
							Console.WriteLine($"Message published successfully: DeliveryTag={deliveryTag}");
						}
						else
						{
							// Handle failed acknowledgment
							Console.WriteLine("Message publishing failed");
						}
					}
					
					
				}
				catch (Exception ex) { Console.WriteLine(ex.ToString()); }

			}

		}

		private void HandleBasicAck(object sender, BasicAckEventArgs e)
		{
			_messageAckTaskCompletionSource.TrySetResult(e.DeliveryTag);
		}

		private void HandleBasicNack(object sender, BasicNackEventArgs e)
		{
			_messageAckTaskCompletionSource.TrySetResult(0);
		}

		public void Dispose()
		{
			_channel.Close();
			_connection.Close();
		}
	}
}

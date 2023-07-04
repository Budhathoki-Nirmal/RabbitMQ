using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Models;
using System.Collections.Generic;
using System;
using System.Threading.Channels;
using System.Data.Common;
using System.Text.Json;

namespace RabbitMQ.Services
{
	public class RabbitMQService2 : IDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private readonly IConnection _connection;
		private readonly IModel _channel;
		private readonly string _exchangeName;
		private readonly string _routingKey;
		private readonly string _queueName;
		private TaskCompletionSource<ulong> _messageAckTaskCompletionSource;
		public RabbitMQService2(IConfiguration configuration)
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

			}
		}
		public async Task PublishMessagesInBatch(List<Booking> messages, int batchSize)
		{
			if (_channel != null)
			{
				int totalMessages = messages.Count;
				int publishedMessages = 0;
				
				while (publishedMessages < totalMessages)
				{
					int remainingMessages = totalMessages - publishedMessages;
					int currentBatchSize = Math.Min(batchSize, remainingMessages);
					Console.WriteLine(currentBatchSize);

					for (int i = 0; i < currentBatchSize; i++)
					{
						Booking message = messages[publishedMessages + i];
						var json = JsonSerializer.Serialize(message);
						var body = Encoding.UTF8.GetBytes(json);
						_messageAckTaskCompletionSource = new TaskCompletionSource<ulong>();

						
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
					
					publishedMessages += currentBatchSize;
				}

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

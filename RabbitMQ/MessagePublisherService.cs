
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ
{

	public class MessagePublisherService : BackgroundService
	{
		private readonly ConnectionFactory _connectionFactory;
		private readonly string _exchangeName;
		private readonly string _routingKey;
		private readonly ILogger<MessagePublisherService> _logger;
		private readonly ConcurrentQueue<string> _messageQueue;
		private readonly SemaphoreSlim _semaphore;
		private IConnection _connection;
		private IModel _channel;
		private bool _isConnected;

		public MessagePublisherService(IOptions<RabbitMQConfig> options, ILogger<MessagePublisherService> logger)
		{
			_connectionFactory = new ConnectionFactory
			{
				HostName = options.Value.Hostname,
				VirtualHost= options.Value.VirtualHost,
				//Port = options.Value.Port,
				UserName = options.Value.Username,
				Password = options.Value.Password
			};
			_exchangeName = options.Value.ExchangeName;
			_routingKey = options.Value.RoutingKey;
			_logger = logger;
			_messageQueue = new ConcurrentQueue<string>();
			_semaphore = new SemaphoreSlim(1, 1);
			_isConnected = false;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				if (!_isConnected)
				{
					Connect();
				}

				try
				{
					List<string> messagesToPublish = new List<string>();

					while (_messageQueue.TryDequeue(out var message))
					{
						messagesToPublish.Add(message);
					}

					if (messagesToPublish.Count > 0)
					{
						var publishTasks = messagesToPublish.Select(message => PublishMessageAsync(message));
						await Task.WhenAll(publishTasks);

						_logger.LogInformation($"Published batch of {messagesToPublish.Count} messages.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred while publishing messages.");
					Disconnect();
				}

				// Wait for a specified interval before publishing the next batch
				await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
			}

			Disconnect();
		}

		public void EnqueueMessage(string message)
		{
			_messageQueue.Enqueue(message);
		}

		private async Task PublishMessageAsync(string message)
		{
			await _semaphore.WaitAsync();

			try
			{
				var body = Encoding.UTF8.GetBytes(message);
				_channel.BasicPublish(_exchangeName, _routingKey, null, body);
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private void Connect()
		{
			lock (_semaphore)
			{
				if (!_isConnected)
				{
					_connection = _connectionFactory.CreateConnection();
					_channel = _connection.CreateModel();
					_channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);
					_isConnected = true;
				}
			}
		}

		private void Disconnect()
		{
			lock (_semaphore)
			{
				if (_isConnected)
				{
					_channel.Close();
					_channel.Dispose();
					_connection.Close();
					_connection.Dispose();
					_isConnected = false;
				}
			}
		}
	}

}

namespace RabbitMQ.Models
{
	public class RabbitMQConfig
	{
		public string Hostname { get; set; }
		public string VirtualHost { get; set; }
		//public int Port { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ExchangeName { get; set; }
		public string QueueName { get; set; }
		public string RoutingKey { get; set; }
	}
}

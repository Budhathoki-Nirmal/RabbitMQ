namespace RabbitMQ.Services
{
	public interface IMessageProducer
	{
		bool Connect();
		void Dispose();
		public void SendMessage<T>(T message);
	}
}

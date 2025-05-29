namespace LearnMicroservice.Services
{
	public interface IMyService
	{
		string GetInstanceId();
	}

	public class MyService : IMyService
	{
		private readonly string _id;

		public MyService()
		{
			_id = Guid.NewGuid().ToString();
			Console.WriteLine($"[MyService CREATED] ID: {_id}");
		}

		public string GetInstanceId() => _id;
	}
}

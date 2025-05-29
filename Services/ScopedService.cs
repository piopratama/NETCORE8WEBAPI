namespace LearnMicroservice.Services
{
	public class ScopedService
	{
		public string Id { get; } = Guid.NewGuid().ToString();
	}
}

namespace LearnMicroservice.Services
{
	public class TransientService
	{
		public string Id { get; } = Guid.NewGuid().ToString();
	}
}

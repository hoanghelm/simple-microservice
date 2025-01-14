using Communication.Consumer.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Communication.Consumer
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var serviceProvider = new ServiceCollection().AddServices().BuildServiceProvider();


			await Worker.Delivery(serviceProvider);

		}
	}
}
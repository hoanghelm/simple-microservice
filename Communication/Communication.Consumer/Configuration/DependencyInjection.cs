using Common.Mail;
using Communication.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Consumer.Configuration
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = Environment.GetEnvironmentVariable("REDIS_HOST");
			});

			services.AddScoped<IDeliveryNotification, DeliveryNotification>();
			services.AddMailService();
			return services;
		}
	}
}

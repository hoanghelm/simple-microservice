using Common.Constants;
using StackExchange.Redis;
using System;
using Common.Mail;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Common.Extensions;
using System.Threading;

namespace Communication.Services
{
	public class DeliveryNotification : IDeliveryNotification
	{
		private readonly Lazy<IMailService> _lazyMailService;
		private readonly IDistributedCache _cache;

		public DeliveryNotification(Lazy<IMailService> lazyMailService, IDistributedCache cache)
		{
			_lazyMailService = lazyMailService;
			_cache = cache;
		}

		public async Task ExecuteAsync()
		{
			try
			{
				var redisConnection = Environment.GetEnvironmentVariable("REDIS_HOST");
				string[] keysArr;

				using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnection))
				{
					var endpoints = redis.GetEndPoints();
					var server = redis.GetServer(endpoints.First());
					var keys = server.Keys(-1, CacheKey.COMMENT_POST + "*");
					keysArr = keys.Select(key => (string)key).ToArray();
				}

				foreach (var key in keysArr)
				{
					var noti = await _cache.GetCacheValueAsync<CommentNotificationModel>(key);
					if (noti.NotificationStatus == Common.Enums.ComunicationStatus.COMPLETED)
					{
						continue;
					}

					Console.WriteLine($"{key} - ${noti.NotificationStatus}");
					
					var mailMessage = new MailMessage()
					{
						To = noti.Email,
						Subject = "Notification - Post",
						HtmlMessage = $"{noti.AuthorName} commented on your post."
					};

					noti.NotificationStatus = Common.Enums.ComunicationStatus.DELIVERING;
					_ = _cache.SetCacheValueAsync(key, noti, 60 * 1);

					Console.WriteLine($"{key} - ${noti.NotificationStatus}");

					var result = await _lazyMailService.Value.SendAsync(mailMessage);

					if (result)
					{
						noti.NotificationStatus = Common.Enums.ComunicationStatus.COMPLETED;
						_ = _cache.SetCacheValueAsync(key, noti, 10);
						Console.WriteLine($"{key} - ${noti.NotificationStatus}");
					}
					else
					{
						noti.NotificationStatus = Common.Enums.ComunicationStatus.READY;
						_ = _cache.SetCacheValueAsync(key, noti, 60 * 1);
					}

					Thread.Sleep(2000);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error {ex}");
			}
		}
	}
}

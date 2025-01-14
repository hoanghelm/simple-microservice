using Communication.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Communication.Consumer
{
    public static class Worker
    {
        public static async Task Delivery(IServiceProvider services)
        {
            await services.GetService<IDeliveryNotification>().ExecuteAsync();
        }
    }
}
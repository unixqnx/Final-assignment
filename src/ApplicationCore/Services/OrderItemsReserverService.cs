using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public interface IOrderItemsReserverService
{
    Task ReserveOrderItemsAsync(Order order);
}

public class OrderItemsReserverService : IOrderItemsReserverService
{
    private readonly string _queueConnectionString;
    private readonly string _queueName;

    public OrderItemsReserverService(IOptions<OrderItemsReserverSettings> options)
    {
        _queueConnectionString = options.Value.QueueConnectionString;
        _queueName = options.Value.QueueName;
    }

    public async Task ReserveOrderItemsAsync(Order order)
    {
        var messageContent = JsonSerializer.Serialize(order);

        var message = new ServiceBusMessage(messageContent);

        await using var client = new ServiceBusClient(_queueConnectionString);
        await using var sender = client.CreateSender(_queueName);
        await sender.SendMessageAsync(message);
    }
}

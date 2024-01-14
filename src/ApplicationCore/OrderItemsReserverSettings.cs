namespace Microsoft.eShopWeb.ApplicationCore;

public sealed class OrderItemsReserverSettings
{
    public string QueueConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}

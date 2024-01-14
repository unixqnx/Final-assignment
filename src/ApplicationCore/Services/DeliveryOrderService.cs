using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public interface IDeliveryOrderService
{
    Task CreateDeliveryOrderAsync(Order order);
}

public class DeliveryOrderService: IDeliveryOrderService
{
    private readonly string _deliveryOrderServerUrl;

    private readonly HttpClient _httpClient;

    public DeliveryOrderService(IOptions<DeliveryOrderSettings> options)
    {
        _deliveryOrderServerUrl = options.Value.DeliveryOrderServerUrl;
        _httpClient = new HttpClient();
    }

    public async Task CreateDeliveryOrderAsync(Order order)
    {
        //var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
        //await _httpClient.PostAsync(_deliveryOrderServerUrl, content);
    }
}

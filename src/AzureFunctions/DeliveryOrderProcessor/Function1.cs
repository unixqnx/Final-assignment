using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using DeliveryOrderProcessor.Dto;

namespace DeliveryOrderProcessor;

public static class Function1
{
    private const string COSMOS_ENDPOINT = "https://final-assignment-cosmos.documents.azure.com:443/";
    private const string COSMOS_KEY = "9WQX8lGwfiFwUqD9ql8nqlNtqo6aoABPeB6GCdpUAgtVE5SBRspXOD4G3cgc0jT87UDn5szhMmzQACDbHQsmnQ==";
    private const string DATABASE_ID = "final-assignment-container";
    private const string CONTAINER_ID = "delivery-order-items";

    [FunctionName("DeliveryOrderProcessor")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string json = await req.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<Order>(json);
        order.id = Guid.NewGuid().ToString();

        var client = new CosmosClient(COSMOS_ENDPOINT, COSMOS_KEY);
        var database = client.GetDatabase(DATABASE_ID);
        var container = database.GetContainer(CONTAINER_ID);

        ItemResponse<Order> response = await container.CreateItemAsync(order);

        return new OkObjectResult("OK");
    }
}

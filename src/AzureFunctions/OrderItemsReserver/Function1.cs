using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Dto;


namespace OrderItemsReserver
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run([ServiceBusTrigger("finalassignmentqueue", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            #region configuration

            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var blobConnectionString = config["BlobConnectionString"];
            var fileContainerName = config["FileContainerName"];
            var logicAppUrl = config["LogicAppUrl"];
            var email = config["Email"];

            #endregion

            var order = JsonConvert.DeserializeObject<Order>(myQueueItem);
            var orderId = Guid.NewGuid().ToString();
            order.id = orderId;
            var attachmentsName = $"order_request_{orderId}.json";

            var messageContent = System.Text.Json.JsonSerializer.Serialize(order);

            var sleepTyme = 1000;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(fileContainerName);
                    BlobClient blobClient = containerClient.GetBlobClient(attachmentsName);
                    var data = new BinaryData(messageContent);
                    var response = await blobClient.UploadAsync(data);
                    return;
                }
                catch
                {
                    if (i < 2)
                    {
                        Thread.Sleep(sleepTyme);
                        sleepTyme *= 2;
                    }
                }
            }

            var emailService = new EmailService(logicAppUrl);
            var result = await emailService.SendAsync(email, attachmentsName, messageContent);
        }
    }
}

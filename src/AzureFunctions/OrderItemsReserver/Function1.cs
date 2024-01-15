using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Dto;

namespace OrderItemsReserver
{
    public class Function1
    {
        [FunctionName("OrderItemsReserver")]
        public async Task Run([ServiceBusTrigger("finalassignmentqueue", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            #region configuration

            var blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=finalassignment;AccountKey=4KcT3GVF0FuM5Gsg6bkAwxkMoiaSwq6tOezeNc1zqJj1NVhsnsWjfDVdMGM0NeKhrAHbIcVNKLAj+AStd7KoyQ==;EndpointSuffix=core.windows.net";
            var fileContainerName = "final-assignment-container";
            var logicAppUrl = "https://prod-34.eastus2.logic.azure.com:443/workflows/7c4815333e8748f19eef647ddcd992c7/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=3LKGhYf3gn2XvRWVOwRdHBveFWgYKTeHNkaLpXmwy8M";
            var email = "unixqnx@mail.ru";

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

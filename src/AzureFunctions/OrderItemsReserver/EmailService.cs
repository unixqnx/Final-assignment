using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderItemsReserver;
internal class EmailService
{
    private readonly string _logicAppUrl;

    public EmailService(string logicAppUrl)
    {
        _logicAppUrl = logicAppUrl;
    }

    public async Task<HttpResponseMessage> SendAsync(string to, string attachmentsName, string attachmentsContent)
    {
        using var client = new HttpClient();
        var jsonData = JsonSerializer.Serialize(new
        {
            To = to,
            Subject = "OrderItemsReserver service fallback.",
            Body = "Please find Order in the attachment.",
            AttachmentsName = attachmentsName,
            AttachmentsContent = attachmentsContent,
            AttachmentsContentType = "json"
        });

        return await client.PostAsync(_logicAppUrl, new StringContent(jsonData, Encoding.UTF8, "application/json"));
    }
}

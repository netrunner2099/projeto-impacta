using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;

namespace Credenciamento.Application.Services.QrCode;

public interface IQrCodeClient
{
    Task<QRCodeResponse> GenerateAsync(QrCodeRequest data);
}

public class QrCodeClient : IQrCodeClient
{
    private readonly ILogger _logger;
    private readonly QrCodeOptions _qrCodeOptions;

    public QrCodeClient(
        ILogger<QrCodeClient> logger,
        IOptions<QrCodeOptions> qrCodeOptions)
    {
        _logger = logger;
        _qrCodeOptions = qrCodeOptions.Value;
    }

    public async Task<QRCodeResponse> GenerateAsync(QrCodeRequest data)
    {
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _qrCodeOptions.Endpoint);
            request.Headers.Add("x-api-key", _qrCodeOptions.Token);
            var content = new StringContent($"{{\"data\":\"{data.Data}\"}}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            return JsonSerializer.Deserialize<QRCodeResponse>(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QrCodeGenerate: {0}", ex.Message);
            return null;
        }
    }
}



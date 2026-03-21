namespace Credenciamento.Application.Models;

public class QRCodeResponse
{
    [JsonPropertyName("base64")]
    public string Base64 { get; set; }
    
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; }

    [JsonPropertyName("data_url")]
    public string DataUrl { get; set; }
}



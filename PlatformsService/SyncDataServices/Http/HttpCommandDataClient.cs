using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformsService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<HttpCommandDataClient> _logger;
  private readonly IConfiguration _config;

  public HttpCommandDataClient(
    HttpClient httpClient,
    ILogger<HttpCommandDataClient> logger,
    IConfiguration config)
  {
    _httpClient = httpClient;
    _logger = logger;
    _config = config;
  }

  public async Task SendPlatformToCommand(PlatformReadDto platform)
  {
    var httpContent = new StringContent(
      JsonSerializer.Serialize(platform),
      Encoding.UTF8,
      "application/json"
    );

    var response = await _httpClient.PostAsync($"{_config["CommandsService"]}/api/c/platforms", httpContent);

    if (response.IsSuccessStatusCode)
    {
      _logger.LogInformation("--> Sync POST to CommandsService: Success");
    }
    else
    {
      _logger.LogError("--> Sync POST to CommandsService: Error");
    }
  }
}

using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
  private readonly ILogger<PlatformDataClient> _logger;
  private readonly IConfiguration _configuration;
  private readonly IMapper _mapper;

  public PlatformDataClient(
    ILogger<PlatformDataClient> logger,
    IConfiguration configuration,
    IMapper mapper)
  {
    _logger = logger;
    _configuration = configuration;
    _mapper = mapper;
  }
  public IEnumerable<Platform> ReturnAllPlatforms()
  {
    _logger.LogInformation($"--> Getting Platforms with Grpc Service {_configuration["GrpcPlatform"]}");

    var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
    var client = new GrpcPlatform.GrpcPlatformClient(channel);
    var request = new GetAllRequest();

    try
    {
      var response = client.GetAllPlatforms(request);
      return _mapper.Map<IEnumerable<Platform>>(response.Platforms);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "--> Could not call GRPC Server");
    }

    return null;
  }
}

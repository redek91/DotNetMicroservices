using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
  private readonly ILogger<EventProcessor> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly IMapper _mapper;

  public EventProcessor(ILogger<EventProcessor> logger,
    IServiceProvider serviceProvider,
    IMapper mapper)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
    _mapper = mapper;
  }

  public void ProcessEvent(string message)
  {
    var eventType = DetermineEvent(message);

    switch (eventType)
    {
      case EventType.PlatformPublished:
        _logger.LogInformation("--> Received PlatformPublished Event");
        AddPlatform(message);
        break;
      case EventType.Undetermined:
        _logger.LogWarning("--> Received Undetermined Event");
        break;
    }
  }

  private void AddPlatform(string platformPublishedMessage)
  {
    using (var scope = _serviceProvider.CreateScope())
    {
      var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

      var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
      var platformModel = _mapper.Map<Platform>(platformPublishedDto);

      try
      {
        if (!repo.ExternalPlatformExists(platformModel.ExternalId))
        {
          repo.CreatePlatform(platformModel);
          repo.SaveChanges();
          _logger.LogInformation("--> Platform created");
        }
        else
        {
          _logger.LogInformation("--> Platform already exists...");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "--> Could not add Platform to Db.");
      }

    }
  }

  private EventType DetermineEvent(string notificationMessage)
  {
    _logger.LogInformation("--> Determining Event");
    var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
    return eventType.Event switch
    {
      "Platform_Published" => EventType.PlatformPublished,
      _ => EventType.Undetermined
    };
  }
}

enum EventType
{
  PlatformPublished,
  Undetermined
}

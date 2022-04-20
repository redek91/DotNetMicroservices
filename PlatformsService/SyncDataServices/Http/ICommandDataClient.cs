using PlatformService.Dtos;

namespace PlatformsService.SyncDataServices.Http;

public interface ICommandDataClient
{
  Task SendPlatformToCommand(PlatformReadDto platform);
}

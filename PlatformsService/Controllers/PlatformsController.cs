using System.ComponentModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformsService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly ILogger<PlatformsController> _logger;
  private readonly IPlatformRepo _repo;
  private readonly IMapper _mapper;
  private readonly ICommandDataClient _commandDataClient;
  private readonly IMessageBusClient _messageBusClient;

  public PlatformsController(
    ILogger<PlatformsController> logger,
    IPlatformRepo repo,
    IMapper mapper,
    ICommandDataClient commandDataClient,
    IMessageBusClient messageBusClient)
  {
    _repo = repo;
    _mapper = mapper;
    _commandDataClient = commandDataClient;
    _messageBusClient = messageBusClient;
    _logger = logger;
  }

  /// <summary>
  /// Returns all Platforms.
  /// </summary>
  /// <returns>List of <see cref="PlatformCreateDto"/></returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<PlatformReadDto>), StatusCodes.Status200OK)]
  public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
  {
    _logger.LogInformation("Returning Platforms.");
    var platforms = _repo.GetAllPlatforms();
    return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
  }

  /// <summary>
  /// Returns a specific Platform with given id
  /// </summary>
  /// <param name="id">Platform Id</param>
  /// <returns>Platform.</returns>
  [HttpGet("{id}", Name = nameof(GetPlatformById))]
  [ProducesResponseType(typeof(PlatformReadDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult<PlatformReadDto> GetPlatformById(int id)
  {
    _logger.LogInformation($"Returning Platform with id {id}");

    var platform = _repo.GetPlatformById(id);

    if (platform == null) return NotFound();

    return Ok(_mapper.Map<PlatformReadDto>(platform));
  }

  /// <summary>
  /// Creates a new Platform
  /// </summary>
  /// <param name="platformCreateDto"><see cref="PlatformCreateDto"/></param>
  /// <returns><see cref="PlatformReadDto"/></returns>
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
  {
    _logger.LogInformation("Adding new Platform");

    var platformModel = _mapper.Map<Platform>(platformCreateDto);
    _repo.CreatePlatform(platformModel);
    _repo.SaveChanges();

    var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

    // Send Sync Message
    try
    {
      await _commandDataClient.SendPlatformToCommand(platformReadDto);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Could not send synchronously");
    }

    // Send Async Message
    var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformModel);
    platformPublishedDto.Event = "Platform_Published";
    _messageBusClient.PublishNewPlatform(platformPublishedDto);

    return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
  }

  /// <summary>
  /// Deletes an existing Platform
  /// </summary>
  /// <param name="id">Platform Id</param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult DeletePlatform(int id)
  {
    var platformToDelete = _repo.GetPlatformById(id);

    if (platformToDelete == null)
    {
      _logger.LogError($"No Platform with id:{id}");
      return NotFound();
    }

    _logger.LogInformation($"Delete Platform with id {id}");
    _repo.DeletePlatform(platformToDelete);
    _repo.SaveChanges();
    return Ok();
  }
}

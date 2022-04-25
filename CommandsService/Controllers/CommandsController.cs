using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
  private readonly ILogger<CommandsController> _logger;
  private readonly ICommandRepo _repository;
  private readonly IMapper _mapper;

  public CommandsController(
    ILogger<CommandsController> logger,
    ICommandRepo repository,
    IMapper mapper)
  {
    _logger = logger;
    _repository = repository;
    _mapper = mapper;
  }

  [HttpGet]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
  {
    _logger.LogInformation($"--> Hit GetCommandsForPlatform: {platformId}");
    if (!_repository.PlatformExists(platformId)) return NotFound();

    var commands = _repository.GetCommandsForPlatform(platformId);
    return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
  }

  [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
  {
    _logger.LogInformation($"--> Get command {commandId} for platform {platformId}");
    if (!_repository.PlatformExists(platformId)) return NotFound();

    var command = _repository.GetCommand(platformId, commandId);
    if (command == null) return NotFound();

    return Ok(_mapper.Map<CommandReadDto>(command));
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto command)
  {
    _logger.LogInformation($"--> Create new Command for platform {platformId}");
    if (!_repository.PlatformExists(platformId)) return NotFound();

    var commandModel = _mapper.Map<Command>(command);

    _repository.CreateCommand(platformId, commandModel);

    if (_repository.SaveChanges())
    {
      var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);
      return CreatedAtRoute(
        nameof(GetCommandForPlatform),
        new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
    else
    {
      _logger.LogError("--> error on CreateCommandForPlatform");
      return BadRequest();
    }
  }

}

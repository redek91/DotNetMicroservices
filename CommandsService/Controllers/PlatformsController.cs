using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly ILogger<PlatformsController> _logger;
  private readonly ICommandRepo _repository;
  private readonly IMapper _mapper;

  public PlatformsController(
    ILogger<PlatformsController> logger,
    ICommandRepo repository,
    IMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
    _logger = logger;
  }

  [HttpGet]
  public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
  {
    _logger.LogInformation("--> Getting Platforms from CommandsService");
    var platformItems = _repository.GetAllPlatforms();

    return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
  }

  /// <summary>
  /// Test Call for inbound post
  /// </summary>
  /// <returns></returns>
  [HttpPost]
  public ActionResult TestInboundConnection()
  {
    _logger.LogInformation("--> Inbound POST # Command Service");

    return Ok("Inbound test of from Platforms Controller");
  }
}
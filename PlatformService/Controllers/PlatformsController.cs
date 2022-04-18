using System.ComponentModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly ILogger<PlatformsController> _logger;
  private readonly IPlatformRepo _repo;
  private readonly IMapper _mapper;

  public PlatformsController(
    ILogger<PlatformsController> logger,
    IPlatformRepo repo,
    IMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
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

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
  {
    _logger.LogInformation("Adding new Platform");

    var platformModel = _mapper.Map<Platform>(platformCreateDto);
    _repo.CreatePlatform(platformModel);
    _repo.SaveChanges();

    var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

    return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
  }
}

using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly ILogger<PlatformsController> _logger;

  public PlatformsController(
    ILogger<PlatformsController> logger)
  {
    _logger = logger;
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
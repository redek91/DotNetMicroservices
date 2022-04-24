using PlatformService.Models;

namespace PlatformService.Data;
public class PlatformRepo : IPlatformRepo
{
  private readonly AppDbContext _context;

  public PlatformRepo(AppDbContext context)
  {
    _context = context;
  }
  public void CreatePlatform(Platform platform)
  {
    _ = platform ?? throw new ArgumentNullException(nameof(platform));
    _context.Add<Platform>(platform);
  }

  public IEnumerable<Platform> GetAllPlatforms()
  {
    return _context.Platforms.ToList();
  }

  public Platform GetPlatformById(int id)
  {
    return _context.Platforms.FirstOrDefault(x => x.Id == id);
  }

  public void DeletePlatform(Platform platform)
  {
    if (platform != null) _context.Remove(platform);
  }

  public bool SaveChanges()
  {
    return _context.SaveChanges() >= 0;
  }
}
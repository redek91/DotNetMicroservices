using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data;

public static class PrepDb
{
  public static void PrepPopulation(IApplicationBuilder app)
  {
    using (var serviceScope = app.ApplicationServices.CreateScope())
    {
      SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }
  }

  private static void SeedData(AppDbContext context)
  {
    if (context.Database.IsSqlServer())
    {
      Console.WriteLine("--> Attempt to apply migrations");
      try
      {
        context.Database.Migrate();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Could not run migrations:{ex.Message}");
      }

    }

    if (!context.Platforms.Any())
    {
      Console.WriteLine("--> Seeding Data...");
      context.Platforms.AddRange(
        new Models.Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
        new Models.Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
        new Models.Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Fundation", Cost = "Free" }
      );

      context.SaveChanges();
    }
    else
    {
      Console.WriteLine("--> We already have data");
    }
  }
}

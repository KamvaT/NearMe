using Microsoft.EntityFrameworkCore;

namespace NearMe.Infrastructure.Persistence;

/// <summary>
/// The single EF Core unit of work for the NearMe SQL Server database.
/// </summary>
/// <remarks>
/// Phase 1 intentionally defines no entities. Entities are added with their feature
/// (Identity first, then the marketplace model), each with an IEntityTypeConfiguration
/// in Persistence/Configurations and an EF Core migration.
/// </remarks>
public sealed class NearMeDbContext(DbContextOptions<NearMeDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NearMeDbContext).Assembly);
    }
}

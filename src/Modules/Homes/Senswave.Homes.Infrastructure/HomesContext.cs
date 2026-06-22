using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Sharings.Entities;
using Senswave.Infrastructure.Extensions;
using Senswave.Infrastructure.Persistence;

namespace Senswave.Homes.Infrastructure;

public class HomesContext(IOptions<PostgreSqlOptions> options) : DbContext(options.GetPostgresOptions<HomesContext>())
{
    public DbSet<Home> Homes { get; set; }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<HomeSharing> HomeSharings { get; set; }

    public DbSet<HomeSharingInvitation> HomeInvitations { get; set; }

    public DbSet<DataSourceReference> DataSourceReferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Homes");

        modelBuilder.Entity<Home>(entity =>
        {
            entity.ToTable("Homes");

            entity.HasIndex(x => new { x.OwnerId, x.Name })
                .IsUnique()
                .HasDatabaseName("Home_OwnerId_Name");

            entity.OwnsOne(x => x.Location)
                 .ToTable("Location");

            entity.HasMany(h => h.Rooms)
                .WithOne(r => r.Home)
                .HasForeignKey(r => r.HomeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.DataSourceReference)
                .WithOne(x => x.Home)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Rooms");

            entity.HasIndex(x => new { x.HomeId, x.Name })
                  .IsUnique()
                  .HasDatabaseName("Room_HomeId_Name");
        });

        modelBuilder.Entity<HomeSharing>(entity =>
        {
            entity.ToTable("HomeSharing");

            entity.HasIndex(x => new { x.UserId, x.HomeId })
                  .IsUnique()
                  .HasDatabaseName("HomeSharing_UserId_HomeId");
        });

        modelBuilder.Entity<HomeSharingInvitation>(entity =>
        {
            entity.ToTable("HomeSharingInvitation");

            entity.HasIndex(x => new { x.HomeId, x.FriendId })
                  .IsUnique()
                  .HasDatabaseName("HomeSharingInvitation_HomeId_FriendId");

            entity.HasOne(x => x.Home)
                .WithMany(x => x.HomeSharingInvitations)
                .HasForeignKey(x => x.HomeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DataSourceReference>(entity =>
        {
            entity.ToTable("DataSourceReference");

            entity.HasIndex(x => x.DataSourceId)
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
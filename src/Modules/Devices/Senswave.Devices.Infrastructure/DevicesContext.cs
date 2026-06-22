using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.ShareDevices.Entites;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Infrastructure.Extensions;
using Senswave.Infrastructure.Persistence;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Infrastructure;

public class DevicesContext(IOptions<PostgreSqlOptions> options) : DbContext(options.GetPostgresOptions<DevicesContext>())
{
    public DbSet<DeviceSharing> DeviceSharings { get; set; }

    public DbSet<Device> Devices { get; set; }

    public DbSet<DeviceTile> DeviceTiles { get; set; }

    public DbSet<DevicePresence> DevicePresence { get; set; }

    public DbSet<Dashboard> Dashboards { get; set; }

    public DbSet<Widget> Widgets { get; set; }

    public DbSet<Operation> Operations { get; set; }

    public DbSet<HomeReference> HomeReferences { get; set; }

    public DbSet<DataSourceDataReference> DataReferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Devices");

        // Device Entities
        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("Devices");

            entity.HasOne(d => d.Tile)
                .WithOne(t => t.Device)
                .HasForeignKey<DeviceTile>(t => t.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Presence)
                .WithOne(t => t.Device)
                .HasForeignKey<DevicePresence>(t => t.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Operations)
                .WithOne(o => o.Device)
                .HasForeignKey(o => o.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Dashboards)
                .WithOne(o => o.Device)
                .HasForeignKey(o => o.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.DeviceSharings)
                .WithOne(x => x.Device)
                .HasForeignKey(ds => ds.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.DataReferences)
                .WithOne(x => x.Device)
                .HasForeignKey(ds => ds.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DeviceTile>(entity =>
        {
            entity.ToTable("DeviceTiles");

            entity.Property(t => t.Configuration)
                .HasConversion(
                    v => v.ToJsonString(null),
                    v => JsonObject.Parse(v, null, default)!.AsObject()
                )
                .HasColumnType("jsonb");

            entity.HasOne(t => t.SwitchOperation)
                .WithMany()
                .HasForeignKey(t => t.SwitchOperationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.DisplayableOperation)
                .WithMany()
                .HasForeignKey(t => t.DisplayableOperationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DevicePresence>(entity =>
        {
            entity.ToTable("DevicePresence");

            entity.HasOne(p => p.Operation)
                .WithMany()
                .HasForeignKey(p => p.OperationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Dashboards Entity
        modelBuilder.Entity<Dashboard>(entity =>
        {
            entity.ToTable("Dashboards");

            entity.Property(o => o.Configuration)
                .HasConversion(
                    v => v.ToJsonString(null),
                    v => JsonObject.Parse(v, null, default)!.AsObject()
                )
                .HasColumnType("jsonb");

            entity.HasIndex(x => new { x.DeviceId, x.Name })
                .IsUnique();
        });

        // Widgets Entity
        modelBuilder.Entity<Widget>(entity =>
        {
            entity.ToTable("Widgets");

            entity.Property(o => o.Configuration)
                .HasConversion(
                    v => v.ToJsonString(null),
                    v => JsonObject.Parse(v, null, default)!.AsObject()
                )
                .HasColumnType("jsonb");

            entity.HasOne(w => w.Operation)
                .WithMany(o => o.Widgets)
                .HasForeignKey(w => w.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasIndex(x => new { x.OperationId, x.Name })
                .IsUnique();
        });

        // Operations Entity with JsonObject Conversion
        modelBuilder.Entity<Operation>(entity =>
        {
            entity.ToTable("Operations");

            entity.Property(o => o.Configuration)
                .HasConversion(
                    v => v.ToJsonString(null),
                    v => JsonObject.Parse(v, null, default)!.AsObject()
                )
                .HasColumnType("jsonb");

            entity.HasOne(o => o.DataReference)
                .WithMany(x => x.Operations)
                .HasForeignKey(o => o.DataReferenceId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(x => x.Widgets)
                .WithOne(x => x.Operation)
                .HasForeignKey(x => x.OperationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.OwnsMany(o => o.Values, values =>
            {
                values.ToTable("OperationValues");

                values.Property(v => v.InternalValue)
                    .HasConversion(
                        v => v.ToJsonString(null),
                        v => JsonObject.Parse(v, null, default)!.AsObject()
                    )
                    .HasColumnType("jsonb");
            });
        });

        // HomeReference Entity
        modelBuilder.Entity<HomeReference>(entity =>
        {
            entity.ToTable("HomeReferences");
            entity.HasIndex(hr => new { hr.HomeId, hr.OwnerId })
                .IsUnique();
        });

        // DataSourceDataReference Entity
        modelBuilder.Entity<DataSourceDataReference>(entity =>
        {
            entity.ToTable("DataReferences");

            entity.HasOne(d => d.Device)
                .WithMany(x => x.DataReferences)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(d => d.Operations)
                .WithOne(x => x.DataReference)
                .HasForeignKey(d => d.DataReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => d.DataSourceDataReferenceId)
                .IsUnique();
        });

        // DeviceSharing Entity
        modelBuilder.Entity<DeviceSharing>(entity =>
        {
            entity.ToTable("DeviceSharing");

            entity.HasIndex(ds => new { ds.UserId, ds.DeviceId })
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}